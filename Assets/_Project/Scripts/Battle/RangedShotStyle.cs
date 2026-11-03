using Project.Data;
using Project.Visual;
using UnityEngine;
namespace Project.Battle
{
    // Explicit approved families. Unknown/new units get no guessed impact.
    public static class RangedShotStyle
    {
        public static RangedImpactKind Kind(UnitDefinition def)
        {
            if(!def)return RangedImpactKind.None;
            switch(def.UnitId)
            {
                case "elf_common":case "elf_legendary":return RangedImpactKind.Arrow;
                case "elf_epic":case "human_epic":return RangedImpactKind.Magic;
                case "machine_rare":return RangedImpactKind.Machine;
                case "dead_rare":case "dead_epic":return RangedImpactKind.Spirit;
                case "monster_common":return RangedImpactKind.Web;
                case "mythic_elf":return RangedImpactKind.Elf;
                case "mythic_clock":return RangedImpactKind.Clock;
                case "enemy_slime_green":return RangedImpactKind.GreenSlime;
                case "enemy_slime_violet":return RangedImpactKind.VioletSlime;
                default:return RangedImpactKind.None;
            }
        }
        public static Vector3 Origin(Unit unit,Transform overrideAnchor)
        {
            if(overrideAnchor)return overrideAnchor.position;
            var body=unit.GetComponent<FrameUnitVisual>()?.BodyRenderer;if(!body)return unit.HitPoint;
            var bounds=body.bounds;float x=.9f,y=.55f;
            switch(unit.Definition.UnitId)
            {
                case "elf_common":case "elf_epic":case "elf_legendary":x=.94f;y=.52f;break;
                case "human_epic":x=.84f;y=.78f;break;
                case "machine_rare":x=.98f;y=.49f;break;
                case "dead_epic":x=.95f;y=.47f;break;
                case "monster_common":x=.9f;y=.44f;break;
            }
            if(unit.transform.lossyScale.x<0)x=1-x;
            return new Vector3(Mathf.Lerp(bounds.min.x,bounds.max.x,x),Mathf.Lerp(bounds.min.y,bounds.max.y,y),unit.transform.position.z);
        }
        public static bool Shielded(Unit unit)=>unit&&(unit.Effects&&unit.Effects.Shield>0||unit.Combat&&unit.Combat.Shield>0||unit.Mythic&&unit.Mythic.Shield>0);
        public static Vector3 Contact(Unit unit,Vector3 incoming)
        {
            var centre=unit.HitPoint;if(!Shielded(unit))return centre;
            var body=unit.GetComponent<FrameUnitVisual>()?.BodyRenderer;
            float radius=body?Mathf.Clamp(body.bounds.size.x*.55f,.16f,.7f):.3f;
            var direction=incoming-centre;if(direction.sqrMagnitude<.00001f)direction=Vector3.left;
            return centre+direction.normalized*radius;
        }
    }
}
