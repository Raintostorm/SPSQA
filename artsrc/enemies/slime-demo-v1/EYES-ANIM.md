# Approved eyes slime animation draft

Built-in ImageGen, transparent_background=true, source alpha preserved. Six key poses per state, 3x2 sheets, one per animation. Original PNGs eyes-idle.png / eyes-walk.png / eyes-attack.png / eyes-dead.png. Preview eyes.html. Original approved design used as identity reference, no APK art input. Attack direction is inflate/spit; not production combat/projectile integration. Unequal pose timing in preview, no Unity changes.

## Prompt set

### idle

Use case: stylized-concept. Asset type: 2D game idle sprite sheet. Reference image role: EXACT approved slime identity. Preserve grass green body, one dark lower-right patch, two pale-yellow slanted eyes with black surrounds, asymmetrical squat silhouette, black outline. Original SpinSquadAgain enemy, not another game's art. No new accessories, limbs, teeth, white gloss or texture. Flat opaque simple colors.
STRICT layout EXACTLY 3 columns by 2 rows, six equal SQUARE cells, read left-to-right top-to-bottom. Landscape 3:2 sheet. All poses of same ONE character. Same scale, anchored center x50%, feet baseline y80%. Body occupies at most 50% cell width and 48% cell height. At least 20% transparent padding on all sides; even effects may NEVER touch or cross cell boundaries.
Six IDLE breathing poses: neutral, slight squash, neutral, slight rise, neutral, same neutral. No mouth in any pose. Small subtle changes only, feet baseline stable.
Transparent background alpha=0 outside sprite; fully opaque inside. NO haze, glow, cast shadow, backdrop, grid lines or text. No unrelated shapes. Keep every complete silhouette comfortably within its own cell to avoid neighbor-frame contamination.

### walk

Use case: stylized-concept. Asset type: 2D game walk sprite sheet. Reference image role: EXACT approved slime identity. Preserve grass green body, one dark lower-right patch, two pale-yellow slanted eyes with black surrounds, asymmetrical squat silhouette, black outline. Original SpinSquadAgain enemy, not another game's art. No new accessories, limbs, teeth, white gloss or texture. Flat opaque simple colors.
STRICT layout EXACTLY 3 columns by 2 rows, six equal SQUARE cells, read left-to-right top-to-bottom. Landscape 3:2 sheet. All poses of same ONE character. Same scale, anchored center x50%, feet baseline y80%. Body occupies at most 50% cell width and 48% cell height. At least 20% transparent padding on all sides; even effects may NEVER touch or cross cell boundaries.
Six WALK hop poses: squat, push off, short hop upwards, descend tilted slightly LEFT, soft landing squash, recover neutral. No mouth in any pose. Movement within cell only, no horizontal traveling.
Transparent background alpha=0 outside sprite; fully opaque inside. NO haze, glow, cast shadow, backdrop, grid lines or text. No unrelated shapes. Keep every complete silhouette comfortably within its own cell to avoid neighbor-frame contamination.

### attack

Use case: stylized-concept. Asset type: 2D game attack sprite sheet. Reference image role: EXACT approved slime identity. Preserve grass green body, one dark lower-right patch, two pale-yellow slanted eyes with black surrounds, asymmetrical squat silhouette, black outline. Original SpinSquadAgain enemy, not another game's art. No new accessories, limbs, teeth, white gloss or texture. Flat opaque simple colors.
STRICT layout EXACTLY 3 columns by 2 rows, six equal SQUARE cells, read left-to-right top-to-bottom. Landscape 3:2 sheet. All poses of same ONE character. Same scale, anchored center x50%, feet baseline y80%. Body occupies at most 50% cell width and 48% cell height. At least 20% transparent padding on all sides; even effects may NEVER touch or cross cell boundaries.
Six ATTACK poses: neutral, puff body and hold air with eyes narrowing, taller inflated anticipation, suddenly open a small round toothless black mouth on LEFT and spit a SHORT green slime glob, mouth closes as glob separates left, recover original idle. NO TEETH, NO FANGS, NO TONGUE. Mouth only in release pose. Glob must stay completely within same cell with safe blank margins, body not stretched far across cell. Clear anticipation then fast release.
Transparent background alpha=0 outside sprite; fully opaque inside. NO haze, glow, cast shadow, backdrop, grid lines or text. No unrelated shapes. Keep every complete silhouette comfortably within its own cell to avoid neighbor-frame contamination.

### dead

Use case: stylized-concept. Asset type: 2D game dead sprite sheet. Reference image role: EXACT approved slime identity. Preserve grass green body, one dark lower-right patch, two pale-yellow slanted eyes with black surrounds, asymmetrical squat silhouette, black outline. Original SpinSquadAgain enemy, not another game's art. No new accessories, limbs, teeth, white gloss or texture. Flat opaque simple colors.
STRICT layout EXACTLY 3 columns by 2 rows, six equal SQUARE cells, read left-to-right top-to-bottom. Landscape 3:2 sheet. All poses of same ONE character. Same scale, anchored center x50%, feet baseline y80%. Body occupies at most 50% cell width and 48% cell height. At least 20% transparent padding on all sides; even effects may NEVER touch or cross cell boundaries.
Six DEAD poses: hit recoil, droops right, collapses lower, flattened puddle, settles, same settled puddle. Eyes dim to darker cream then close as short slits, NO X-eyes, NO mouth. Preserve body mass and green colors; not disappearing.
Transparent background alpha=0 outside sprite; fully opaque inside. NO haze, glow, cast shadow, backdrop, grid lines or text. No unrelated shapes. Keep every complete silhouette comfortably within its own cell to avoid neighbor-frame contamination.

