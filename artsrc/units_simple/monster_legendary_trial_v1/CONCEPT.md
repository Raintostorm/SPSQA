# MONSTER Legendary V1

Lion Warlord: cute biped orange lion, large round orange mane, plain tan face with ONLY a little black triangular nose (NO eyes NO mouth), gold shoulder armor boots, simple red cape, short brown mace with DARK GREY round head and THREE blunt spikes. Anatomical RIGHT NEAR arm under prominent gold shoulder plate on screen-left/front ALWAYS holds mace. Idle mace forward low near hip; walk leg raised same grip; windup mace raised in right near arm over shoulder, left empty; hit RIGHT gold shoulder-to-cream forearm-to-mace grip clearly continuous across FRONT torso, heavy mace swings down-right, left arm empty behind; recovery same right grip low, hurt kneeling mace kept right. Skill windup legs braced head tilts UP a little same facing and camera, mace stays low RIGHT hand; skill hit ROAR, only this pose has small open black mouth, head tilted up and empty LEFT fist raised, mace still right hand LOW no swap; skill recovery mouth closes and returns idle. No shockwave drawn sheet.

Nine authored full silhouettes, dynamically cropped, preserving original antialiased alpha. Pose order: idle/walk/windup/strike/recovery/hurt/skill_windup/skill_hit/skill_recovery. Same common scale, feet-based pivots with forward weapon-tip exclusion; no forced square cells. Six demos: idle/walk/attack/skill/dead/full. Skill uses three dedicated drawn poses with procedural movement, not additional hand-drawn interpolation frames.

Equipment ownership visually reviewed: melee right near arm holds weapon; archer left far arm holds bow/right near arm draws string; robot right near fist attacks. Never mirror bodies. Effects separate and active only at release/contact. Demo-only, user approval pending; Unity untouched. Machine Epic remains deferred.

Reproduce: python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/monster_legendary_trial_v1 --profile lion --skills --effects --expressive --strong-fx

