# Chapter 1 Slime enemies v1

## Installed roster

- Amber Slime: Common melee, body slam.
- Stone Slime: Common melee, stone-shell body slam.
- Green Slime: Common ranged, approved design and six-pose clips.
- Violet Slime: Common ranged, spitting glob.
- Bramble Slime: Elite melee, three back spines.
- Slime Overlord: Boss melee, crimson body and stone shell.

Built-in ImageGen used; transparent_background=true. Original generated PNG alpha preserved. No APK art in runtime assets. New five sheets are 4x4 (Idle/Walk/Attack/Dead), four poses per state. Approved Green uses four 3x2 sheets, six poses/state. Runtime source textures and layout JSON are under Assets/_Project/Resources/ChapterOneSlimes. tools/slime_layout.py measures connected silhouettes without changing images, including complete poses that cross nominal grid boundaries. Sprite.Create uses those explicit rectangles and ground pivots. No manual image editing.

## Integration

BattleSetup replaces only enemy definitions with private runtime clones, leaving allies, roster, save and scene assets untouched. Common identities rotate among existing spawn slots per wave; base counts, level 1-2 multiplier and extras are retained. A single Elite every third wave; a single Boss on final wave (Boss takes priority). Specials replace one enemy, never multiply with the 1-2 test multiplier. Other duplicated slots remain common. Fallback preserves original enemies if assets fail to load.

EnemyClass is independent of ally Rank. Common stats copied from existing melee/ranged enemy templates. Provisional Elite: HP x1.75, ATK x1.2, scale x1.15. Boss: HP x4, ATK x1.5, scale x1.5. These are initial test tuning, not final balancing. Ranged projectile uses a runtime Sprite region from approved green glob; violet tinted. Art source faces left; SourceFacesLeft handles world direction without touching existing ally playback.

Preview: http://127.0.0.1:8769/chapter1-enemies.html

## Prompt set

### amber

Use case: stylized-concept. Original 2D enemy game sprite sheet of Amber Slime. Reference role: exact approved green slime style/family, NOT recoloring APK. amber-orange, low wide dome, two small cream slanted eyes, no accessories. MELEE attacks by crouching then short body headbutt LEFT, NO projectile, NO mouth. Matte two-tone fill, one shadow patch, clean thick black outlines, flat simple mobile game art. No gloss, white spots, gradients, haze, cast shadows, text.
EXACTLY FOUR columns FOUR rows (16 equal SQUARE cells), square sheet. Same one creature repeated, all facing LEFT, same scale. Safe margins 18% in EVERY cell, no pose or projectile touching/crossing cell edges. Center x50%, baseline y80%, body max55% cell width. Fully transparent alpha=0 outside, opaque sprite inside.
Row1 four IDLE: neutral, subtle squash, neutral, subtle rise loop.
Row2 four WALK: squat, short hop, descend, landing. Do not translate across cells.
Row3 four ATTACK: windup, anticipation, short forceful body slam left keeping whole body inside safe margins, recover neutral.
Row4 four DEAD: recoil, slump, flatten, settled puddle, eyes close to dark slits, never X symbols.
No limbs, weapons, teeth, decorative details or other characters. Keep each cell visibly isolated.

### stone

Use case: stylized-concept. Original 2D enemy game sprite sheet of Stone Slime. Reference role: exact approved green slime style/family, NOT recoloring APK. slate-blue body with ONE simple broad gray stone plate over its upper back, two cream slanted eyes. MELEE attacks by squat windup then heavy body slam LEFT, NO projectile, NO mouth. Matte two-tone fill, one shadow patch, clean thick black outlines, flat simple mobile game art. No gloss, white spots, gradients, haze, cast shadows, text.
EXACTLY FOUR columns FOUR rows (16 equal SQUARE cells), square sheet. Same one creature repeated, all facing LEFT, same scale. Safe margins 18% in EVERY cell, no pose or projectile touching/crossing cell edges. Center x50%, baseline y80%, body max55% cell width. Fully transparent alpha=0 outside, opaque sprite inside.
Row1 four IDLE: neutral, subtle squash, neutral, subtle rise loop.
Row2 four WALK: squat, short hop, descend, landing. Do not translate across cells.
Row3 four ATTACK: windup, anticipation, short forceful body slam left keeping whole body inside safe margins, recover neutral.
Row4 four DEAD: recoil, slump, flatten, settled puddle, eyes close to dark slits, never X symbols.
No limbs, weapons, teeth, decorative details or other characters. Keep each cell visibly isolated.

