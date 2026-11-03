# Upgrade stat icons v1

Generated using the built-in ImageGen tool with transparent_background=true. Original PNG alpha is preserved, with no pixel retouching. Runtime copies live in Assets/_Project/Resources/StatIcons. Unity import caps textures at 256px, with no mipmaps, preserving source PNGs at full resolution.

## Prompts and generated sources

### hp

/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482/exec-dd37df5b-c44f-4d4c-b740-cec5fdd7f5a7.png

Use case: stylized-concept. Asset type: transparent stat icon for a simple flat 2D cartoon mobile army game. Draw a single pink heart for health, one broad lighter pink highlight. One isolated large centered icon, square canvas, generous 15% transparent margin, clean fully transparent background. Very thick near-black outline, flat saturated color blocks, just one broad highlight, strong silhouette clearly readable at 40 pixels. Consistent simple game icon family. No tile, no frame, no badge background, no letters or words (only specified percent/exclamation symbol where requested), no realism, no metallic textures, no glow, no drop shadow outside icon, no extra objects.

### mp

/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482/exec-59782bc1-5328-48b4-aa85-6a5a079eb97a.png

Use case: stylized-concept. Asset type: transparent stat icon for a simple flat 2D cartoon mobile army game. Draw a single blue water droplet for mana, one broad pale cyan highlight. One isolated large centered icon, square canvas, generous 15% transparent margin, clean fully transparent background. Very thick near-black outline, flat saturated color blocks, just one broad highlight, strong silhouette clearly readable at 40 pixels. Consistent simple game icon family. No tile, no frame, no badge background, no letters or words (only specified percent/exclamation symbol where requested), no realism, no metallic textures, no glow, no drop shadow outside icon, no extra objects.

### atk

/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482/exec-a31d2fe2-a48e-4691-b584-91800db40ea4.png

Use case: stylized-concept. Asset type: transparent stat icon for a simple flat 2D cartoon mobile army game. Draw a single short red sword tilted diagonally, dark grip, broad red blade. One isolated large centered icon, square canvas, generous 15% transparent margin, clean fully transparent background. Very thick near-black outline, flat saturated color blocks, just one broad highlight, strong silhouette clearly readable at 40 pixels. Consistent simple game icon family. No tile, no frame, no badge background, no letters or words (only specified percent/exclamation symbol where requested), no realism, no metallic textures, no glow, no drop shadow outside icon, no extra objects.

### speed

/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482/exec-812549eb-f36c-4e0f-880b-85fbf3022b31.png

Use case: stylized-concept. Asset type: transparent stat icon for a simple flat 2D cartoon mobile army game. Draw a single light-blue feather-shaped fast arrow with TWO short motion streaks, for attack speed. One isolated large centered icon, square canvas, generous 15% transparent margin, clean fully transparent background. Very thick near-black outline, flat saturated color blocks, just one broad highlight, strong silhouette clearly readable at 40 pixels. Consistent simple game icon family. No tile, no frame, no badge background, no letters or words (only specified percent/exclamation symbol where requested), no realism, no metallic textures, no glow, no drop shadow outside icon, no extra objects.

### crit_chance

/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482/exec-9bc46831-9df5-49b6-b2ae-31b9ca586345.png

Use case: stylized-concept. Asset type: transparent stat icon for a simple flat 2D cartoon mobile army game. Draw a single orange four-point impact star containing a bold dark percent symbol, for critical chance. One isolated large centered icon, square canvas, generous 15% transparent margin, clean fully transparent background. Very thick near-black outline, flat saturated color blocks, just one broad highlight, strong silhouette clearly readable at 40 pixels. Consistent simple game icon family. No tile, no frame, no badge background, no letters or words (only specified percent/exclamation symbol where requested), no realism, no metallic textures, no glow, no drop shadow outside icon, no extra objects.

### crit_damage

/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482/exec-a1c3775f-956f-4fd4-92c1-ea4c8830e445.png

Use case: stylized-concept. Asset type: transparent stat icon for a simple flat 2D cartoon mobile army game. Draw a single golden angular impact burst containing one dark exclamation mark, for critical damage. One isolated large centered icon, square canvas, generous 15% transparent margin, clean fully transparent background. Very thick near-black outline, flat saturated color blocks, just one broad highlight, strong silhouette clearly readable at 40 pixels. Consistent simple game icon family. No tile, no frame, no badge background, no letters or words (only specified percent/exclamation symbol where requested), no realism, no metallic textures, no glow, no drop shadow outside icon, no extra objects.

### defense

/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482/exec-86dac928-3c55-4fc2-a2d0-fc9c0faa14de.png

Use case: stylized-concept. Asset type: transparent stat icon for a simple flat 2D cartoon mobile army game. Draw a single cyan shield with simple dark inner shield outline, for damage reduction. One isolated large centered icon, square canvas, generous 15% transparent margin, clean fully transparent background. Very thick near-black outline, flat saturated color blocks, just one broad highlight, strong silhouette clearly readable at 40 pixels. Consistent simple game icon family. No tile, no frame, no badge background, no letters or words (only specified percent/exclamation symbol where requested), no realism, no metallic textures, no glow, no drop shadow outside icon, no extra objects.

### range

/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482/exec-ec940ce8-0fa4-4e66-a6f9-e211b028fc56.png

Use case: stylized-concept. Asset type: transparent stat icon for a simple flat 2D cartoon mobile army game. Draw a single yellow bullseye with exactly three thick concentric rings and central dot, for attack range. One isolated large centered icon, square canvas, generous 15% transparent margin, clean fully transparent background. Very thick near-black outline, flat saturated color blocks, just one broad highlight, strong silhouette clearly readable at 40 pixels. Consistent simple game icon family. No tile, no frame, no badge background, no letters or words (only specified percent/exclamation symbol where requested), no realism, no metallic textures, no glow, no drop shadow outside icon, no extra objects.

## Machine Epic audit (no unit changes in this task)

Anim_machine_epic.asset still references Art/Units/machine_epic for all four states. Machine Common, Rare and Legendary reference Art/UnitsSimple instead. The more detailed look is in the source art, not a UI shader or scale defect. artsrc/units_simple/machine_epic_trial_v2 contains an uninstalled trial; replacing all runtime clips requires separate validation. No unit definition, animation binding or battle stats were changed here.
