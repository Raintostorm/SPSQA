# Mythic v2 — full animation preview

Built-in imagegen original sheet: animations.png. Style references: v1 sprite sheet and existing human_legendary idle sprite. Prompt: same minimal faceless ivory/teal guardian, 4 frames each idle/walk/attack/skill/dead; obvious raise-and-plant sword cast, full collapsed death; no baked aura.

Browser demo uses read-only measured alpha bounds and runtime canvas cropping. No raster modification. VFX rendered separately: cast ground flash, double aura boundary, expanding pulse, orbiting marks and healing motes; UI effect name, remaining time, pulse count, HP and duration bars. These are preview UI, not final Unity assets.

Skill: 5 ticks after startup, .2 ATK per target each second, .5 actual total damage returned as HP capped to maximum. Five-second duration is provisional; mana/cooldown not decided. Manual controls plus automatic sequence. Death holds last pose.

No Unity/gameplay changes. URL http://127.0.0.1:8793/v2/
