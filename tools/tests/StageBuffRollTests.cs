using System;
using System.Linq;
using Project.Gacha;
using Project.Data;
namespace Project.Data
{
    public enum Rank{Common,Rare,Epic,Legendary,Mythic}
    public enum Race{Human,Elf,Dead,Monster,Machine}
    public sealed class GameBalance
    {
        public int[] ClusterRewardCoin={1,2,3,4},ClusterRewardHP={10,20,30,40},ClusterRewardDamage={10,20,30,40},ClusterRewardAtkSpeed={10,20,30,40},ClusterRewardCritChance={1,2,3,4};
    }
}
class StageBuffRollTests
{
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Main()
    {
        Check(StageRollPreview.OpeningChoices(1,1).Length==0&&StageRollPreview.OpeningChoices(2,1).Length==0,"opening preview only stage 3");
        Check(StageRollPreview.OpeningChoices(3,2).Length==0,"no opening choices in later waves");
        var opening=StageRollPreview.OpeningChoices(3,1);
        Check(opening.SequenceEqual(new[]{Rank.Epic,Rank.Legendary,Rank.Epic,Rank.Legendary,Rank.Epic,Rank.Legendary}),"six mixed opening choices");
        var balance=new GameBalance();var cells=StageRollPreview.FirstEpicBuff(3,0);
        Check(cells.Length==6&&cells.Count(c=>c==RollCellKind.Buff)==5,"five buff cells");
        var epic=RollResolver.Resolve(cells,balance,new Random(2));
        Check(epic.BuffAdds.Count==1&&epic.BuffAdds[0].Rank==Rank.Epic&&!epic.Jackpot,"first roll is Epic, not Legendary");
        var legendary=RollResolver.Resolve(Enumerable.Repeat(RollCellKind.Buff,6).ToArray(),balance,new Random(2));
        Check(legendary.BuffAdds[0].Rank==Rank.Legendary&&legendary.BuffAdds[0].Value>epic.BuffAdds[0].Value,"distinct Legendary tier/value");
        Check(StageRollPreview.FirstEpicBuff(3,1)==null,"only first success");
        Check(StageRollPreview.FirstEpicBuff(1,0)==null&&StageRollPreview.FirstEpicBuff(2,0)==null,"other stages unchanged");
        Check(StageRollPreview.FirstEpicBuff(3,0)!=cells,"fresh cells on new run");
        Console.WriteLine("PASS: Chapter 1-3 first-success Epic override, later/other stages unchanged, distinct Epic/Legendary rank and values.");
    }
}
