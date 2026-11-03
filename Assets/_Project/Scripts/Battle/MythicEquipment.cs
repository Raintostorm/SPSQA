using System.Collections.Generic;
using System.Collections;
using UnityEngine;
namespace Project.Battle
{
    /// <summary>Derived bonuses preserve component values when basic items are combined.</summary>
    public sealed class MythicEquipment:MonoBehaviour
    {
        readonly List<int> types=new();public IReadOnlyList<int> Types=>types;
        readonly List<SpriteRenderer> badges=new();Unit unit;
        public float SpeedBonus{get;private set;}public float StartingMana{get;private set;}
        public float ManaGainMultiplier{get;private set;}=1;
        bool slashRunning,resolvingSlash;readonly Queue<Vector3> kills=new();
        public void Add(int type)
        {
            types.Add(type);
            switch(type){case 0:SpeedBonus+=.1f;break;case 1:StartingMana+=20;break;case 2:SpeedBonus+=.5f;break;case 3:SpeedBonus+=.1f;StartingMana+=20;break;case 4:StartingMana+=40;ManaGainMultiplier+=.5f;break;}
            GetComponent<MythicCombat>()?.RefreshStartingMana();
        }
        public void OnKill(Unit victim)
        {
            if(resolvingSlash||!types.Contains(3)||!victim)return;
            if(!unit)unit=GetComponent<Unit>();
            if(!unit.IsAlive||victim.Team==unit.Team)return;
            kills.Enqueue(victim.transform.position);if(!slashRunning)StartCoroutine(SlashKills());
        }
        IEnumerator SlashKills()
        {
            slashRunning=true;
            while(kills.Count>0)
            {
                var point=kills.Dequeue();yield return null;
                if(!unit||!unit.IsAlive)break;
                var flow=FindFirstObjectByType<BattleFlow>();if(flow&&flow.Current!=BattleFlow.Phase.Fighting&&flow.Current!=BattleFlow.Phase.Marching)break;
                Unit target=null;float distance=float.MaxValue;
                foreach(var enemy in UnitRegistry.All)
                {if(!enemy||!enemy.IsAlive||enemy.Team==unit.Team||!enemy.Mythic.Deployed)continue;float d=(enemy.transform.position-point).sqrMagnitude;if(d<distance){distance=d;target=enemy;}}
                if(target){Project.Visual.MythicEffect.Play("slash",unit,target.HitPoint,.7f,.25f);resolvingSlash=true;try{target.TakeDamage(unit.AttackPower*.2f,unit,false);}finally{resolvingSlash=false;}}
            }
            kills.Clear();slashRunning=false;
        }
        void OnDisable(){StopAllCoroutines();kills.Clear();slashRunning=resolvingSlash=false;}
        void LateUpdate()
        {
            if(!unit)unit=GetComponent<Unit>();
            while(badges.Count<types.Count)
            {
                var badge=new GameObject("Equipped "+Project.Data.BattleItemRecipes.Items[types[badges.Count]].Name).AddComponent<SpriteRenderer>();
                badge.transform.SetParent(transform,false);badge.sprite=Project.Data.BattleItemRecipes.Icon(types[badges.Count]);badges.Add(badge);
            }
            var body=GetComponent<Project.Visual.FrameUnitVisual>()?.BodyRenderer;
            for(int i=0;i<badges.Count;i++)
            {
                var badge=badges[i];badge.enabled=unit.IsAlive&&(!body||body.enabled);if(!badge.enabled||!badge.sprite)continue;
                badge.sortingLayerID=body?body.sortingLayerID:0;badge.sortingOrder=(body?body.sortingOrder:0)+30;
                badge.transform.position=transform.position+new Vector3((i%3-(Mathf.Min(types.Count,3)-1)*.5f)*.27f,-.20f-(i/3)*.23f,0);
                float size=.25f/Mathf.Max(badge.sprite.bounds.size.x,badge.sprite.bounds.size.y);var parent=transform.lossyScale;
                badge.transform.localScale=new Vector3(size/Mathf.Max(.001f,Mathf.Abs(parent.x)),size/Mathf.Max(.001f,Mathf.Abs(parent.y)),1);
            }
        }
    }
}
