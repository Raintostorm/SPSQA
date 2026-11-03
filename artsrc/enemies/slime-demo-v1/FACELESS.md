# Faceless Green Slime animation draft

Built-in ImageGen; transparent_background=true; original alpha preserved. Reference: approved faceless UI concept. No runtime asset changes. Six poses per state, rows Idle / Walk / Attack / Dead. Browser review in faceless.html. This is a key-pose draft, not final production interpolation.

## Prompt

Use case: stylized-concept. Generate a transparent 2D game animation sprite sheet of ONLY the faceless green slime shown in the reference UI screenshot. Reference role: exact slime design/style reference only, exclude UI and scenery. Same squat asymmetrical grass green blob, thick black outline, single flat dark green lower-right shadow patch. NO eyes, mouth, face, leaf, accessories, limbs, highlights, white spots or translucent rendering. Flat opaque two-tone cartoon. EXACTLY 6 columns by 4 rows of equal cells, 24 frames total. No text, no grid, no background or cast shadow. Same scale and anchor in each cell: ground baseline at 80% cell height, character centered horizontally. Leave generous safe padding; body about 55% cell width. Row1 six IDLE breathing frames: subtle squat/expand/return loop. Row2 six WALK frames: squash, push off, short hop, descend, squash landing, recover; no horizontal translation. Row3 six MELEE ATTACK frames: neutral, crouch windup, pull backwards right, fast body slam stretching left, compress impact, recover. Row4 six DEAD frames: struck recoil right, tilt, slump, flatten, settle puddle, same settled puddle. Dying not disappearing and no facial marks. Keep matte two-tone coloring and chunky simple outline consistent with reference. Uniform 6x4 grid, no cropping, no other subjects.

## Transparency cleanup prompt

Edit target: supplied 6 columns x4 rows sprite sheet. Remove ALL dark green haze, glow, backdrop, shadow, translucent fringe and every pixel outside the black outline of each slime, replacing with genuinely fully transparent alpha=0. Keep all 24 slime poses, black outlines, two flat green fill colors, cell positions and sizes EXACTLY unchanged. Interior body opaque. No gradients, no effects, no halo. Clean hard cartoon sprite cutouts on true transparency. Do not add background color, grid, text or checkerboard. Keep 1536x1024 6x4 layout.

Selected output: faceless-sheet-clean.png. Initial generated faceless-sheet.png kept for provenance, not used by preview.