### violet

Use case: stylized-concept. Original 2D enemy game sprite sheet of Violet Slime. Reference role: exact approved green slime style/family, NOT recoloring APK. violet-purple body with a slightly pointed teardrop top, two cream slanted eyes, NO accessories. RANGED attacks by puffing up then briefly opening a tiny round toothless mouth on left to spit one tiny purple glob, then recovers. NO teeth. Matte two-tone fill, one shadow patch, clean thick black outlines, flat simple mobile game art. No gloss, white spots, gradients, haze, cast shadows, text.
EXACTLY FOUR columns FOUR rows (16 equal SQUARE cells), square sheet. Same one creature repeated, all facing LEFT, same scale. Safe margins 18% in EVERY cell, no pose or projectile touching/crossing cell edges. Center x50%, baseline y80%, body max55% cell width. Fully transparent alpha=0 outside, opaque sprite inside.
Row1 four IDLE: neutral, subtle squash, neutral, subtle rise loop.
Row2 four WALK: squat, short hop, descend, landing. Do not translate across cells.
Row3 four ATTACK: windup, anticipation, puff then release glob left with tiny round toothless mouth, recover neutral.
Row4 four DEAD: recoil, slump, flatten, settled puddle, eyes close to dark slits, never X symbols.
No limbs, weapons, teeth, decorative details or other characters. Keep each cell visibly isolated.

### elite

Use case: stylized-concept. Original 2D game enemy sprite sheet Bramble Slime. Reference role: approved slime FAMILY style only. ELITE MELEE slime, deep teal green, slightly angular broad body, THREE short blunt dark green bramble spikes on upper back as its only distinguishing accessory, two cream-yellow slanted eyes. Attacks by heavy short body slam left, no projectile or mouth. Thick black outlines, flat two-tone colors, minimal details readable at small size. Keep identity identical through all frames.
EXACTLY FOUR columns FOUR rows, 16 equal SQUARE cells, square sheet, row-major. One whole creature per cell, same scale and LEFT-facing. Max body55% cell width, anchor centerx50%, feet y80%. At least18% fully transparent safety margin per cell. No cross-cell parts or effects.
Row1 four IDLE: neutral, slight squash, neutral, rise.
Row2 four WALK: crouch, short heavy hop, descend, land.
Row3 four ATTACK: squash windup, pull back, forceful short LEFT body slam staying inside cell, recover.
Row4 four DEAD: recoil, slump, flatten, settled puddle, eyes closed as slits not X. Shell/spikes collapse with body.
Opaque body on genuinely transparent background, no haze, gradients, gloss, particles, cast shadow, text or grid. No weapons, teeth, eyeballs, hands or feet.

### boss

Use case: stylized-concept. Original 2D game enemy sprite sheet Slime Overlord. Reference role: approved slime FAMILY style only. BOSS MELEE slime, deep crimson red body, large broad domed silhouette, TWO thick dark stone shoulder plates framing the back as a simple heavy shell, two narrow gold eyes. No crown or skull, no teeth or mouth, no limbs. Attacks by very low crouch windup then huge short body slam left. Original design, no resemblance to the APK king/queen. Thick black outlines, flat two-tone colors, minimal details readable at small size. Keep identity identical through all frames.
EXACTLY FOUR columns FOUR rows, 16 equal SQUARE cells, square sheet, row-major. One whole creature per cell, same scale and LEFT-facing. Max body55% cell width, anchor centerx50%, feet y80%. At least18% fully transparent safety margin per cell. No cross-cell parts or effects.
Row1 four IDLE: neutral, slight squash, neutral, rise.
Row2 four WALK: crouch, short heavy hop, descend, land.
Row3 four ATTACK: squash windup, pull back, forceful short LEFT body slam staying inside cell, recover.
Row4 four DEAD: recoil, slump, flatten, settled puddle, eyes closed as slits not X. Shell/spikes collapse with body.
Opaque body on genuinely transparent background, no haze, gradients, gloss, particles, cast shadow, text or grid. No weapons, teeth, eyeballs, hands or feet.

