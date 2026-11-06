# D1 verified 057
2026-11-05. Static reachable-path review, **chưa chạy Unity**. “có” means a valid code path can load, not PlayMode observation. All 049 candidate lines classified below.
## Asset decisions
| Asset | Decision | GUID consumers | Reason |
|---|---|---:|---|
| `Assets/_Project/Resources/ButtonFamilies/atlas.png` | KEEP | 0 | ButtonFamilySkin.Get loads atlas; BattleUiSkin.Skin calls Get(6/7) and Red/Purple/Info/etc use Get. Live and fallback paths. No tool input dependency found. |
| `Assets/_Project/Resources/Buttons/blue-pressed.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/Buttons/blue.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/Buttons/disabled.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/Buttons/gold-pressed.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/Buttons/gold.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/Buttons/red-pressed.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/Buttons/red.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/Buttons/slate-pressed.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/Buttons/slate.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/SharedUI/btn_blue.png` | KEEP | 0 | SharedSettings.Start literal Resources.Load<Sprite>(SharedUI/btn_blue); keep even if later role skin replaces it. Review tool references: tools/import_settings_icons.py, tools/import_home_chrome.py, tools/build_jackpot_castle_review.py, tools/build_skill_roll_review.py, tools/build_home_walk_review.py, tools/import_home_walk_art.py, tools/import_chapter_art.py, tools/import_summon_art.py, tools/import_action_icons.py, tools/build_jackpot_review.py |
| `Assets/_Project/Resources/SharedUI/btn_primary.png` | KEEP | 0 | SharedSettings.Button Resume literal load; valid initialization path before role skin. Review tool references: tools/import_settings_icons.py, tools/import_home_chrome.py, tools/build_jackpot_castle_review.py, tools/build_skill_roll_review.py, tools/build_home_walk_review.py, tools/import_home_walk_art.py, tools/import_chapter_art.py, tools/import_summon_art.py, tools/import_action_icons.py, tools/build_jackpot_review.py |
| `Assets/_Project/Resources/SharedUI/buff_speed.png` | KEEP | 0 | Visual/MonsterBuffVisual.cs MakeBadge AttackSpeed literal load. Review tool references: tools/import_settings_icons.py, tools/import_home_chrome.py, tools/build_jackpot_castle_review.py, tools/build_skill_roll_review.py, tools/build_home_walk_review.py, tools/import_home_walk_art.py, tools/import_chapter_art.py, tools/import_summon_art.py, tools/import_action_icons.py, tools/build_jackpot_review.py |
| `Assets/_Project/Resources/SharedUI/gold.png` | KEEP_TOOL | 0 | BattleUiSkin.Icon real arguments: settings,battle,enemy,upgrade,qualified Glyphs/home/close and HomeChrome/coin. SharedSettings generic pictogram args use home/close/restart/resume; toggle args sound/vibration; no gold/key. CoinRewardEffect now HomeChrome/coin. Old token matches are other loaders. Actual tool input: build_home_walk_review.py:13 reads SharedUI/{name}.png with names gold/key; retain to keep reproducible demo. |
| `Assets/_Project/Resources/SharedUI/key.png` | KEEP_TOOL | 0 | BattleUiSkin.Icon real arguments: settings,battle,enemy,upgrade,qualified Glyphs/home/close and HomeChrome/coin. SharedSettings generic pictogram args use home/close/restart/resume; toggle args sound/vibration; no gold/key. CoinRewardEffect now HomeChrome/coin. Old token matches are other loaders. Actual tool input: build_home_walk_review.py:13 reads SharedUI/{name}.png with names gold/key; retain to keep reproducible demo. |
| `Assets/_Project/Resources/SummonKit/card-common.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/SummonKit/card-epic.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/SummonKit/card-legendary.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/SummonKit/card-rare.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/SummonKit/card-stone.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/SummonKit/price-pill.png` | KEEP | 0 | GemEconomyScreen.SummonButton, SummonResultsView count badge, DailyShopScreen count badge literal loads. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/SummonKit/summon-x1.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/SummonKit/summon-x10.png` | RETIRE | 0 | No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method. Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained. |
| `Assets/_Project/Resources/UiRefresh/atk.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/atk No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/back.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/back No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/battle.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/battle No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/close No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/coin No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/cyan.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/cyan No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/defense.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/defense No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/dice.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/dice No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/disabled.bytes` | RETIRE | 0 | Only ApprovedUiArt.GoldStates references this slug; no code calls GoldStates. No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/gold No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/hp.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/hp No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/info.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/info No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/inventory.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/inventory No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/key.bytes` | RETIRE | 0 | ResultPanel reward key and HomeChrome wallet key use HomeChrome/key, not ApprovedUiArt; generic UiRefresh loader alone is not an argument. No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/menu.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/menu No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/mp.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/mp No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/navy.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/navy No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/next.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/next No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/panel.bytes` | RETIRE | 0 | No ApprovedUiArt call passes panel. BattleSceneSetup panel is a serialized property name, unrelated. No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/pressed.bytes` | RETIRE | 0 | Only ApprovedUiArt.GoldStates references this slug; no code calls GoldStates (one comment says deliberately not used). No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/previous.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/previous No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/range.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/range No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/recipe.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/recipe No tool input dependency found. |
| `Assets/_Project/Resources/UiRefresh/speed.bytes` | KEEP | 0 | ApprovedUiArt.Get/Apply/StatTexture active argument maps to UiRefresh/speed No tool input dependency found. |

