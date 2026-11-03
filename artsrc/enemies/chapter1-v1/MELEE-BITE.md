# Melee toothless bite update

Built-in ImageGen; transparent_background=true. Original generated PNG alpha unchanged. New source sheets: amber-bite-v2.png, stone-bite-v2.png, elite-bite-v2.png, boss-bite-v2.png. Runtime resources use corresponding *_bite.png and layout JSON; only Attack clips switch to these. Idle/Walk/Dead source assets and ranged attacks unchanged. Six poses: neutral, opening, gape, snap, recoil, recovery. No teeth/tongue/spit.

Attack timing retains original 0.85s total (0.20 / 0.20 / 0.12 / 0.08 / 0.10 / 0.15); damage step 3 is snap, not open-mouth windup. Source-sheet pixel scale normalized against original atlas cell height. Full silhouette rectangles and grounded pivots measured without editing image data.

## Prompts

### amber

Use case: stylized-concept. Create a replacement ATTACK animation sheet for the EXACT amber slime in input. Reference role: identity and palette reference only; keep original eye shape, body colors, outline and shell/spikes if present. Do NOT depict other states.
SIX bite attack poses, exactly THREE columns TWO rows in a 3:2 landscape sheet, equal square cells read row-major.
1 neutral body, tiny closed mouth seam LEFT.
2 crouch windup, mouth begins opening LEFT.
3 body pulls back, a very large black mouth gapes LEFT clearly beneath the eyes.
4 strong short LEFT lunge, smooth upper and lower edges of mouth snap almost shut in a clear toothless BITE.
5 recoil, mouth closes.
6 recover neutral.
Mouth is only a simple solid black cavity. ABSOLUTELY NO TEETH, FANGS, TONGUE, GUMS OR WHITE MOUTH DETAILS in ANY pose. Keep two original pale eyes distinct above mouth. No projectile, drool, beam or spit. Flat cartoon simple thick black outline.
Same body scale all cells, center x50%, ground y80%. Complete body maximum55% cell width. At least18% transparent padding each side, no crossing cell boundary. Opaque clean body on transparent background, no haze/glow/shadow, no grid/text. Preserve exact character, do not add accessories or extra spikes.

### stone

Use case: stylized-concept. Create a replacement ATTACK animation sheet for the EXACT stone slime in input. Reference role: identity and palette reference only; keep original eye shape, body colors, outline and shell/spikes if present. Do NOT depict other states.
SIX bite attack poses, exactly THREE columns TWO rows in a 3:2 landscape sheet, equal square cells read row-major.
1 neutral body, tiny closed mouth seam LEFT.
2 crouch windup, mouth begins opening LEFT.
3 body pulls back, a very large black mouth gapes LEFT clearly beneath the eyes.
4 strong short LEFT lunge, smooth upper and lower edges of mouth snap almost shut in a clear toothless BITE.
5 recoil, mouth closes.
6 recover neutral.
Mouth is only a simple solid black cavity. ABSOLUTELY NO TEETH, FANGS, TONGUE, GUMS OR WHITE MOUTH DETAILS in ANY pose. Keep two original pale eyes distinct above mouth. No projectile, drool, beam or spit. Flat cartoon simple thick black outline.
Same body scale all cells, center x50%, ground y80%. Complete body maximum55% cell width. At least18% transparent padding each side, no crossing cell boundary. Opaque clean body on transparent background, no haze/glow/shadow, no grid/text. Preserve exact character, do not add accessories or extra spikes.

### elite

Use case: stylized-concept. Create a replacement ATTACK animation sheet for the EXACT elite slime in input. Reference role: identity and palette reference only; keep original eye shape, body colors, outline and shell/spikes if present. Do NOT depict other states.
SIX bite attack poses, exactly THREE columns TWO rows in a 3:2 landscape sheet, equal square cells read row-major.
1 neutral body, tiny closed mouth seam LEFT.
2 crouch windup, mouth begins opening LEFT.
3 body pulls back, a very large black mouth gapes LEFT clearly beneath the eyes.
4 strong short LEFT lunge, smooth upper and lower edges of mouth snap almost shut in a clear toothless BITE.
5 recoil, mouth closes.
6 recover neutral.
Mouth is only a simple solid black cavity. ABSOLUTELY NO TEETH, FANGS, TONGUE, GUMS OR WHITE MOUTH DETAILS in ANY pose. Keep two original pale eyes distinct above mouth. No projectile, drool, beam or spit. Flat cartoon simple thick black outline.
Same body scale all cells, center x50%, ground y80%. Complete body maximum55% cell width. At least18% transparent padding each side, no crossing cell boundary. Opaque clean body on transparent background, no haze/glow/shadow, no grid/text. Preserve exact character, do not add accessories or extra spikes.

### boss

Use case: stylized-concept. Create a replacement ATTACK animation sheet for the EXACT boss slime in input. Reference role: identity and palette reference only; keep original eye shape, body colors, outline and shell/spikes if present. Do NOT depict other states.
SIX bite attack poses, exactly THREE columns TWO rows in a 3:2 landscape sheet, equal square cells read row-major.
1 neutral body, tiny closed mouth seam LEFT.
2 crouch windup, mouth begins opening LEFT.
3 body pulls back, a very large black mouth gapes LEFT clearly beneath the eyes.
4 strong short LEFT lunge, smooth upper and lower edges of mouth snap almost shut in a clear toothless BITE.
5 recoil, mouth closes.
6 recover neutral.
Mouth is only a simple solid black cavity. ABSOLUTELY NO TEETH, FANGS, TONGUE, GUMS OR WHITE MOUTH DETAILS in ANY pose. Keep two original pale eyes distinct above mouth. No projectile, drool, beam or spit. Flat cartoon simple thick black outline.
Same body scale all cells, center x50%, ground y80%. Complete body maximum55% cell width. At least18% transparent padding each side, no crossing cell boundary. Opaque clean body on transparent background, no haze/glow/shadow, no grid/text. Preserve exact character, do not add accessories or extra spikes.

