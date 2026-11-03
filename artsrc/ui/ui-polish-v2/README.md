# UI polish v2 — review only
Generated 2026-10-26 using built-in ImageGen via imagegen skill. No production Resources, code, catalogs or saves modified. Source generated alpha preserved; files copied unmodified.

- mercenaries-upgrade-demo.png: proposal board for compact roster stage, 3-column upper-body cards, larger labels, progress/ready states, detail view with 8 stats, persistent resource status and gold-only price.
- settings-icons-candidate.png: transparent 2-cell sheet, left damage numbers / right effects. Not sliced or installed yet.
- Plan: doc/WORK_QUEUE.md

## Review limitations
Generated portraits are illustrative and are not approved replacements for current character sprites. Names Forest Archer/Cave Spider/Scrap Crawler are proposed display names, not catalog edits. Collection card shard denominators are illustrative and not all consistent with actual level+1 requirements; runtime must use AllySummon.ShardsFor(level). The detail Lv4->5 / 5 shards / 2,000 gold follows current formulas, while stats are examples. Ready state must test both gold and shards; do not mark ready solely for a full shard bar. Idle animation remains existing game animation, not supplied by a static mockup.

## Prompt set
1. ui-mockup, transparent sprite sheet: exactly two large padded simple 2D settings pictograms. Left cream rounded impact puff containing 123; right cyan four-point magic spark with two small purple flecks. Thick dark outlines, flat chibi game style, legible at32px, no button/background/glow/filigree. Preserve generated alpha.
2. ui-mockup, two portrait mobile screens side-by-side using upgrade-live.png as existing-game style reference. Keep faceless chibi militia and flat green faction stage. Left compact Mercenaries stage, 3-column upper-body roster with progress and ready state, larger labels, filters and Back. Right larger stage, eight named actual stat categories, green HP/ATK deltas, level4->5, full5/5 shard requirement, persistent Ready to upgrade and gold-only2,000 button; separate i and Back. No Unity editor chrome, ornate frames, extra currencies, rate table or skin tabs. Sample values/names for review only.

## Production installation
User approved installation. Settings icon pair copied unmodified to Resources/SharedUI/settings-polish.png and displayed through RawImage UV cells. Mercenaries/Upgrade layout recreated as real uGUI, not a baked image. Current game sprites retained; common ally display names updated to Forest Archer, Cave Spider and Scrap Crawler. All shard requirements and readiness use real level/gold data, not mockup values. Validation: compile102 files, summon and ally-stat suites pass.
