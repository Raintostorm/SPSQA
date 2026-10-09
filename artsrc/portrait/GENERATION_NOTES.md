# Portrait backgrounds — generated candidates, NOT final layout approval

Request: `outdata/PROMPT_BG_PORTRAIT.md`.
References: `outdata/layout_portrait.png`, `artsrc/bg_prep_wide_ai.png`.
Built-in image generation used, then mechanical resize to 1242x2796.
No camera, balance, HUD or Assets changes.

Both outputs exist at requested paths. No grids or units painted into art.
Battle background was derived from prep to retain similar palette/horizon.
Prep needed several corrections because the generator did not obey exact
track coordinates. Latest candidate STILL needs alignment validation:
track rims extend into the requested lower army region. Do NOT mark these
assets layout-approved or import without checking against the actual grid.
The wide variations in grass colour also need evaluation beneath translucent
tiles. Matching appearance is approximate, not pixel-registered sky.

Expected track limits: y1188..1568, centre1378.
Expected empty army bands: x50..1192, y574..1188 and y1568..2182.
Requested side strips: 50px; check foreground silhouettes, not only trunks.

## Initial prep prompt
Create ONE finished portrait background A for a cartoon medieval strategy game. Output 1242 x 2796 pixels (exact width:height ratio 1242:2796, very tall portrait). Fully opaque artwork. Reference1 is layout only; its boxes, words, and red/blue boundaries MUST NOT be painted. Reference2 is STYLE reference only, do not reproduce its horizontal layout.
Style: polished flat cartoon game landscape, dark outlines only on perimeter scenery, saturated greens and warm ochre packed earth, soft shading, light upper left. Match existing background palette.
Absolute coordinate layout in target 1242x2796, top-down coordinates:
y0..294 overflow decoration only: pale sky and gentle blue distant mountains.
y294..574 top HUD area: extremely calm pale sky/mountains/remote treeline, horizon ends by y574. No tall sharp trees or high contrast there.
y574..1188 upper army placement area, x50..1192: completely FLAT EVEN unobstructed short grass and packed earth, uniform muted green-ochre tone. No rocks, objects, paths, shadows, grass tufts or sharp patches anywhere inside.
y1188..1568 EXACT horizontal dirt track, midpoint y1378, extends full width left edge to right edge. Straight parallel horizontal edges, no perspective vanishing point. Red SMALL banner pole at very left end, blue SMALL banner at very right end, confined entirely to x0..50 and x1192..1242, inside track band, never into army placement.
y1568..2182 lower army placement area, x50..1192: same calm flat uniform surface as upper army placement; no objects, trees, rocks, shadows, other paths.
y2182..2502 lower HUD area: calm ground without objects or contrast, subtly darker.
y2502..2796 overflow scenery only: calm grass, tiny scattered decorations allowed, not important.
Only very extreme left x0..50 and right x1192..1242 may carry any foreground bushes, fences, rocks or slim pines. Do not crowd the sides. Left and right decor must be DIFFERENT, never mirrored. No tree foliage overhanging x50..1192 in either army area. No large boulders or bushes inside playable space.
IMPORTANT: Do NOT draw ANY grids, square cells, tiles, platforms, placement markers, blue/red rectangles, schematic lines, words, characters, creatures, UI, text, numbers, watermark, border. Two plain unbroken flat expanses above/below one central HORIZONTAL track. Background should not compete with colorful sprites. Keep exact portrait ratio.

## Battle edit prompt
Edit this portrait background into its companion battle background B. Preserve IDENTICAL sky, mountains, distant treeline, horizon position, lighting and overall warm green/ochre palette. Same cartoon rendering, identical tall aspect ratio1242:2796. Output1242x2796 if possible. Fully opaque.
Change ONLY the central ground: REMOVE red and blue flags, remove narrow horizontal track's distinct grass rim boundaries. Replace the ground between 12.5% and87.5% of the image height with ONE broad seamless even expanse of packed warm ochre earth mixed with short grass. This central open ground must extend edge to edge, and must have no objects, tall foliage, rocks, tree shadows, contrasting pathways or deep grooves. Soldiers advance from TOP and BOTTOM, so no left-right corridor or perspective converging road. Keep ground horizontal/flat seen slightly overhead. Earth tone smooth and subdued, with only very faint even soil grain.
Keep visible sky/mountains/horizon exactly as reference: central battle ground begins naturally below horizon around17% height; do not replace sky with dirt. Top/bottom eighths remain calmer. Bottom ground may fade gently darker. Lower HUD y2182..2502 (78.04%..89.49%) remains quiet, flat unobstructed.
Foreground side scenery, if retained, is limited to extreme4% of width each side, particularly keep x50..1192 entirely free from foreground objects in y574..2182 of target1242x2796. Side strips must look different left/right, not mirrored.
No grid, squares, tiles, hexagons, platforms, markers, characters, creatures, UI, text, watermark, border. Do not shift horizon, change sky colors, add castles, zoom, crop or change art style. This is a registered companion background to be swapped in game with no scenery jump.

## Latest prep alignment prompt
Targeted geometry edit ONLY. This portrait background's broad horizontal dirt band is too HIGH. Move the ENTIRE broad horizontal track AND BOTH banner poles DOWN by exactly4.4% of total canvas height (123pixels on1242x2796;83pixels on836x1881). Do not move sky, mountains, horizon, trees, or any other scenery. Keep SAME band thickness and flat parallel edges. Fill the vacated strip ABOVE with matching grass/packed-earth ground; erase old boundary completely. The track center after moving must lie at49.3% of total height, approximately y1378 on2796px image. Upper edge approximately42.5%, lower edge56.1%. The band must NOT remain above image midpoint. Keep reference style and palette, no new details, no grid/text/UI. Exact same portrait aspect ratio.
