# DEAD Legendary V1

Skeleton King: oversized rounded cream skull BLANK without facial features, three-prong gold crown with one purple inset, purple cape with cream collar, simple gold shoulder plates, compact cream bony limbs, grey boots, short silver straight sword gold hilt. Anatomical RIGHT near arm under visible gold shoulder plate at screen-left/front ALWAYS holds sword. Idle sword forward low near hip, walk leg raised same grip; windup right arm sword overhead, left empty; hit right shoulder connects visibly across FRONT of chest to sword-gripping hand slashing down-right, left empty behind; recovery sword back near hip, hurt crouched same right grip. Skill windup raise SAME right sword upright above head; skill hit plant right-hand sword tip down at front-right while EMPTY left hand raised casting, same shoulder/arm connection; skill recovery return sword hip, left empty. Skull stays blank like approved faceless units. No separate floating spell in sheet.

Nine authored full silhouettes, dynamically cropped, preserving original antialiased alpha. Pose order: idle/walk/windup/strike/recovery/hurt/skill_windup/skill_hit/skill_recovery. Same common scale, feet-based pivots with forward weapon-tip exclusion; no forced square cells. Six demos: idle/walk/attack/skill/dead/full. Skill uses three dedicated drawn poses with procedural movement, not additional hand-drawn interpolation frames.

Equipment ownership visually reviewed: melee right near arm holds weapon; archer left far arm holds bow/right near arm draws string; robot right near fist attacks. Never mirror bodies. Effects separate and active only at release/contact. Demo-only, user approval pending; Unity untouched. Machine Epic remains deferred.

Reproduce: python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/dead_legendary_trial_v1 --profile king --skills --effects --expressive --strong-fx

