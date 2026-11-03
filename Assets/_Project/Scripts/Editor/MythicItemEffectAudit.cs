#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Project.Battle;
using Project.Data;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;
namespace Project.EditorTools
{
    public static class MythicItemEffectAudit
    {
        [MenuItem("SpinSquad/Diagnostics/Audit completed item effects")]
        static void Run()
        {
            if(!Application.isPlaying||Object.FindFirstObjectByType<BattleFlow>()){Debug.LogWarning("Run item effect audit in Play Mode outside battle.");return;}
            var root=new GameObject("Item effect audit");var spawner=root.AddComponent<UnitSpawner>();
            typeof(UnitSpawner).GetField("unitPrefab",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(spawner,AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Units/Unit.prefab"));
            spawner.StartCoroutine(Check(spawner,root));
        }
        static IEnumerator Check(UnitSpawner spawner,GameObject root)
        {
            RunBuffs.Reset();var log=new List<string>();
            void Test(bool ok,string label){log.Add((ok?"PASS ":"FAIL ")+label);}
            Unit Spawn(UnitTeam team,float x,float hp=10000){var u=spawner.Spawn(MythicCatalog.Find(MythicKind.Guardian),team,new Vector3(x,0),root.transform);u.SetSummonedStats(hp,100);u.GetComponent<UnitBrain>().enabled=false;return u;}
            var heart=Spawn(UnitTeam.Ally,100);yield return null;
            heart.Equipment.Add(4);Test(Mathf.Approximately(heart.Mythic.Mana,60),"Mana Heart boosts starting 40 MP to 60");
            heart.Mythic.BasicHit();Test(Mathf.Approximately(heart.Mythic.Mana,90),"Mana Heart boosts landed hit 20 MP to 30");
            heart.Mythic.ReceiveBasicHit();Test(Mathf.Approximately(heart.Mythic.Mana,96),"Mana Heart boosts received hit 4 MP to 6");
            heart.Mythic.GainMana(100);Test(heart.Mythic.Mana==100,"Mana gain remains capped at 100");
            var twin=Spawn(UnitTeam.Ally,101);twin.Equipment.Add(2);Test(Mathf.Approximately(1+twin.Equipment.SpeedBonus,1.5f),"Twin Fang attack speed x1.5");
            var runic=twin;runic.transform.position=new Vector3(110,0);runic.Equipment.Add(3);var victim=Spawn(UnitTeam.Enemy,110.3f,1);var next=Spawn(UnitTeam.Enemy,110.5f);
            yield return null;victim.TakeDamage(10000,runic,false);yield return null;yield return null;
            Test(Mathf.Approximately(next.DamageTaken,20),"Runic kill slashes nearest surviving enemy for 20 percent ATK");
            next.SetSummonedStats(1,100);var untouched=Spawn(UnitTeam.Enemy,110.8f);yield return null;
            var victim2=Spawn(UnitTeam.Enemy,110.4f,1);yield return null;victim2.TakeDamage(10000,runic,false);yield return null;yield return null;yield return null;
            Test(!next.IsAlive&&untouched.DamageTaken==0,"Secondary slash cannot recursively proc itself");
            Debug.Log("[ItemEffectAudit]\n"+string.Join("\n",log));Object.Destroy(root);
        }
    }
}
#endif
