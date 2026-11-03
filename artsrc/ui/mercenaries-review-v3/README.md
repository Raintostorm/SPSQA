# Mercenaries / Upgrade v3
Approval preview only; no runtime changes.
Reference actual saved Unity captures: ../ui-polish-v2/mercenaries-live.png and ../global-review-v1/upgrade-live.png. These captures predate the third Mythic Stone wallet now present in code; they are explicitly labelled historical captures, not fresh Play Mode QA.
Built-in image_gen used. Prompt: two complete iPhone X ratio screens matching actual faceless Sickle Militia and simple faction stage; navy, larger numbers, collection 3-column head portraits, 8-stat separate detail, full 2/2 shard bar, next level deltas, gold-only upgrade, three currency wallets. Full detailed prompt retained in tool call.

## Implemented frame
AllyUpgradeView now uses navy surfaces, blue back/navigation, larger collection level/shard numbers, existing ShopV2 head crops for ordinary allies, cyan selected border, full-width detail shard bar, larger 8-stat values and coin-only gold button. Collection hides the duplicated shard bar above filters. Existing idle animation, sorting, Mythic grouping, costs and upgrade rules remain. Safe-area fitting added. Preview's extra Ready-only filter was not introduced: this pass changes presentation only. Runtime compile: 138 files clean. Fresh Play Mode visual verification pending.
