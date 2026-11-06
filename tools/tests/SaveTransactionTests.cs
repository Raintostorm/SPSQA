using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Project.Data;
using Project.Meta;
namespace UnityEngine
{
    public class Sprite{}
    public enum RuntimeInitializeLoadType{SubsystemRegistration}
    public sealed class RuntimeInitializeOnLoadMethodAttribute:Attribute{public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t){}}
    public static class Application{public static string persistentDataPath;}
    public static class Debug{public static void Log(object m){}public static void LogWarning(object m){}public static void LogError(object m){}}
    public static class Mathf{public static int Max(int a,int b)=>Math.Max(a,b);public static int Clamp(int a,int b,int c)=>Math.Min(c,Math.Max(b,a));}
    // Field-based like Unity's JsonUtility; throws on empty/truncated input where Unity returns null or throws.
    public static class JsonUtility
    {
        static readonly JsonSerializerOptions O=new(){IncludeFields=true,WriteIndented=true};
        public static string ToJson(object o,bool pretty=false)=>JsonSerializer.Serialize(o,o.GetType(),O);
        public static T FromJson<T>(string s)=>JsonSerializer.Deserialize<T>(s,O);
        public static void FromJsonOverwrite(string s,object target)
        {
            var fresh=JsonSerializer.Deserialize(s,target.GetType(),O);
            foreach(var f in target.GetType().GetFields(BindingFlags.Public|BindingFlags.Instance))f.SetValue(target,f.GetValue(fresh));
        }
    }
}
namespace Project.Data
{
    public class GameBalance
    {
        public Rank TenRollFloor=Rank.Epic;public int LegendaryPity=80;public int[] TreasureWeights={70,23,6,1};
        public int GoldForLevel(int level,bool first)=>first?1000:400;public int KeysForLevel(int level,bool first)=>first?3:1;
    }
    public class TreasureDatabase
    {
        public Dictionary<Rank,List<TreasureItem>> Pools=new();
        public List<TreasureItem> OfTier(Rank r)=>Pools.TryGetValue(r,out var l)?l:new List<TreasureItem>();
        public TreasureItem Find(string id)=>Pools.Values.SelectMany(l=>l).FirstOrDefault(i=>i.Id==id);
    }
}
class SaveTransactionTests
{
    static string dir;
    static string F(string name)=>Path.Combine(dir,name);
    static void Assert(bool ok,string m){if(!ok)throw new Exception("FAIL: "+m);}
    static void Fresh(){typeof(SaveSystem).GetField("current",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,null);}
    static void Write(string name,SaveData d)=>File.WriteAllText(F(name),UnityEngine.JsonUtility.ToJson(d,true));
    static SaveData Data(int gold){return new SaveData{Gold=gold,AllyLevelResetRevision=1};}
    static void Reset(){foreach(var f in Directory.GetFiles(dir))File.Delete(f);UnityEngine.Application.persistentDataPath=dir;Fresh();}
    static TreasureDatabase Db(bool withLegendary)
    {
        var db=new TreasureDatabase();
        foreach(Rank r in Enum.GetValues(typeof(Rank)))
            if(r!=Rank.Legendary||withLegendary)db.Pools[r]=new List<TreasureItem>{new TreasureItem{Id="item_"+r,Tier=r,MaxLevel=10}};
        return db;
    }
    static void Main()
    {
        dir=Path.Combine(Path.GetTempPath(),"spinsquad-save-test-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(dir);
        try
        {
            // 1. Round trip; second save keeps the first as save.prev.json.
            Reset();SaveSystem.Current.Gold=111;Assert(SaveSystem.Save(),"save returns true");
            SaveSystem.Current.Gold=222;Assert(SaveSystem.Save(),"second save");
            Fresh();Assert(SaveSystem.Current.Gold==222,"round trip");
            Assert(File.Exists(F("save.prev.json"))&&File.ReadAllText(F("save.prev.json")).Contains("111"),"previous good copy kept");
            Assert(!File.Exists(F("save.json.tmp")),"tmp consumed");

            // 2. Old crash window: save.json deleted, .tmp complete → recover .tmp.
            Reset();Write("save.json.tmp",Data(555));
            Assert(SaveSystem.Current.Gold==555,"recover from .tmp when save.json missing");

            // 3. Corrupt save.json → archived to .bak, previous copy used.
            Reset();File.WriteAllText(F("save.json"),"{\"Gold\":9");Write("save.prev.json",Data(333));
            Assert(SaveSystem.Current.Gold==333,"recover from save.prev.json");
            Assert(File.Exists(F("save.bak.json")),"corrupt file archived, not deleted");

            // 4. Truncated .tmp is skipped in favour of .prev.
            Reset();File.WriteAllText(F("save.json.tmp"),"{\"Gold\":");Write("save.prev.json",Data(444));
            Assert(SaveSystem.Current.Gold==444,"truncated tmp skipped");

            // 5. Normalize: null lists, bad rows, negative numbers.
            Reset();File.WriteAllText(F("save.json"),"{\"Gold\":-5,\"AllyLevelResetRevision\":1,\"Items\":[{\"Id\":\"a\",\"Level\":-3,\"Shards\":-1},null],\"Units\":null}");
            var n=SaveSystem.Current;
            Assert(n.Gold==0&&n.Units!=null&&n.Items.Count==1&&n.Items[0].Level==1&&n.Items[0].Shards==0,"normalized");

            // 6. Transact rollback on false / exception keeps the same object.
            Reset();var s=SaveSystem.Current;s.Gold=100;SaveSystem.Save();
            Assert(!SaveSystem.Transact(d=>{d.Gold=0;d.Items.Add(new SaveData.OwnedItem{Id="x"});return false;}),"false aborts");
            Assert(ReferenceEquals(s,SaveSystem.Current)&&s.Gold==100&&s.Items.Count==0,"rollback after false");
            Assert(!SaveSystem.Transact(d=>{d.Gold=0;throw new InvalidOperationException();}),"exception aborts");
            Assert(s.Gold==100,"rollback after exception");

            // 7. Disk failure → rollback, reports false.
            UnityEngine.Application.persistentDataPath=Path.Combine(dir,"missing","nested");
            Assert(!SaveSystem.Transact(d=>{d.Gold=7;return true;}),"write failure reported");
            Assert(s.Gold==100,"rollback after write failure");
            UnityEngine.Application.persistentDataPath=dir;

            // 8. Chest: not enough keys → nothing changes.
            Reset();s=SaveSystem.Current;s.TreasureKeys=3;SaveSystem.Save();
            Assert(!GameProgress.TryOpenChest(5,Db(true),new GameBalance(),new Random(1),out var none)&&none.Count==0,"insufficient keys");
            Assert(s.TreasureKeys==3&&s.Items.Count==0,"keys untouched");

            // 9. Audit repro: pity 80 + empty Legendary pool → no key loss, pity preserved.
            Reset();s=SaveSystem.Current;s.TreasureKeys=10;s.RollsSinceLegendary=80;SaveSystem.Save();
            Assert(!GameProgress.TryOpenChest(1,Db(false),new GameBalance(),new Random(1),out _),"missing tier aborts");
            Assert(s.TreasureKeys==10&&s.RollsSinceLegendary==80&&s.Items.Count==0,"pity and keys preserved");
            Fresh();Assert(SaveSystem.Current.TreasureKeys==10,"disk unchanged after abort");

            // 10. Chest success: debit and grants reach disk together.
            Reset();s=SaveSystem.Current;s.TreasureKeys=12;SaveSystem.Save();
            Assert(GameProgress.TryOpenChest(10,Db(true),new GameBalance(),new Random(7),out var got)&&got.Count==10,"open 10");
            Assert(got.Any(r=>r.Item.Tier>=Rank.Epic),"x10 floor still applied");
            Fresh();var disk=SaveSystem.Current;
            Assert(disk.TreasureKeys==2&&disk.Items.Sum(i=>i.Shards)==10,"debit + 10 shards persisted together");

            // 11. Level clear returns the exact grant; replay pays the replay rate.
            Reset();s=SaveSystem.Current;s.Gold=0;s.TreasureKeys=0;s.Gems=0;s.HighestLevelUnlocked=1;SaveSystem.Save();
            var first=GameProgress.Clear(1,new GameBalance());
            Assert(first.Saved&&first.FirstTime&&first.Gold==1000&&first.Keys==3&&first.Gems==80,"first clear reward");
            Fresh();Assert(SaveSystem.Current.Gold==1000&&SaveSystem.Current.HighestLevelUnlocked==2,"clear persisted");
            var again=GameProgress.Clear(1,new GameBalance());
            Assert(again.Saved&&!again.FirstTime&&again.Gold==400&&SaveSystem.Current.Gold==1400,"replay reward");

            // 12. Clear with disk failure grants nothing and reports it.
            UnityEngine.Application.persistentDataPath=Path.Combine(dir,"missing");
            var lost=GameProgress.Clear(2,new GameBalance());
            Assert(!lost.Saved&&lost.Gold==0&&SaveSystem.Current.Gold==1400&&SaveSystem.Current.HighestLevelUnlocked==2,"failed clear rolled back");

            Console.WriteLine("PASS: atomic replace keeps prev copy; recovery from .tmp/.prev; corrupt file archived; load normalization; transaction rollback on false/exception/disk failure; chest debit+grant+pity commit together (incl. audit empty-pool repro); level clear returns exact persisted reward.");
        }
        finally{Directory.Delete(dir,true);}
    }
}
