#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using Project.Data;
using Project.Meta;
using Project.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace Project.EditorTools
{
    public static class RelicUiChecks
    {
        static int frames;
        public static void Run()
        {
            var item=new TreasureItem{Id="test",Tier=Rank.Common,MaxLevel=10};
            var save=new SaveData{Gold=1000,AllyLevelResetRevision=1};
            save.Items.Add(new SaveData.OwnedItem{Id="test",Level=1,Shards=6});
            typeof(TreasureRoll).GetMethod("Grant",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{save,item});
            if(save.Items[0].Level!=1 || save.Items[0].Shards!=7)throw new Exception("Automatic level-up still enabled");
            if(!TreasureRoll.TryUpgrade(save,item)||save.Gold!=0||save.Items[0].Level!=2||save.Items[0].Shards!=5)throw new Exception("Upgrade transaction");
            if(TreasureRoll.TryUpgrade(save,item)||save.Items[0].Shards!=5)throw new Exception("Insufficient gold changed shards");
            save.Gold=10000;save.Items[0].Shards=0;
            if(TreasureRoll.TryUpgrade(save,item)||save.Gold!=10000)throw new Exception("Insufficient shards changed gold");
            SessionState.SetBool("relic-ui-check",true);
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/Treasure.unity");EditorApplication.EnterPlaymode();
        }
        [InitializeOnLoadMethod]static void Watch(){if(SessionState.GetBool("relic-ui-check",false))EditorApplication.update+=Check;}
        static void Check()
        {
            if(!EditorApplication.isPlaying||++frames<30)return;
            EditorApplication.update-=Check;SessionState.SetBool("relic-ui-check",false);
            try
            {
                var ui=UnityEngine.Object.FindFirstObjectByType<TreasureScreen>();
                if(!ui)throw new Exception("No TreasureScreen");
                var root=ui.transform.Find("RelicsReviewUI");
                if(!root || !root.Find("OpenAll"))throw new Exception("Missing new UI");
                typeof(TreasureScreen).GetMethod("ShowRelicDetail",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(ui,new object[]{0});
                if(!root.Find("RelicDetails"))throw new Exception("Missing detail modal");
                var tracks=root.GetComponentsInChildren<Image>().Where(im=>im.name=="ShardFill");
                if(!tracks.Any())throw new Exception("Missing shard progress");
                Debug.Log("[RelicUiChecks] PASS: no auto upgrade, gold/shard atomic spend, real Treasure scene UI and detail popup.");
                EditorApplication.Exit(0);
            }
            catch(Exception ex){Debug.LogException(ex);EditorApplication.Exit(1);}
        }
    }
}
#endif
