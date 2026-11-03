# Legendary skills and roll presentation — 2026-10-12

Implemented in runtime scripts (no shared UnitDefinition modifications).

- Successful basic hit: +20 mana, capped at 100. Melee at impact; ranged at projectile arrival. Skill damage has `basicAttack=false` and never generates mana. Cast automatically after the previous attack finishes and an enemy target exists.
- Blue mana bar under unit HP, Legendary only, hidden in prep/queue/death.
- Elf: actual Skill clip, upward shot, cyan arrow rain, AoE damage then freeze 2s after arrows land.
- Human: actual Skill clip, gold shockwave, AoE damage + 0.6s visual knock-up. Existing ground bounds still constrain the complete sprite.
- Machine: shields living allied melee units, each shield = 50% of recipient MaxHp; expires after 8 game seconds or absorbed. Refresh replaces; no stacking. Bubble disappears when shield breaks, expires or unit dies.
- Dead: 3 Dead Common runtime units, each HP and attack snapshot = caster / 3. Movement speed = caster / 3, attack rate = caster / 3 (cooldown ×3). Not added to board, evolution or save. Removed at end of wave.
- Monster: living teammates +30% attack and attack speed for 5 game seconds; refresh, never stacking. Red/orange roar effect.
- AoE demo defaults: radius 3.5 world units, damage 2× attack, configurable on LegendaryCombat. These are starting balance values, not final tuning.
- Roll: pay once, reveal reel, commit coins/buffs exactly once, deliver recruit light trail to board cell then spawn. Battle start is locked during reveal/delivery, including Space shortcut.
- Coins and buff increases count in green, pause, then restore normal color. Coins also rain into the counter. Presentation animations respect settings pause and use unscaled time; combat uses scaled game time.

Verification: Roslyn compile clean; Unity Play Mode audit 22 PASS (outdata/LEGEND_COMBAT_AUDIT.txt). Also manually rolled in CharacterLab: ally count stayed 3 during reel and became 4 after delivery, zero Console errors. Existing audio warnings remain unrelated.

Inline demo is an interactive visual simulation using existing character sprites, not a recording of a Unity match. Actual game VFX are procedural LineRenderers; no APK art copied.
