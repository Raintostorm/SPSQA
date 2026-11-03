# Green Slime bite attack v1

Built-in ImageGen; transparent_background=true; original generated PNG alpha preserved. Six attack poses in a 3x2 sheet: bite-sheet-v1.png. Original files retained. faceless.html uses clearRect before every drawImage and does not use original overflowing attack cells.

Original defect: attack pose crosses x=768 grid boundary; opaque pixels touch both adjacent cell edges, producing clipped silhouettes and neighboring fragments. Old CSS renderer did not accumulate frames.

## Prompt

Use case: stylized-concept. Asset type: original 2D game sprite animation ATTACK sheet.
Reference image role: design reference for the green slime only. Keep same matte green two-tone body, very thick black outline, simple asymmetrical squat blob. NO EYES at all. NO leaves, limbs, accessories, shine or highlights.
Make a bite attack: ONLY a black mouth opening towards LEFT, with two simple cream triangular teeth, no lips or tongue. Mouth closed at rest as a small black seam, opens wide for bite then closes.
STRICT LAYOUT: exactly THREE columns and TWO rows, SIX equal square cells, read row-major. Entire image landscape 3:2. Within EVERY cell draw one complete slime silhouette centered at x50%, bottom baseline y78%. Maximum width 60% of cell, maximum height 55%. At least 20% EMPTY TRANSPARENT padding on both left/right of EACH cell. Nothing may touch or cross a cell edge. No neighboring fragments. All six poses same scale and stable anchor.
Frame1: neutral squat body, small closed mouth seam at left.
Frame2: crouches down, mouth just starting to open.
Frame3: body pulls slightly backward right, mouth wide open facing left, only two simple teeth.
Frame4: bite snap lunges LEFT within safe margin, jaws clamp closed; visible simple tooth, short rounded body not stretched across cell.
Frame5: recoils toward center, mouth closes.
Frame6: exact same resting silhouette as frame1.
Background: genuinely transparent alpha=0, no green haze or glow, no cast shadow, no texture, no text, no grid, no background scene. Flat clean opaque body fill and thick opaque black contour. Keep each entire pose fully inside its cell.

