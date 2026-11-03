# Mythic v4 — integrated sword animation

Inspected human_legendary skill_000, skill_021 and dead_027, plus FrameUnitVisual.Apply/PlaySkill: complete character sprites include sword; VFX separate; BakedMotion true.

Regenerated Skill and Dead via built-in imagegen with those references and existing Mythic identity. Original generated skill.png/dead.png preserved. No procedural weapon, no weaponless character sheet, no separate weapon layer. The sword is included in every Skill/Dead source pose, including final corpse. Body/sword consistency remains an art-review concern, not enforced by a detached runtime weapon.

Runtime crops whole character with measured alpha boxes inside manually checked grid regions, fixed scale per sheet and foot anchoring. All five states retained, frame stepping retained, aura demonstration unchanged. No Unity integration yet.

Preview http://127.0.0.1:8793/v4/
