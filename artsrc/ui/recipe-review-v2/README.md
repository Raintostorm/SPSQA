# Recipe UI review v2 — proposal only

Current reference: ../audit-proposals-v1/recipe-in-game.png, an actual prior runtime capture. Checked against MythicFusionPanel.Open/IngredientCards: current bottom sheet has two tabs, selected hero, three ingredients, status in disabled Summon button, capacity and horizontal Mythic list. This is not a newly captured combat screen.

Proposed direction: retain bottom sheet and existing recipe logic; flat navy surfaces, readable ingredient names/counts, separate disabled Summon action and missing-material explanation, selected portrait/highlight and horizontal-scroll affordance. No invented fourth material, currency cost, or new skills. AI-generated comparison only, not wired into gameplay.

Generation: built-in imagegen; current screenshot plus original Guardian art as references. Prompt specifies iPhone X1125x2436 composition, three ingredients(Epic Crescent Mage + two Common Sickle Militia), missing state and Mythic0/2 capacity, existing bottom controls.

## Implemented
MythicFusionPanel now uses a taller preparation bottom sheet (bottom330, top1590 in1125x2436 reference space), flat navy interior, icon tabs/underline, larger hero portrait and three ingredient cards with rarity and per-instance0/1 or1/1 allocation. Summon keeps a stable label and separate status; capacity and horizontal portrait list remain. Right chevron advances the list; saved scroll position is restored after canvas layout. The Item tab retains its existing recipe content and shorter sheet. Uses live game portraits and existing UI assets, not generated character replacements.

Validation: runtime C# compilation clean. Layout intervals checked for ingredient/button/status/capacity/list separation. Full interactive Play Mode review is still pending.
