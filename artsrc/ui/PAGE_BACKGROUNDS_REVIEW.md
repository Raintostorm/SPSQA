# Page backgrounds — 2026-10-09

Generated using built-in imagegen from `outdata/PROMPT_PAGE_BACKGROUNDS.md`.
Home uses `PROMPT_HOMEPAGE.md` section3/A with its quiet-band line replaced
by y600..2280, as requested by the newer document. Numeric bands take
precedence over contradictory older "bottom quarter" placement language.

All saved outputs are RGB (no alpha),1125×2436, fullbleed. Native generated
portraits exported with sips to the exact requested dimensions. No UI,
labels, characters, frame or watermark. No game-code/Unity import changes.

| File | Quiet zone | RGB-average brightness mean /255 | Std /255 | Mean adjacent horizontal difference |
|---|---|---:|---:|---:|
| bg_upgrade_portrait.png | y400..2436 |40.52|0.49|0.227|
| bg_shop_portrait.png | y400..2436 |72.08|0.57|0.234|
| bg_home_portrait.png | y600..2280 |89.16|6.52|0.391|

Upgrade/Shop: detailed headers condensed above the quiet-zone boundary.
Lower areas are visually uniform; Shop brighter than Upgrade as required.
Final regeneration requested thinner top headers to leave a safety margin
before y400. Both pass the major quiet-band layout check.

Home: castle, distant hills and camp preserve outdoor depth. Central field
was simplified and bright flags removed from the middle. Residual grass
variation and low trees/tent decoration near the bottom still intrude into
the strict y600..2280 band; not marked as fully layout-compliant. Requires
review with actual UI overlay before approval. The numeric statistics alone
do not certify all salient-object placement or shared-game style matching.

Prompts otherwise preserve the stated places and cartoon palette. Only
targeted follow-ups altered decorative placement and quiet-field contrast.
