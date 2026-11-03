using Project.Data;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    public static class MythicPresentation
    {
        public static string SkillName(MythicKind kind)=>kind switch
        {
            MythicKind.Guardian=>"Dawn Aura",MythicKind.Undead=>"Grave Pact · Passive",MythicKind.Elf=>"Moonwood Blessing",
            MythicKind.Bear=>"Thunder Hammer",MythicKind.Robot=>"Aegis Field",MythicKind.Clock=>"Time Stop",MythicKind.Mimic=>"Devour",_=>"Passing Slash"
        };
        public static string Description(MythicKind kind)=>kind switch
        {
            MythicKind.Guardian=>"Move and attack while the aura follows you. Each second, deal 20% ATK to nearby enemies and heal for 50% of actual aura damage. Drains 15 MP/s; no mana gain while active.",
            MythicKind.Undead=>"Enemy Common and Elite units return as temporary allies. Bosses cannot return. Each ally revives once per battle at 50% HP, gaining a skull seal; its next death is final. Multiple Wardens do not add revivals.",
            MythicKind.Elf=>"All living allies gain +20% ATK and +20% attack speed for 8s. Recasting refreshes the duration; bonuses do not stack.",
            MythicKind.Bear=>"Empower the hammer for 6s. Attacks chain lightning to up to 2 additional enemies, each taking 50% ATK. Each chain strikes a target only once.",
            MythicKind.Robot=>"Shield nearby allies for 25% of each recipient's maximum HP for 6s. Recasting refreshes the shield; shields do not stack.",
            MythicKind.Clock=>"Create a field at the target for 5s. Enemies inside cannot move or attack. Allies are unaffected; enemies outside the field act normally.",
            MythicKind.Mimic=>"Devour a nearby enemy for 300% ATK and heal for 50% of actual damage. Bosses take damage but cannot be pulled. Each resolved cast grants 5 battle coins at the next wave.",
            _=>"Dash through the target and strike enemies along the path for 200% ATK. Each enemy can be hit only once per cast."
        };
    }
    // Constant pixels per world unit and a fixed foot anchor prevent pose-size jitter.
    public sealed class MythicIdleImage:MonoBehaviour
    {
        Image image;UnitAnimationSet.Clip clip;int step;float elapsed;float pixelsPerUnit=220;
        public void Bind(Image target,UnitAnimationSet.Clip idle,float previewPixelsPerUnit=220){image=target;clip=idle;pixelsPerUnit=previewPixelsPerUnit;Draw();}
        void Update()
        {
            if(clip==null||!clip.IsValid||SharedSettings.IsPaused)return;
            elapsed+=Time.unscaledDeltaTime;while(elapsed>=clip.DurationAt(step)){elapsed-=clip.DurationAt(step);step=(step+1)%clip.Count;}Draw();
        }
        void Draw()
        {
            if(!image||clip==null||!clip.IsValid)return;var sprite=clip.FrameAt(step);if(!sprite)return;
            image.sprite=sprite;image.rectTransform.pivot=new Vector2(sprite.pivot.x/sprite.rect.width,sprite.pivot.y/sprite.rect.height);
            image.rectTransform.sizeDelta=sprite.rect.size/sprite.pixelsPerUnit*pixelsPerUnit;image.raycastTarget=false;
        }
    }
}
