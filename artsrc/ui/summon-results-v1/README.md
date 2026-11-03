# Summon result UI

Approved concept: proposal.png. Runtime uses existing game portraits and reusable ribbon/card/button art, not baked mockup text or newly redrawn characters.

- 3 columns, duplicate UnitId grouping, rarity descending then display name.
- Actual total shards and distinct ally count; large quantities and upper-body portraits.
- Safe-area fit, scrollable rewards, fixed Continue; overlay blocks underlying controls.
- TryRoll remains responsible for charge/grant/save before presentation. 20 shards per summon unchanged.
- Editor Diagnostics > Preview Summon Results (no grants), Cmd+Alt+S: fixture totals 200 across 6 allies without spending/granting/saving.

Validation: 110 C# sources compile clean; tools/test_summon.sh passes. Unity play-mode fixture verified total 200 / 6, grouped counts, rarity cards, upper portraits, and Continue dismissal.
