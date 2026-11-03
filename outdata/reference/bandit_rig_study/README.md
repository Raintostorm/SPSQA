# Bandit 1005 — local APK study

Source: user-supplied Spin+Squad! 2.5.1 APK. Extracted commercial sprites are reference-only, not imported into production Assets.

`bandit_model.json` includes the original transform hierarchy, sprite metadata (120×120 source rectangles, pivot 0.5/0, PPU 100), and four source clips.

`bandit_full_rig_v2.gif` replays idle, walk, attack, skill and dead at 30 sampled frames/second. It includes source cubic scale/position/rotation curves and the separate Weapon_slash branch. The ground and labels are preview scaffolding. Root movement/combat AI are not reconstructed, and the APK has not been run directly.

V1 was incorrect: streamed `curveCount` excludes `discreteCurveCount`. The old decoder treated sprite-change channels as constant transforms, leaving idle art during walk/attack. V2 includes discrete channels and asserts exact source sprite selection at key times.

Confirmed through prefab AnimatorOverrideController `anim_player_unit_1005`: walk/attack are the corresponding `1005_bandit_*` clips, not the UI clips. Base controller uses speed 1, idle/walk loop; attack/skill/dead do not. Spawn is overridden by this Bandit's idle clip, so it has no separate spawn drawing.

- Idle: 0.5167 seconds, 3 distinct body sprites.
- Walk: 1.0167 seconds, idle pose + 1 walk sprite, plus continuous transforms.
- Attack: 1.5167 seconds, idle + 3 attack body sprites; slash sprites on separate branch.
- Dead: 0.8333 seconds, idle/reaction/dead sprite with falling transform; preview holds final pose.
- Skill: 0.4333 seconds, three skill drawings plus source transform curves.

The rendered GIF has more sampled frames than source drawings. These are interpolated transform samples, not additional drawn poses.

Regenerate: `python3 tools/render_bandit_rig_study.py`.
