# Shop v1 — 2026-10-24

Implemented currency exchange and nine mixed shard offers in3columns. Five unique allies and four unique relics, shuffled daily, each purchased once. Offers and sold flags persist in SaveData, reset at00:00 UTC+7, based on device UTC. Reopening does not reroll; clock rollback cannot reopen an earlier day. This is local/offline behavior, not server anti-cheat.

Prices:100/500/1000gems →10000/50000/100000gold. Ally20shards:Common15/Rare30/Epic75/Legendary150gems. Relic5shards:10/25/60/120gems. All configured in Resources/Shop/prices.json. First relic purchase unlocks Lv.1 plus purchased shards; existing levels unchanged. Confirmations revalidate day/resources/sold status; no partial debit. Free gem packs retained in separate TEST / NO PAYMENT section, as previously requested.

APK evidence: IL2CPP metadata names LobbyUIShop (dailyShopView, currencyShopView, artifactShardItemView), DailyShopView, ShopDailyProductView, ShopAssetProductItem, CurrentDailyGoodsIdList, RefreshDailyShopInfo, ShopDailyRefreshGoldCost and ShopDailyRefreshGoldCount. This supports separated sections, product cards and persisted daily offers; exact original visual positions, live prices and purchase limits were not extracted. Our3x3 requirement and chosen prices are user-authorized design, not claimed original APK values.

Built-in ImageGen transparent output, unchanged alpha. New assets gold.png and fragment.png copied to Resources/Shop. Shared prompt: production mobile fantasy cartoon UI icon; flat saturated colors, thick smooth black outline, small highlight, minimal detail, centered square18% transparent padding, no glow/shadow/text/numbers/scene. Gold prompt:compact pile of crown-marked gold coins with small open brown pouch. Fragment prompt:single ivory parchment puzzle piece with thick outline and small teal corner accent, shard marker not gem/currency. Item identity uses actual existing unit portrait/relic icon; fragment only overlays a corner. Rounded buttons/cards reuse RelicUiSkin.

Resources/Shop/Relics.asset is a runtime catalog copy of Data/TreasureDatabase.asset so the shop works from Home before opening Relics; refresh the copy if the source catalog changes later.

Verification: C# compilation and isolated daily-shop/relic/summon tests pass. Tests use in-memory saves only. Live Shop visual/interaction verification still pending in the open Unity editor.
