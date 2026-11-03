# Battle controls v1
Approved preview: proposal.png. Built-in ImageGen extraction produced the unmodified transparent Resources/BattleControls/atlas.png. Six runtime sprites cut at exact alpha-island bounds (slices.json); backgrounds use 9-slice, icons preserve aspect. Source alpha was inspected read-only; no raster editing or alpha reconstruction.

## Prompt
Extract exactly six isolated assets from approved board on genuine transparent 1536x1024 atlas: cyan die with yellow curved arrows; crossed silver swords/gold handles; gold crown coin; empty blue rounded Roll background; empty gold Battle background; empty navy ally frame. No text/digits/characters/separators/mana bars/backdrop, no outer glow/shadow. Flat bold outlined 2D chibi style, blank stretchable centers, generous gutters.

## Implementation
BattleHud keeps actual RollCost and existing enable/disable rules; separate generated icons/backgrounds and runtime labels. No purchase logic or combat mechanics changed.
StatPanel keeps scene/builder serialization compatibility and shows a navy live ranking, one row per registered ally, values/share/bar using total across all registered allies. Full ranking is vertically scrollable; tabs select Dealt/Taken. It remains visible only in Marching/Fighting, preserving preparation field space. Removed/dead-unit run totals remain in Contribution modal.
BattleAllyStrip is read-only, horizontally scrollable, uses real portraits or idle-frame fallback. Normal units show actual HP, mana units actual MP, current casts show CAST. There is no fictitious timer: existing automatic skills use mana, not timed cooldowns. No generated demo character art replaces production allies.

## Verification
105 source files compile clean. Transparent atlas has six isolated components with non-overlapping bounds. Live Unity preparation and fighting inspected; portrait fallback/alignment fixed and rechecked. live-prep.png and live-damage.png capture actual game. Dealt total and share visible with real damage. Final tab switching and large-roster drag/device-touch coverage remain pending; tool session timed out during final tab interaction. No Roll/Shop/Summon purchase used for validation.
