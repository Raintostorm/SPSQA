# Relic language / UI audit — 2026-10-24

Source: user-supplied Spin+Squad! 2.5.1 XAPK, base_assets.apk; accompanying screenshots of Whip, Half Gloves, Werewolf Coat, Wallet and POS Device. No source art or code imported into runtime. No gameplay/UI changes during this audit.

## Direct APK evidence

Scanned 5,342 hash-named Unity bundles for TextAsset / relevant object names: did not recover the full readable English relic/localization table. Shared asset data contained mainly sprites; globalgamemanagers.assets contained MonoScript identifiers. The standard IL2CPP metadata file at assets/bin/Data/Managed/Metadata/global-metadata.dat contains:

- ArtifactInfo: Grade, ResId, MaxLevel, LevelPerValues, LocalizeValue, SettingValues, BonusSettingValues, BonusLocalizeValues.
- ArtifactLevelUpInfo: LevelUpCount, LevelUpCost.
- Localization methods: GetLocalizationKeyArtifactDesc, GetLocalizationKeyArtifactBonusDesc.
- ArtifactInformationPopUp: artifactImage, artifactTitle, artifactSlider, artifactDesc, levelUpIcon, disableText, costDimmed, gradeObjs, bonusGroup, bonusGroupRoot, lockGroup, requireAssetView, upgradeGoldCost.
- ArtifactView: disabledIcon, countSlider, newIconRedDot, ableObject, disableObject.

These names establish separate named data/UI concepts, not complete implementation, exact formulas or numeric tables. Full English names Half Gloves / Werewolf Coat / POS Device were not found as plain strings in this metadata; exact visible prose is taken from screenshots, not claimed to be decoded from APK. No inference that localization must be remote or encrypted.

## Screenshot evidence

- Half Gloves: concise current effect ATK SPD +10%.
- Werewolf Coat: effect specifies equipped mercenary and Base ATK SPD, with20% coefficient.
- Wallet:20coins at game start.
- POS Device:18coins at each wave end, separate locked Lv.6 bonus describing10% refund per100coins spent each wave.
- Whip: Lv.0 / NONE / Not owned; effect text still previews potential benefit.
- Prominent single relic icon, rarity badge, name, level/progress, dedicated prose region, price/action. These screenshots do not show a Next Level delta comparison.

## Project gaps

TreasureItem only has Id/name/tier/max/icon and Stat/Base/PerLevel. No description template, scope/trigger/condition, base-value reference, bonus unlock milestone or dedicated bonus block. TreasureBuffs applies all owned effects team-wide; all 30 relics are one/two numeric passive stats. TreasureReviewUi builds text from stat enum, hardcodes All allies and Next Level, with stat icon for every row. Current Next Level number is an increment, not next total. Armor is points; crit chance increase is percentage points, distinct from multiplicative ATK/HP percentages.

Do not call a relic icon wrong solely because the object is whimsical. The real problem is absence of explicit effect/description design, not a requirement that every attack relic look like a sword. Keep relic identity icon separate from optional stat icons. Existing 3-column inventory, Open All and manual gold+shard upgrades are user-approved divergences, not defects to revert.
