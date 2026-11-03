using Project.Visual;
using UnityEngine;
namespace Project.Battle
{
    // Presentation only, once at the existing attack callback. Never applies damage.
    public static class MeleeAttackEffect
    {
        static BattleFlow battle;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()=>battle=null;
        public static RangedImpactKind Kind(string id,bool empowered)
        {
            switch(id)
            {
                case "human_common":case "dead_common":case "mythic_swordsman":return RangedImpactKind.Slash;
                case "human_rare":return RangedImpactKind.Pierce;
                case "human_legendary":return RangedImpactKind.Royal;
                case "elf_rare":return RangedImpactKind.Leaf;
                case "dead_legendary":return RangedImpactKind.Skeleton;
                case "machine_common":return RangedImpactKind.Saw;
                case "machine_epic":return RangedImpactKind.Fire;
                case "machine_legendary":case "mythic_robot":return RangedImpactKind.Smash;
                case "monster_rare":return RangedImpactKind.Claw;
                case "monster_epic":return RangedImpactKind.Sting;
                case "monster_legendary":return RangedImpactKind.Heavy;
                case "mythic_undead":return RangedImpactKind.DarkSlash;
                case "mythic_bear":return empowered?RangedImpactKind.None:RangedImpactKind.Heavy;
                case "mythic_mimic":case "enemy_slime_amber":case "enemy_slime_stone":case "enemy_slime_elite":case "enemy_slime_boss":return RangedImpactKind.Bite;
                default:return RangedImpactKind.None; // Guardian already has a baked slash.
            }
        }
        public static void Play(Unit owner,bool empowered)
        {
            if(!owner||!owner.Definition||owner.Definition.IsRanged)return;
            var kind=Kind(owner.Definition.UnitId,empowered);if(kind==RangedImpactKind.None)return;
            var body=owner.GetComponent<FrameUnitVisual>()?.BodyRenderer;if(!body)return;
            bool left=(owner.transform.lossyScale.x<0)^(owner.Definition.Animations&&owner.Definition.Animations.SourceFacesLeft);
            float width=body.bounds.size.x;bool blunt=kind==RangedImpactKind.Smash||kind==RangedImpactKind.Heavy;
            float factor=kind==RangedImpactKind.Bite?1f:blunt?1.8f:1.5f;
            var point=owner.HitPoint+Vector3.right*(left?-1:1)*width*.55f;
            var group=owner.GetComponent<UnityEngine.Rendering.SortingGroup>();
            if(!battle)battle=Object.FindFirstObjectByType<BattleFlow>();
            RangedImpact.Play(kind,point,width*factor,group?group.sortingLayerID:body.sortingLayerID,(group?group.sortingOrder:body.sortingOrder)+5,battle,blunt?.48f:.4f,left);
        }
    }
}
