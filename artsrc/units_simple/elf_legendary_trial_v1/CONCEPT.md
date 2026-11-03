# ELF Legendary V1

Verdant Archer: blonde hair with simple braid, pointed ear, gold leaf headband with green gem, green tunic/cape, cream trousers brown boots, compact GOLD BOW with green tips. Anatomical LEFT far arm always HOLDS BOW forward/right; anatomical RIGHT near arm always DRAWS STRING back/left. Idle bow lowered, walk lifted foot; windup raised bow fully drawn right hand near cheek with single arrow; release left hand SAME bow held forward, right hand opens near cheek, no outgoing arrow on sheet; recovery lowered bow; hurt crouch same bow grip. Skill windup stronger fully drawn stance with ONE arrow nocked, same left bow/right draw; skill hit same grip bow held forward right hand released at cheek; skill recovery lowered bow. Skill multi-arrow effect added separately, don't draw multiple bows or extra arms. No face features.

Nine authored full silhouettes, dynamically cropped, preserving original antialiased alpha. Pose order: idle/walk/windup/strike/recovery/hurt/skill_windup/skill_hit/skill_recovery. Same common scale, feet-based pivots with forward weapon-tip exclusion; no forced square cells. Six demos: idle/walk/attack/skill/dead/full. Skill uses three dedicated drawn poses with procedural movement, not additional hand-drawn interpolation frames.

Equipment ownership visually reviewed: melee right near arm holds weapon; archer left far arm holds bow/right near arm draws string; robot right near fist attacks. Never mirror bodies. Effects separate and active only at release/contact. Demo-only, user approval pending; Unity untouched. Machine Epic remains deferred.

Reproduce: python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/elf_legendary_trial_v1 --profile verdant --skills --effects --expressive --strong-fx

