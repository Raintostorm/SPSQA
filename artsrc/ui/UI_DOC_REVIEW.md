# UI art review — 2026-10-09

Generated with built-in imagegen from `outdata/PROMPT_UI_DOC.md`.
Style reference: `artsrc/ui/ui_sheet.png` (style only; not its magenta).
PNG outputs copied without thresholding alpha or cutting into cells.
No Unity assets or code changed.

## Delivered candidates, not approved production assets

- `resources.png`: 2079×756; four icons in correct order. Alpha has all
  256 levels, including zero. Visible-art outer margins measured at
  alpha >20: 54/132/49/148 px (left/top/right/bottom). Horizontal gaps:
  69/94/94 px; first gap misses the requested 80px minimum. Closed/open
  chest bodies are similar but not pixel-identical; alignment remains
  unverified. Icon height is larger than requested ~280px.
- `bar_resource.png`: 2172×724, visible bbox (97,126)-(2073,583),
  aspect 4.324:1 rather than 3:1. All 256 alpha levels. Endcap width
  is not certified 48px; centre includes slight uneven illumination.
  Not yet safe to mark as a compliant 384×128 9-slice.
- `button_treasure.png`: 1774×887. All 256 alpha levels. Visible-art
  outer margins 84/73/87/78 px, gap 90px. Both states present; native
  diameters around 756px rather than 256px. Their geometry differs
  slightly, so pixel-for-pixel alignment is NOT certified.

Measurement threshold >20 was used only to measure bounds, not to
modify the output alpha. Having 256 alpha levels confirms non-binary
alpha, but does not certify a one-pixel edge transition.

These are raw AI art candidates for visual review. Exact dimensions,
paired geometry, repeatable centre and all spacing constraints still
need correction/validation before import. Do not treat them as passed.
