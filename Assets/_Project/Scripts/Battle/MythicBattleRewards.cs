using Project.Gacha;
using UnityEngine;
namespace Project.Battle
{
    // Owned by the battle, so a Mimic dying cannot discard an earned payout.
    public sealed class MythicBattleRewards:MonoBehaviour
    {
        BattleFlow flow;readonly DeferredWaveCoins coins=new();
        public static void Add(BattleFlow flow,int amount)
        {if(!flow||amount<=0)return;var ledger=flow.GetComponent<MythicBattleRewards>();if(!ledger)ledger=flow.gameObject.AddComponent<MythicBattleRewards>();ledger.coins.Earn(flow.Wave,amount);}
        void Awake(){flow=GetComponent<BattleFlow>();flow.PhaseChanged+=Phase;flow.RunRestarted+=Reset;}
        void OnDestroy(){if(flow){flow.PhaseChanged-=Phase;flow.RunRestarted-=Reset;}}
        void Reset()=>coins.Reset();
        void Phase(BattleFlow.Phase p)
        {
            if(p==BattleFlow.Phase.Prep){int amount=coins.Claim(flow.Wave);if(amount>0)FindFirstObjectByType<RollService>()?.Run?.AddCoins(amount);}
            if(p==BattleFlow.Phase.Defeated||p==BattleFlow.Phase.LevelCleared)coins.Reset();
        }
    }
}
