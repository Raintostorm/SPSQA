# UI audit — 2026-10-25

Scope: current runtime UI source and user screenshots. Not an exhaustive on-device traversal. Demo is isolated and changes no production code or save data.

## Priority 0: information accuracy
- GemEconomyScreen.Summon: UI hardcodes +1 Shard, count shard(s) received. AllySummon.Result.Shards is 20. Render Result.Shards and sum totals, including duplicates. Costs remain 10/100.
- Disabled controls need reason: enough gold but insufficient shards vs insufficient gold vs max level. Keep price legible. Never rely on dimming alone.

## Priority 1: consistent reading and navigation
- ShopUiSkin, RelicUiSkin, BattleUiSkin and procedural BattleModalView surfaces coexist. Consolidate design tokens: wallet, modal frame, 3 action roles, radius/stroke, typography and spacing. Preserve current art direction and icon-only Home dock.
- One numeric hierarchy: primary value, label, helper. Keep game text in a real font, tabular digits where possible. Avoid making a raster font atlas for variable numbers.
- Home, Shop, Relics, Allies, Summon each position Back/wallet/title differently. Shared page header/safe-area shell, fixed footer and one scroll region. Touch hit targets measured on final device resolution.
- Relic detail: scope/stat on first line, current value and next delta below, full-width shard bar, gold-only price. Already partially present; refine spacing rather than add more icons.
- Allies: retain roster/detail split, idle stage, all eight real stats. Remove technical stat explanation from primary view and put it behind info. Add next-level context once, not eight repeated labels.
- ChapterSelect: gems reward is currently smaller plain text while Gold/Keys are large icons+values. Equal reward cells for all earned currencies. Codex/Missions currently say Coming soon; hide or visibly mark unavailable.
- Settings BGM tile is always unavailable; vibration only supported on mobile. Filter unsupported options, retain ON/OFF text. Leave confirm remains without X.
- Result: retain victory/waves/time but reduce redundant RESULT/VICTORY headings, align reward amounts in a row/grid and distinguish primary Next/Retry from Home.

## Priority 2: art and interaction polish
- Shared ally portrait crop by faction/unit bounds (machines need different upper-body crop); same portrait in shop/roster/results, full sprite only on stage/battle.
- Rarity belongs on portrait/card accents; action color should encode action type consistently. Sold out should be visibly inactive; current transparent price target does not dim its atlas strip.
- Visible scroll affordance for tall collections. Validate touch drag, wheel, safe area and small screens; wheel success alone does not establish touch success.
- Inline success status should expire rather than indefinitely covering lower shop content.

## Demo
index.html: three local tabs (relic upgrade, summon result, settings), existing asset files embedded unmodified. Upgrade and toggles demonstrate states only. No production changes, no new assets required.

## Applied to production runtime
- Summon uses sum of actual result.Shards and groups duplicate units; portrait crop.
- Upgrade displays missing-resource reasons, preserves eight stats and moves formula explanation behind info.
- Relic manual upgrade retains gold-only price with an explicit eligibility reason.
- Chapter rewards use equal-scale gold/key/gem cells; unfinished shortcuts disabled and labeled.
- Unsupported settings filtered; resume action emphasized. Leave confirmation behavior preserved.
- Result has a single outcome heading and three aligned reward cells.
- Shop/roster/relic collections have shared non-interactive scroll indicators. Shop sold items dim, status expires.
- Shop wallet and relic primary-action skins reuse BattleUiSkin. Roster portraits crop to upper body.

Verification: 102 C# files compile; summon, ally-stat, relic-upgrade and daily-shop suites pass. Live Unity Upgrade checked with missing shards, all eight stats, readable gold price and new info control. Other screens not all traversed in this pass; touch-device gestures remain unverified. This is an incremental consistency pass, not a replacement of every legacy UI helper.
