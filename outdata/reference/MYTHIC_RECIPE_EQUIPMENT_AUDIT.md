# Mythic, recruitment and equipment audit — 2026-10-28

Source: supplied `/Users/vuhagiabao/spin/Spin+Squad!_2.5.1_APKPure.xapk`, `base_assets.apk`, Unity global-metadata.dat; user screenshots. Read-only source inspection; no APK artwork or code imported.

## Direct metadata evidence
- `ModeBattleDefenseRecipePopup`: `SetCombineEquipScrollView`, `SetCombineUnitScrollView`, `UpdateUpgradeView`, `upgradeRoot`, `curEquipId`.
- `UnitCombinationInfo`: `ResultUnitId`, `CombinationUnitIds` — named result and ingredient identity data.
- `EquipCombinationInfo`: `ResultEquipId`, `CombEquipId01`, `CombEquipId02` — two equipment inputs and one result.
- `BattleEquipmentItem`: `EquipSpawnId`, `SetInventoryUI`, `OnSelectItem`, `userEquipTouchCallback` — spawned inventory item identity and selection.
- `BattleEquipSlotView`: `inCurEquipSlotCount`, `inMountEquipDic`, `equipSlotList` — separate equipped slots/state.
- `BattleEventRewardSelectItem`: `equipNormalIconGO`, `equipAdvancedIconGO`, `upgradeEpicGO`, `upgradeLegendGO` — reward presentation separates equipment and Epic/Legend upgrades.
- `LobbyRecruitPercentagePopup`: `ShowMythicUnitProbs`, `unitProbList`, `exceptUnitIds` — recruitment probability UI is separate from in-battle recipe UI.
- Asset inventory: `EquipIconMerge` and `EquipIconWearing` clips, each 0.25s.

These identifiers establish data/UI concepts, not complete method behavior. 5,342 hash-named bundles were scanned; readable TextAsset recruitment/Mythic tables were not recovered. Do not claim a complete Mythic roster, exact recipes, probabilities, reward waves or equipment restrictions were decoded. Screenshots establish the horizontal Mythic list and ingredient check presentation.

## User-approved requirements, independent of APK findings
- A ready Mythic's single portrait/Summon action fuses directly; no automatic recipe sheet on tapping it.
- Recipe opens a horizontally scrollable Mythic list; selecting a portrait displays its exact recipe.
- Bottom: Recipe left; one centered Roll/Start action (Start replaces Roll when insufficient coins); Item right.
- Exactly three named ally ingredients, at least one Epic/Legendary; two identical allowed, three identical forbidden.
- Battle item system: only Mythics equip; waves 5,15,25,... award weapon parts; two parts combine to a complete weapon with an effect. Exact combination catalogue/effects and equip limits remain unspecified.
- Epic+ upgrade choices use effect cards.

No gameplay/UI changes made during this audit.

## Deeper pass
- Reassembled split Unity scene/sharedasset files in numerical split order, including sharedassets0 (172,429,240 bytes), sharedassets2 (32,003,532 bytes), and levels 0–5.
- Object inventories contain no TextAsset entries in these reassembled files. Many custom MonoBehaviour reads fail due to missing/incompatible serialized type information (42,858 object read errors). Therefore this is NOT a complete decode of serialized game data.
- Parsed metadata v31 string heap and literal table separately. Found `LoadTable`, `ResourceVersion`, `ResourceVersionState`, CSV reader `InMemoryDataLoader`, `<csvFiles>5__2`, `Localization.csv`, `ToolWaveBattleInfo.csv`, and a ZIP-name format literal. These support versioned table loading capabilities but do not prove where the live table bytes reside or whether a specific table is downloaded.
- `BattleEquipInfo` exposes HP/ATK weights, crit probability, attack-speed weight, mana charge, lifesteal, defense weight and ability values. Values are still unknown.
- `ModeBattleDefenseUIReady` exposes separate recipe/unit/equipment actions and start-button roots. This does not mandate copying its controls into our game.
- No adb/emulator executable found on PATH or the standard local Android SDK paths during this pass. The next runtime audit would need an available Android device/emulator and its normally accessible downloaded resources/cache.

Evidence: `apk-deep/inventory.json`, `apk-deep/table-symbols.json`, `apk-deep/table-literals.json`. Raw symbol discovery is not execution tracing or a full Mythic catalogue.

## Touch interaction and item icon pass
Confirmed metadata identifiers: `TouchControlBase.OnTouchStartEquip`, `TouchControl_Normal.OnTouchDragEquip`, `OnTouchEndEquip`, `DragTouchDataBase.isEquipDragging`, `SelectEquipSpawnId`, `TargetEquipSpawnId`, `TouchStartPos`, `TouchEndPos`, `CheckDragging`, `Hit2d`, `HitUI`. These establish dedicated equipment drag input/state; exact allowed drop conditions and gesture thresholds remain unverified.

Extracted four source UI sprites to `apk-deep/item-icons/` for reference inspection only: `item_equipment` (sword/shield), `img_equipment` (simplified sword/shield silhouette), `icon_equip` (muted compact version), `equipment_icon_glow` (white glow). They use strong silhouettes with few details. These are category/feedback sprites, not the complete item catalogue. No source sprites imported into runtime.

Proposed original UI: compact inventory drawer; drag part onto compatible part to combine, drag complete weapon onto Mythic to equip; valid targets highlight, invalid release restores source, tap/hold opens a short tooltip. Gesture behavior is our design proposal, not fully decoded APK behavior. Keep the reserved Mythic portrait row separate from inventory.
