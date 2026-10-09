# Treasure art review — 2026-10-09

Built-in imagegen used, source brief: `outdata/PROMPT_TREASURE_UI.md`.
Only art generated; no SaveSystem, inventory icons, UI code or Unity import.
No alpha thresholding, magenta removal or fixed-cell cropping performed.

## Saved results

- `bg_treasure_portrait.png`: exported 1125×2436 with sips. Opaque.
  Indoor stone vault; decoration stops above the requested quiet bands.
  RGB-channel-average brightness in y1060..1210: mean39.77/255,
  std0.74; y1260..2436: mean39.67, std0.84. Quiet dark zones pass.
- `buttons_roll.png`: 2172×724, six complete buttons with 256 alpha
  levels, including true zero. Copper normal/pressed, silver-blue
  normal/pressed, gold normal/pressed. Visible widths320..322px;
  visible height approximately178px, not requested150px. Gaps33..35px,
  outer horizontal margins38/37px: below requested80px and40px.
  Paired pixel-identical geometry and inverted bevel not certified.
  Faces still have mild illumination gradients. Candidate, not passed.
- `item_slots.png`: 2172×724, five separate complete silhouettes,
  256 alpha levels. X-spans103..402,522..821,938..1240,1355..1659,
  1773..2073; gaps120/117/115/114px. No touching glow clouds.
  Sizes299..304px rather than180px, shared border/inner opening still
  need exact calibration. Legendary gained purple gems and lacks
  requested strong external gold rays; this is a deviation, not approved.
- `side_pillar.png`: 1161×1355 canvas with one narrow pillar, visible
  x501..661. Vertical endpoints touch canvas, side background transparent,
  256 alpha levels. Not exported to requested120×1400 art size.
  Top-bottom seam RGBA MAE19.93/255 across visible union; six columns
  differ in alpha>20 occupancy. Seamless vertical tiling NOT passed.

Alpha>20 was used for measurements ONLY; original antialiased alpha was
preserved. 256 levels alone do not prove one-pixel antialias transition.

## Prompt adaptations

Background: confined all detail to upper43%, explicitly kept full-width
lower area plain dark; no chest or UI baked into background.
Buttons: six left-to-right states, blank faces and matched geometry;
targeted follow-up requested wider2.13:1 shapes and larger gaps.
Slots: targeted separation fix replaced wide touching glow clouds with
thin lit borders, kept decoration within shared outline and large gaps.
That prevented merged silhouettes but reduced the brief's glow/rays.
Pillar: prioritised vertical endpoint continuation over conflicting
general top/bottom40px margin. No capitals or pedestal requested.

Only background currently passes the measured major layout requirements.
Other files are review candidates; do not label the batch import-ready.
