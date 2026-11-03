using System;
using Project.Meta;
class MonsterDiscoveryTests
{
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    static void Main()
    {
        var save=new SaveData{Gems=17,EncounteredMonsters=null};
        Check(!MonsterDiscovery.Seen(save,"enemy_slime_amber"),"old save should be undiscovered");
        Check(MonsterDiscovery.Encounter(save,"enemy_slime_amber")==5&&save.Gems==22,"common grant");
        for(int i=0;i<50;i++)Check(MonsterDiscovery.Encounter(save,"enemy_slime_amber")==0,"repeat/restart must not grant");
        Check(MonsterDiscovery.Encounter(save,"enemy_slime_elite")==10,"elite grant");
        Check(MonsterDiscovery.Encounter(save,"enemy_slime_boss")==20,"boss grant");
        Check(MonsterDiscovery.Encounter(save,"human_common")==0,"allies excluded");
        Check(MonsterDiscovery.Encounter(save,null)==0,"null ID excluded");
        var loaded=new SaveData{Gems=save.Gems,EncounteredMonsters=new System.Collections.Generic.List<string>(save.EncounteredMonsters)};
        Check(MonsterDiscovery.Encounter(loaded,"enemy_slime_boss")==0&&loaded.Gems==52,"restored discovery grants once");
        Check(loaded.EncounteredMonsters.Count==3,"unique IDs");
        Console.WriteLine("PASS: common/elite/boss rewards, legacy null list, duplicates, restored progress, unknown/ally IDs.");
    }
}
