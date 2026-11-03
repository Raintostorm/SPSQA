using System;
using System.Linq;
using System.Collections.Generic;
using Project.Battle;
using Project.Data;
namespace UnityEngine {
    public class Object {public static implicit operator bool(Object x)=>x!=null;}
    public enum RuntimeInitializeLoadType { SubsystemRegistration }
    public class RuntimeInitializeOnLoadMethodAttribute:Attribute {public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t){}}
}
namespace Project.Data {
    public enum UnitTeam { Ally,Enemy }
    public enum Rank { Common,Rare,Epic,Legendary }
    public class UnitDefinition:UnityEngine.Object { public Rank Rank; }
}
namespace Project.Gacha {
    public class Grant { public Rank Rank; }
    public class RollPayout { public List<Grant> AllyGrants=new List<Grant>(); }
}
namespace Project.Battle {
    public class Unit:UnityEngine.Object {public UnitTeam Team;public UnitDefinition Definition;public int Id;public int GetInstanceID()=>Id;}
}
static class ContributionTests {
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Main(){
        BattleContribution.Reset();
        var def=new UnitDefinition { Rank=Rank.Legendary };
        var ally=new Unit {Team=UnitTeam.Ally,Definition=def,Id=1};
        var enemy=new Unit {Team=UnitTeam.Enemy,Definition=def,Id=2};
        BattleContribution.Damage(ally,30,false);BattleContribution.Damage(ally,12,true);BattleContribution.Damage(enemy,99,false);BattleContribution.Damage(ally,-1,false);
        Check(BattleContribution.Rows.Count()==1 && BattleContribution.Rows.Single().Dealt==30 && BattleContribution.Rows.Single().Taken==12,"ally-only real damage");
        ally=null;Check(BattleContribution.Rows.Single().Dealt==30,"historical row survives removed unit");
        var payout=new Project.Gacha.RollPayout();payout.AllyGrants.Add(new Project.Gacha.Grant {Rank=Rank.Rare});payout.AllyGrants.Add(new Project.Gacha.Grant {Rank=Rank.Rare});BattleContribution.Roll(payout);
        Check(BattleContribution.Rolls==1 && BattleContribution.Rarity[1]==2,"roll count differs from reward count");
        BattleContribution.Recruited=2;BattleContribution.Merges=1;BattleContribution.SkillCasts=3;BattleContribution.Summons=3;BattleContribution.Reset();
        Check(!BattleContribution.Rows.Any() && BattleContribution.Rarity.Sum()==0 && BattleContribution.Rolls+BattleContribution.Recruited+BattleContribution.Merges+BattleContribution.SkillCasts+BattleContribution.Summons==0,"clean run reset");
        Console.WriteLine("PASS: allied real-damage ledger, retained historical rows, roll/reward counts and clean run reset.");
    }
}