## Each 049 code candidate
| Asset | file:line | Resource key | có/không/không rõ | Evidence |
|---|---|---|---|---|
| `Assets/_Project/Resources/ButtonFamilies/atlas.png` | `Assets/_Project/Scripts/UI/ButtonFamilySkin.cs:15` | `ButtonFamilies/atlas` | có | Literal atlas load reached from ButtonFamilySkin.Get. |
| `Assets/_Project/Resources/SharedUI/btn_blue.png` | `Assets/_Project/Scripts/UI/BattleModalView.cs:75` | `SharedUI/btn_blue` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/btn_blue.png` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58` | `SharedUI/btn_blue` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/btn_blue.png` | `Assets/_Project/Scripts/UI/SharedSettings.cs:54` | `SharedUI/btn_blue` | có | Exact SharedUI Resources load (not Art/UI template). |
| `Assets/_Project/Resources/SharedUI/btn_blue.png` | `Assets/_Project/Scripts/UI/SharedSettings.cs:153` | `SharedUI/btn_blue` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/btn_blue.png` | `Assets/_Project/Scripts/Editor/TreasureSceneSetup.cs:59` | `SharedUI/btn_blue` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/btn_primary.png` | `Assets/_Project/Scripts/UI/BattleModalView.cs:75` | `SharedUI/btn_primary` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/btn_primary.png` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58` | `SharedUI/btn_primary` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/btn_primary.png` | `Assets/_Project/Scripts/UI/SharedSettings.cs:153` | `SharedUI/btn_primary` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/btn_primary.png` | `Assets/_Project/Scripts/UI/SharedSettings.cs:159` | `SharedUI/btn_primary` | có | Exact SharedUI Resources load (not Art/UI template). |
| `Assets/_Project/Resources/SharedUI/btn_primary.png` | `Assets/_Project/Scripts/Editor/UpgradeSceneSetup.cs:81` | `SharedUI/btn_primary` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/buff_speed.png` | `Assets/_Project/Scripts/UI/BattleModalView.cs:75` | `SharedUI/buff_speed` | có | Check live evidence MonsterBuffVisual; generic helper string alone not sufficient. |
| `Assets/_Project/Resources/SharedUI/buff_speed.png` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58` | `SharedUI/buff_speed` | có | Check live evidence MonsterBuffVisual; generic helper string alone not sufficient. |
| `Assets/_Project/Resources/SharedUI/buff_speed.png` | `Assets/_Project/Scripts/UI/SharedSettings.cs:153` | `SharedUI/buff_speed` | có | Check live evidence MonsterBuffVisual; generic helper string alone not sufficient. |
| `Assets/_Project/Resources/SharedUI/buff_speed.png` | `Assets/_Project/Scripts/Visual/MonsterBuffVisual.cs:37` | `SharedUI/buff_speed` | có | Check live evidence MonsterBuffVisual; generic helper string alone not sufficient. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/BattleModalView.cs:75` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:13` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:19` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/ScreenChrome.cs:85` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/ResultPanel.cs:126` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:48` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:50` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/gold.png` | `Assets/_Project/Scripts/UI/SharedSettings.cs:153` | `SharedUI/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/key.png` | `Assets/_Project/Scripts/UI/BattleModalView.cs:75` | `SharedUI/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/key.png` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58` | `SharedUI/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/key.png` | `Assets/_Project/Scripts/UI/ResultPanel.cs:126` | `SharedUI/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/key.png` | `Assets/_Project/Scripts/UI/ResultPanel.cs:152` | `SharedUI/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/key.png` | `Assets/_Project/Scripts/UI/HomeChrome.cs:41` | `SharedUI/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SharedUI/key.png` | `Assets/_Project/Scripts/UI/SharedSettings.cs:153` | `SharedUI/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/SummonKit/price-pill.png` | `Assets/_Project/Scripts/UI/SummonResultsView.cs:56` | `SummonKit/price-pill` | có | Literal Resources key. |
| `Assets/_Project/Resources/SummonKit/price-pill.png` | `Assets/_Project/Scripts/UI/DailyShopScreen.cs:93` | `SummonKit/price-pill` | có | Literal Resources key. |
| `Assets/_Project/Resources/SummonKit/price-pill.png` | `Assets/_Project/Scripts/UI/GemEconomyScreen.cs:99` | `SummonKit/price-pill` | có | Literal Resources key. |
| `Assets/_Project/Resources/UiRefresh/atk.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:193` | `UiRefresh/atk` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/atk.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/atk` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/atk.bytes` | `Assets/_Project/Scripts/UI/ChapterMonsterViews.cs:114` | `UiRefresh/atk` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/atk.bytes` | `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125` | `UiRefresh/atk` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/atk.bytes` | `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77` | `UiRefresh/atk` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/back.bytes` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:22` | `UiRefresh/back` | có | Helper call argument resolves to UiRefresh/back |
| `Assets/_Project/Resources/UiRefresh/back.bytes` | `Assets/_Project/Scripts/UI/ScreenChrome.cs:263` | `UiRefresh/back` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/back.bytes` | `Assets/_Project/Scripts/UI/ScreenChrome.cs:265` | `UiRefresh/back` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/back.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/back` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/back.bytes` | `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:105` | `UiRefresh/back` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/battle.bytes` | `Assets/_Project/Scripts/UI/BattleHud.cs:123` | `UiRefresh/battle` | có | Helper call argument resolves to UiRefresh/battle |
| `Assets/_Project/Resources/UiRefresh/battle.bytes` | `Assets/_Project/Scripts/UI/MythicFusionPanel.cs:155` | `UiRefresh/battle` | có | Helper call argument resolves to UiRefresh/battle |
| `Assets/_Project/Resources/UiRefresh/battle.bytes` | `Assets/_Project/Scripts/UI/HomeScreen.cs:77` | `UiRefresh/battle` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/battle.bytes` | `Assets/_Project/Scripts/UI/HomeScreen.cs:79` | `UiRefresh/battle` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/battle.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/battle` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/battle.bytes` | `Assets/_Project/Scripts/UI/SharedSettings.cs:199` | `UiRefresh/battle` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | `Assets/_Project/Scripts/UI/BattleChrome.cs:39` | `UiRefresh/close` | có | Helper call argument resolves to UiRefresh/close |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | `Assets/_Project/Scripts/UI/MythicFusionPanel.cs:159` | `UiRefresh/close` | có | Helper call argument resolves to UiRefresh/close |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:166` | `UiRefresh/close` | có | Helper call argument resolves to UiRefresh/close |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/close` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | `Assets/_Project/Scripts/UI/SharedSettings.cs:144` | `UiRefresh/close` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | `Assets/_Project/Scripts/UI/SharedSettings.cs:153` | `UiRefresh/close` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:58` | `UiRefresh/close` | có | Helper call argument resolves to UiRefresh/close |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | `Assets/_Project/Scripts/UI/BattleHud.cs:127` | `UiRefresh/coin` | có | Helper call argument resolves to UiRefresh/coin |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | `Assets/_Project/Scripts/UI/RollRewardArt.cs:68` | `UiRefresh/coin` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | `Assets/_Project/Scripts/UI/RollRewardArt.cs:91` | `UiRefresh/coin` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | `Assets/_Project/Scripts/UI/ResultPanel.cs:152` | `UiRefresh/coin` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:82` | `UiRefresh/coin` | có | Helper call argument resolves to UiRefresh/coin |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:208` | `UiRefresh/coin` | có | Helper call argument resolves to UiRefresh/coin |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | `Assets/_Project/Scripts/UI/HomeChrome.cs:40` | `UiRefresh/coin` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/coin` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/cyan.bytes` | `Assets/_Project/Scripts/UI/BattleChrome.cs:22` | `UiRefresh/cyan` | có | Helper call argument resolves to UiRefresh/cyan |
| `Assets/_Project/Resources/UiRefresh/cyan.bytes` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:18` | `UiRefresh/cyan` | có | Helper call argument resolves to UiRefresh/cyan |
| `Assets/_Project/Resources/UiRefresh/cyan.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/cyan` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/defense.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:193` | `UiRefresh/defense` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/defense.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/defense` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/defense.bytes` | `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125` | `UiRefresh/defense` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/defense.bytes` | `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77` | `UiRefresh/defense` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/dice.bytes` | `Assets/_Project/Scripts/UI/BattleChrome.cs:39` | `UiRefresh/dice` | có | Helper call argument resolves to UiRefresh/dice |
| `Assets/_Project/Resources/UiRefresh/dice.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/dice` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/disabled.bytes` | `Assets/_Project/Scripts/UI/ScreenChrome.cs:100` | `UiRefresh/disabled` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/disabled.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/disabled` | không | Generic loader, reviewed actual caller argument set: slug not passed by live callers. |
| `Assets/_Project/Resources/UiRefresh/disabled.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:50` | `UiRefresh/disabled` | không | GoldStates method has zero callers; not a live path. |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:13` | `UiRefresh/gold` | có | Helper call argument resolves to UiRefresh/gold |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:19` | `UiRefresh/gold` | có | Helper call argument resolves to UiRefresh/gold |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | `Assets/_Project/Scripts/UI/ScreenChrome.cs:85` | `UiRefresh/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | `Assets/_Project/Scripts/UI/ResultPanel.cs:126` | `UiRefresh/gold` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/gold` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:48` | `UiRefresh/gold` | không | GoldStates method has zero callers; not a live path. |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:50` | `UiRefresh/gold` | không | GoldStates method has zero callers; not a live path. |
| `Assets/_Project/Resources/UiRefresh/hp.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:193` | `UiRefresh/hp` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/hp.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/hp` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/hp.bytes` | `Assets/_Project/Scripts/UI/ChapterMonsterViews.cs:114` | `UiRefresh/hp` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/hp.bytes` | `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125` | `UiRefresh/hp` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/hp.bytes` | `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77` | `UiRefresh/hp` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/hp.bytes` | `Assets/_Project/Scripts/Debug/BattleProbe.cs:54` | `UiRefresh/hp` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/info.bytes` | `Assets/_Project/Scripts/UI/CellPanel.cs:77` | `UiRefresh/info` | có | Helper call argument resolves to UiRefresh/info |
| `Assets/_Project/Resources/UiRefresh/info.bytes` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:23` | `UiRefresh/info` | có | Helper call argument resolves to UiRefresh/info |
| `Assets/_Project/Resources/UiRefresh/info.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/info` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/inventory.bytes` | `Assets/_Project/Scripts/UI/BattleChrome.cs:21` | `UiRefresh/inventory` | có | Helper call argument resolves to UiRefresh/inventory |
| `Assets/_Project/Resources/UiRefresh/inventory.bytes` | `Assets/_Project/Scripts/UI/BattleChrome.cs:39` | `UiRefresh/inventory` | có | Helper call argument resolves to UiRefresh/inventory |
| `Assets/_Project/Resources/UiRefresh/inventory.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/inventory` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/key.bytes` | `Assets/_Project/Scripts/UI/ResultPanel.cs:126` | `UiRefresh/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/key.bytes` | `Assets/_Project/Scripts/UI/ResultPanel.cs:152` | `UiRefresh/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/key.bytes` | `Assets/_Project/Scripts/UI/HomeChrome.cs:41` | `UiRefresh/key` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/key.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/key` | không | Generic loader, reviewed actual caller argument set: slug not passed by live callers. |
| `Assets/_Project/Resources/UiRefresh/menu.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/menu` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/menu.bytes` | `Assets/_Project/Scripts/UI/SharedSettings.cs:97` | `UiRefresh/menu` | có | Helper call argument resolves to UiRefresh/menu |
| `Assets/_Project/Resources/UiRefresh/mp.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/mp` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/mp.bytes` | `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125` | `UiRefresh/mp` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/mp.bytes` | `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77` | `UiRefresh/mp` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/navy.bytes` | `Assets/_Project/Scripts/UI/BattleChrome.cs:21` | `UiRefresh/navy` | có | Helper call argument resolves to UiRefresh/navy |
| `Assets/_Project/Resources/UiRefresh/navy.bytes` | `Assets/_Project/Scripts/UI/BattleUiSkin.cs:17` | `UiRefresh/navy` | có | Helper call argument resolves to UiRefresh/navy |
| `Assets/_Project/Resources/UiRefresh/navy.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/navy` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/next.bytes` | `Assets/_Project/Scripts/UI/ResultPanel.cs:135` | `UiRefresh/next` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/next.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:216` | `UiRefresh/next` | có | Helper call argument resolves to UiRefresh/next |
| `Assets/_Project/Resources/UiRefresh/next.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/next` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/next.bytes` | `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:109` | `UiRefresh/next` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/panel.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/panel` | không | Generic loader, reviewed actual caller argument set: slug not passed by live callers. |
| `Assets/_Project/Resources/UiRefresh/panel.bytes` | `Assets/_Project/Scripts/Editor/BattleSceneSetup.cs:483` | `UiRefresh/panel` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/pressed.bytes` | `Assets/_Project/Scripts/UI/ScreenChrome.cs:98` | `UiRefresh/pressed` | không | Token is not a load of this asset. |
| `Assets/_Project/Resources/UiRefresh/pressed.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/pressed` | không | Generic loader, reviewed actual caller argument set: slug not passed by live callers. |
| `Assets/_Project/Resources/UiRefresh/pressed.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:50` | `UiRefresh/pressed` | không | GoldStates method has zero callers; not a live path. |
| `Assets/_Project/Resources/UiRefresh/previous.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:216` | `UiRefresh/previous` | có | Helper call argument resolves to UiRefresh/previous |
| `Assets/_Project/Resources/UiRefresh/previous.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/previous` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/range.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/range` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/range.bytes` | `Assets/_Project/Scripts/UI/ChapterMonsterViews.cs:114` | `UiRefresh/range` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/range.bytes` | `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125` | `UiRefresh/range` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/range.bytes` | `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77` | `UiRefresh/range` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/recipe.bytes` | `Assets/_Project/Scripts/UI/BattleChrome.cs:21` | `UiRefresh/recipe` | có | Helper call argument resolves to UiRefresh/recipe |
| `Assets/_Project/Resources/UiRefresh/recipe.bytes` | `Assets/_Project/Scripts/UI/BattleChrome.cs:39` | `UiRefresh/recipe` | có | Helper call argument resolves to UiRefresh/recipe |
| `Assets/_Project/Resources/UiRefresh/recipe.bytes` | `Assets/_Project/Scripts/UI/MythicFusionPanel.cs:155` | `UiRefresh/recipe` | có | Helper call argument resolves to UiRefresh/recipe |
| `Assets/_Project/Resources/UiRefresh/recipe.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/recipe` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/speed.bytes` | `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:193` | `UiRefresh/speed` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/speed.bytes` | `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20` | `UiRefresh/speed` | có | Generic loader, reviewed actual caller argument set: slug reachable. |
| `Assets/_Project/Resources/UiRefresh/speed.bytes` | `Assets/_Project/Scripts/UI/ChapterMonsterViews.cs:114` | `UiRefresh/speed` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/speed.bytes` | `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125` | `UiRefresh/speed` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |
| `Assets/_Project/Resources/UiRefresh/speed.bytes` | `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77` | `UiRefresh/speed` | có | Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second. |

## Newly discovered live evidence missing from 049
- `Assets/_Project/Scripts/Visual/MonsterBuffVisual.cs:37`: `Resources.Load<Sprite>("SharedUI/buff_speed")` — KEEP.
- `Assets/_Project/Scripts/UI/DailyShopScreen.cs:93`: SummonKit/price-pill — KEEP.

## Moves
Applied: True
GUID scan: serialized Assets .unity/.prefab/.asset/.mat/.anim/.controller/.overrideController/.playable/.json; excludes .meta self and retired art. Legacy importers may recreate old output targets if rerun; not deleted or rewritten.
- `Assets/_Project/Resources/Buttons/blue-pressed.png` → `artsrc/ui/retired-2026-11-05/Buttons/blue-pressed.png` + .meta; SHA256 `117677050e89825d6138622f665eaea9d6d72a0b967d41755ebd0f8bd8bf7f42`
- `Assets/_Project/Resources/Buttons/blue.png` → `artsrc/ui/retired-2026-11-05/Buttons/blue.png` + .meta; SHA256 `ab9df9f1cdc8fcb1397564aee9518779ea3059d6c082bce4b90426514ec14894`
- `Assets/_Project/Resources/Buttons/disabled.png` → `artsrc/ui/retired-2026-11-05/Buttons/disabled.png` + .meta; SHA256 `51e37f87e05f4262454206a96ef61167b998dadb780f1dce413bcab3ef132443`
- `Assets/_Project/Resources/Buttons/gold-pressed.png` → `artsrc/ui/retired-2026-11-05/Buttons/gold-pressed.png` + .meta; SHA256 `c3ad87cf11edac8f2dc7d3d2618fe8f50b8f9b48dbb6d44cfe4d4964c9ae04f9`
- `Assets/_Project/Resources/Buttons/gold.png` → `artsrc/ui/retired-2026-11-05/Buttons/gold.png` + .meta; SHA256 `163bdfb905ce759f30611a3800d03b23a9537e5679f46801f5494a035b0caf93`
- `Assets/_Project/Resources/Buttons/red-pressed.png` → `artsrc/ui/retired-2026-11-05/Buttons/red-pressed.png` + .meta; SHA256 `9a18f74fe10a362837b5f50698fc25062d050f433965c9eb8742c192e0de40e0`
- `Assets/_Project/Resources/Buttons/red.png` → `artsrc/ui/retired-2026-11-05/Buttons/red.png` + .meta; SHA256 `2d4f1435f4468285fbdbfb671cf08ba948dce30f3a2cc0c587d574bc8b150ef2`
- `Assets/_Project/Resources/Buttons/slate-pressed.png` → `artsrc/ui/retired-2026-11-05/Buttons/slate-pressed.png` + .meta; SHA256 `1cde2d3fc5572c00038807a14127b110578f5be746a34ca0de2b5b86d0c4ab47`
- `Assets/_Project/Resources/Buttons/slate.png` → `artsrc/ui/retired-2026-11-05/Buttons/slate.png` + .meta; SHA256 `64aa7ee8c28239b432c8ae5881f3eab31aaa7bef29b75210b8b0da4751dfbd45`
- `Assets/_Project/Resources/SummonKit/card-common.png` → `artsrc/ui/retired-2026-11-05/SummonKit/card-common.png` + .meta; SHA256 `71552e35b313a98a384108b4f61481b3e77fb98cfd27abd220f21a904fa1d8ef`
- `Assets/_Project/Resources/SummonKit/card-epic.png` → `artsrc/ui/retired-2026-11-05/SummonKit/card-epic.png` + .meta; SHA256 `3a49e8452b1c404681ddeb91669aaf46f1cd85515b5529e0d5cee7c1e34a7a7f`
- `Assets/_Project/Resources/SummonKit/card-legendary.png` → `artsrc/ui/retired-2026-11-05/SummonKit/card-legendary.png` + .meta; SHA256 `e9dc7bfa475a29fd3b453d1dd9cf80653ddc98bf413259fb7e928cf677dfcff6`
- `Assets/_Project/Resources/SummonKit/card-rare.png` → `artsrc/ui/retired-2026-11-05/SummonKit/card-rare.png` + .meta; SHA256 `2650a1f5a78e3c0a73536c6800f66e48ddb9f9f1d9b0d696ecc38339b842506d`
- `Assets/_Project/Resources/SummonKit/card-stone.png` → `artsrc/ui/retired-2026-11-05/SummonKit/card-stone.png` + .meta; SHA256 `06af11617402083021b1d984b2e1f16f276a12e57fd69608d3e7241cdcb483cd`
- `Assets/_Project/Resources/SummonKit/summon-x1.png` → `artsrc/ui/retired-2026-11-05/SummonKit/summon-x1.png` + .meta; SHA256 `9c28fd64fed7d7fe5d30db25d8ab902cdeb8a078fca8d87d0fd20051e43c3575`
- `Assets/_Project/Resources/SummonKit/summon-x10.png` → `artsrc/ui/retired-2026-11-05/SummonKit/summon-x10.png` + .meta; SHA256 `b25144863b28a7f026f2798f53d4a83caecf54be6c5c3a388790f3341ea66a8f`
- `Assets/_Project/Resources/UiRefresh/disabled.bytes` → `artsrc/ui/retired-2026-11-05/UiRefresh/disabled.bytes` + .meta; SHA256 `cfed3d69e95f945f0d23f62fac92bb3910475946f40bc1b62050d301694961ad`
- `Assets/_Project/Resources/UiRefresh/key.bytes` → `artsrc/ui/retired-2026-11-05/UiRefresh/key.bytes` + .meta; SHA256 `07d1144e5bcae3d0e755927927a3f11a78ffca5aff52897caa9f05ff3f2a00a9`
- `Assets/_Project/Resources/UiRefresh/panel.bytes` → `artsrc/ui/retired-2026-11-05/UiRefresh/panel.bytes` + .meta; SHA256 `ebf3c8d9d6b761ab5d5e4befaeafce79615765427162ba83f0ac8a42dda1875d`
- `Assets/_Project/Resources/UiRefresh/pressed.bytes` → `artsrc/ui/retired-2026-11-05/UiRefresh/pressed.bytes` + .meta; SHA256 `eb8feef4416820548ec5364c59db1aaa07b50363517cdd0a0f8303e33c01d8ad`
