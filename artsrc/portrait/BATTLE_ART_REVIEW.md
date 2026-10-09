# Battle portrait / stat panels: generated assets, pending integration review

Two independent orders from `outdata/PROMPT_BATTLE_PORTRAIT.md`.
Built-in image generation, no magenta removal. Mechanical resize only.
No gameplay, camera, spawn order, counters or HUD changes.

Files:
- `artsrc/portrait/bg_battle_portrait.png`,1242x2796.
- `artsrc/ui/stat_panels.png`,1400x900 RGBA, real antialiased alpha.

## Measured limitations (do not describe as pixel-exact approval)

Background grass rims near y633/y1941, requested700/1900. The ENTIRE
required y700..1900 span is dirt and clear edge-to-edge; actual road extends
beyond that span. Use conservative700..1900 until actual lane is approved.
No rocks/bushes/flags in playable road; sky/horizon is approximate reference
matching, not identical registered pixels. Slight soil texture remains.

UI four major components at x,y,width,height:
- swords29,56,441,209
- shield481,56,437,209
- leaf928,56,443,209
- panel36,301,1329,485

Requested tabs360x150 and panel1300x560 were NOT obeyed exactly by generator.
Tabs look similar but are not pixel-identical silhouettes. Must reconcile
size/shape before interchangeable production sprites. Wood rails thin;
metal corners need separate nine-slice measurement. Central interior sample
x200..1200,y400..700 RGB standard deviation under1 per channel, alpha min253:
visually plain dark fill, not literally an identical RGB255-alpha flat fill.
Outside alpha ranges0..255; no chroma key. Do not claim exact-size objects,
identical masks or production-ready9slice until independently validated.

## Background prompt
Create one fully opaque cartoon medieval game background, tall portrait ratio1242:2796, output1242x2796. Reference1 is GEOMETRY only: do not paint its arrows, text, blocks or outlines. Reference2 is STYLE and sky/horizon reference only: its narrow road must NOT be copied.
Target coordinates from top:
y0..294 decoration overflow pale sky/distant scenery.
y294..700 calm HUD area with pale sky, hazy blue mountains, remote treeline then short grass. Keep same sky/mountain palette and horizon as style reference. Low contrast.
y700..1900 must be one HUGE HORIZONTAL ROAD, height1200px, occupying43% of entire image. Upper boundary at25.04% of full height, lower boundary67.95%. This broad road runs UNBROKEN from extreme LEFT edge to extreme RIGHT edge; armies travel horizontally across it in four rows. It is NOT a narrow path, NOT a vertical road, NOT a perspective corridor. Fullwidth plain warm ochre packed dirt. Surface smooth, flat and evenly colored across entire width, almost solid earthy tone with faint uniform grain only. No green islands, patches, rocks, cracks, grooves, footprints, grass tufts, bushes, puddles, props, shadows or contrasting streaks ANYWHERE inside this road band, including its left/right ends.
Road edges gently undulating with low grass fringe positioned OUTSIDE the playable road band, above700 and below1900.
y1900..2502 calm grass gradually darker, later hidden by game panels.
y2502..2796 overflow ground.
Side pines/rocks/fences only in extreme4% left/right width and ONLY ABOVE y700 or BELOW y1900. Absolutely no scenery intrudes road ends. Different left/right sides, not mirrored. No flags on road. Flat cartoon, thick dark outline on scenery, saturated greens, warm ochre earth, soft shading, upperleft light. Match original art.
No characters, creatures, UI, text, numbers, arrows, grids, square tiles, markers, watermark or border. Accurate broad road geometry is more important than decoration.

## UI prompt
One production game UI sprite sheet,1400x900 landscape ratio14:9. TRUE transparent alpha background, smoothly antialiased edges. Reference is MATERIAL/STYLE only, IGNORE its magenta background completely.
FOUR separate objects, none touching, no shadows outside:
Top row THREE IDENTICAL rounded rectangular TABS, each360wide x150high, same outer geometry/corner radius/wood iron brackets exactly; only emblem/color differ. Place approximately x80..440,520..880,960..1320 at y50..200. Equal size and silhouette.
Tab1 centered two crossed swords, orange-red active accent.
Tab2 centered round shield, steel-blue active accent.
Tab3 centered leaf, green active accent.
All three active/bright. Emblems inside tabs, no glow extending outside silhouette. No extra icons or words.
Bottom one PANEL FRAME exactly1300wide x560high approximately x50..1350 y290..850. Rectangular wood frame with thin grey iron corner brackets and small rivets like reference. Border maximum40pixels at target size, preferably28px, INCLUDING all corner fittings; don't put enormous metal corner ornaments over interior.
The entire recessed INTERIOR is one perfectly FLAT SOLID dark warm brown colour, completely EMPTY and EVEN. NO woodgrain, light band, gradients, highlights, dark vignette, diagonal streaks, texture, engraving, inner subdivisions or pattern. This interior is for four rows of future text/bars; no lines drawn. Thin border leaves huge usable interior.
Flat polished cartoon style, bold dark outlines, warm golden wood, grey iron corner details, upperleft soft shading ONLY on frame and tab materials. Don't copy reference's very thick endcaps. All objects fully inside image, generous transparent spacing.
No magenta, no filled background, no text/numbers, no UI content other than three tab emblems, no watermark, no outside shadow. Exactly3tabs+1panel.

## UI corrective prompt
Precise production UI correction to attached sheet. Keep four objects:3active tabs upperrow and1panel below, true transparent alpha outside. Match existing art.
MANDATORY: replace ENTIRE panel interior with ONE OPAQUE SOLID dark brown fill #30231B, same RGB/alpha255 everywhere. No vignette, no gradients, no sheen, no grain, no mottling, no transparent holes, no ghost patterns. Interior is boring empty flat dark rectangular surface for text. Only FRAME edges have shading.
THIN the panel frame, including iron corner fittings, to at most40px inward from each outer edge at1400x900 output; prefer28px wood rail. Corner iron brackets must fit within that border and not bulge into usable interior. Plain rounded inner corners. No huge endcaps.
Canvas1400x900. Panel approx x50..1350 y290..850 (1300x560). Tabs exactly same geometry and dimensions360x150, positions x80..440,520..880,960..1320 at y50..200. Reduce tabs if needed for these sizes. Same silhouette, corner radius and metal bracket shape in all three, only emblem/accent colors differ. Tab emblems crossed swords orange-red, roundshield blue, leafgreen.
No text, number, extra icons, magenta, filled backdrop, watermark, or outside shadows. Preserve smooth anti-aliased true alpha around objects.
