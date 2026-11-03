using System.Collections.Generic;
namespace Project.Meta
{
    // Stable definition IDs, independent of spawn count, stage, death or resurrection.
    public static class MonsterDiscovery
    {
        public static int Reward(string id)=>id switch
        {
            "enemy_slime_amber" or "enemy_slime_stone" or "enemy_slime_green" or "enemy_slime_violet"=>5,
            "enemy_slime_elite"=>10,"enemy_slime_boss"=>20,_=>0
        };
        public static bool Seen(SaveData save,string id)=>save?.EncounteredMonsters?.Contains(id)==true;
        public static int Encounter(SaveData save,string id)
        {
            int amount=Reward(id);
            if(save==null||amount==0||Seen(save,id))return 0;
            save.EncounteredMonsters??=new List<string>();
            save.EncounteredMonsters.Add(id);save.Gems+=amount;return amount;
        }
    }
}
