# Item icon sheets — 2026-10-09

Built-in imagegen; four sheets generated sequentially using the individual
prompt blocks in `outdata/PROMPT_ITEM_ICONS.md`. Common was simplified to
bold smooth cartoon shading. Later sheets use Common as a style-only
reference. Final follow-up preserved identities/order and shrank each
complete item around its centre to increase margins and separation.

No code/Unity import, index assignment, cell cropping or alpha thresholding.
PNG outputs copied intact. All 30 identities/order visually inspected.

| Sheet | Native size | Major alpha>20 components | Alpha levels | Visible margins L/T/R/B |
|---|---|---:|---:|---|
| items_common.png |1536×1024|12|255|135/95/120/117|
| items_rare.png |1536×1024|8|255|107/137/125/177|
| items_epic.png |1536×1024|6|255|144/98/101/112|
| items_legendary.png |2172×724|4|256|113/146/127/149|

All sheets have genuine zero-alpha background. Major silhouettes are
separated, complete, in the specified 3×4 / 2×4 / 2×3 / 1×4 layouts.
Visible bounding boxes at alpha>20 have >80px gaps between neighbouring
items. This threshold was ONLY diagnostic; output alpha was not changed.

## Import warnings — strict constraints not fully passed

At alpha>0, low-opacity residual tails/noise extend beyond the visible
silhouettes. True nonzero-alpha outer margins L/T/R/B:

- Common:25/29/14/18px;12 components>=1000px and1 extra64..999px.
- Rare:33/67/62/25px;8 components>=1000px and2 extra64..999px.
- Epic:0/0/10/0px;8 components>=1000px and21 extra64..999px.
- Legendary:0/17/14/0px;4 components>=1000px and15 extra64..999px.

Thus strict >=60px fully transparent outer margins are NOT certified.
Do not import via naive alpha>0 component counting or discard faint
pixels silently. Epic smoke and detached sparkles need association with
the intended item; stop if importer count differs. Use row grouping
before column sorting: varying item heights mean global top-Y sorting
would scramble the intended order.

Having many alpha levels confirms non-binary alpha, not an exact one-pixel
edge transition. Shared visual-weight normalization and 124px preview QA
are still importer work. Some ornamental detail remains fine at small
size; treat these as review-ready source sheets, not a certified final
Unity import.
