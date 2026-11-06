# Audit asset trước khi generate · 2026-11-04

Đã xem contact sheet66icon trong StatIcons/SharedUI/HomeChrome/Badges/Summon/Shop/ButtonIcons/Glyphs/EffectBuffs/icons và tra callsite mã. Đây là kiểm art tĩnh, chưa screenshotPlayMode.

- Restart: `SharedUI/restart` cóviềnđen vàđọcđược48px, nhưng `Glyphs/replay` đãđạt vàđượcResultPaneldùng. Tái dùngreplay, khônggenerate.
- Resume: `SharedUI/resume` làtamgiácvàng cóviềnđen, khôngphảihìnhtrơnkhôngviền nhưplan. Đọcđược48px; chỉcầnvẽbảntrắngnếuchốtđồngbộglyphtrênnútvàng.
- Sound: `SharedUI/sound` làloa2sóng cóviềnđen, đọc48px. BattleModalView vàSharedSettings đangnạpnó. Cyan khácglyphtrắng nhưngkhôngbằngchứngcầnvẽlạivìđộrõ. Giữtrướckhiuserduyệtthay.
- Vibration: đúnglàđiệnthoại, khôngthấybellthaythế. Đây làquyếtđịnhngữnghĩa: chuôngdễhiểulànotification/sound. Chưagenerate; cầnuserchốtbiểutượngrungphùhợpfantasy.
- VICTORY/DEFEAT: KHÔNGTHIẾU. `ResultScreen/atlas.png` chứa2ribbon; ResultScreenArt.Get0/1 cắttheoRect, ResultPanel.BuildResult gánribbonvàchữđộng. KhônggenerateB vìplanmôtảcũ. MuốnđổiDefeat tím→slate làstylechangeoptionalkhôngphảibug.
- Home/Battle/Close/Upgrade: cóglyphđạtđểtáidùng, khôngcầntạothêm. KhácbiệtSharedUI/home lều (selectchapter) vớihomehouse (GoHome) cóthểhợplýnếukhácchứcnăng.
- Gold: walletdùngHomeChrome/coin; packagebag/chest trongShopPacks làhợplý. CoinRewardEffect cònSharedUI/gold(túi) trongluồngbaycoin: nênđồngbộđườngdẫnnếucầnsửarồivìcùngvai tròcoinfeedback, khôngcầngenerate.
- Key: SharedUI/key3lỗ vớikeyHomeChrome1lỗlà2designđãcó; nếucùngcurrencythìưu tiênHomeChromekeyđúngbộShopPacksđãchốt. Sửađườngdẫnmãsauuserduyệt, khôngvẽthêm.

Ảnh:042-assets-audit.png (112px+48px),042-existing-result-banners.png. Scripttools/audit_ui_assets.pyđọccropchỉđểreview, khôngghiasset/meta/runtime.
Kếtluận: chưacónhómartmớinàocầnbắtbuộcgenerate. Ưutiêntáidùngreplay,vàreviewvibration; Resume/Soundđọcđược, titlebattleđãcó. KhôngthaybộHomeđãchốt.
