# Screen & roll audit — 2026-10-11

## Phạm vi và độ chắc chắn

Đối chiếu mã/scene của SpinsquadAgain, ảnh SpinSquad người dùng cung cấp, và bản Spin+Squad! 2.5.1 XAPK cục bộ. Đã quét 5.621 file assets/bin/Data của base_assets.apk, ghi 796 object khớp từ khóa vào `outdata/reference/spinsquad_ui_inventory.json`. Đây là **kiểm tra asset tĩnh, không phải đã chơi toàn bộ APK**. Prefab/clip có mặt không có nghĩa màn tương ứng đang mở được; tính năng tải từ server có thể không nằm trong gói này. Không nhập art của 111% vào game ta.

Nguồn ngoài chỉ để kiểm chứng phạm vi tính năng, không dùng khẳng định layout/timing của APK 2.5.1: [Google Play của 111%](https://play.google.com/store/apps/details?id=com.percent.aos.luckyrolling), [trang hỗ trợ chính thức](https://111percent.helpshift.com/hc/en/25-spin-squad/). Bản store hiện tại có thể khác APK đang tham chiếu.

## Những màn ta ĐÃ CÓ — không xây lại từ đầu

| Màn / lớp UI | Hiện trạng trong game ta | Việc còn thiếu |
| --- | --- | --- |
| Home | Tài nguyên, Chapter, Start, Upgrade, Relics, 3 lính idle | Điều hướng nhất quán; vùng tài nguyên có tác vụ; nền hợp art mới |
| Choose Level | Overlay tạo bởi HomeScreen; chọn 1.1/1.2/1.3 | Thể hiện rõ đang chọn/đã qua/khóa; thông tin màn và phần thưởng. Chapter 2/3 để sau theo yêu cầu |
| Prep + Battle | Cùng CharacterLab: xếp/drag lính, roll, buff, xuất quân, tốc độ, thống kê | Pause/settings; skill Legend; hiệu ứng trao thưởng và chuyển pha |
| Upgrade | Danh sách nhân vật và popup chi tiết | Phân loại/tìm nhân vật; mô tả skill; trạng thái khóa/đủ tài nguyên; animation tăng cấp |
| Relics | Scene tên nội bộ Treasure; 30 món, cuộn, chi tiết, x1/x10/x100, bảo hiểm | Luồng mở rương và reveal kết quả, xem đủ nhiều món, trạng thái NEW/duplicate rõ ràng |
| Result | Overlay thắng/thua, thưởng và Home/chơi tiếp | Trình bày phần thưởng, hiệu ứng cộng tài nguyên, trạng thái màn tiếp theo; dịch tiếng Anh |
| Unit/cell detail | CellPanel có chỉ số, Sell/Evolve | Feedback bán/tiến hóa; tooltip và xác nhận hành động có mất tài nguyên |

Không đếm Choose Level/Result/popup chi tiết là scene mới bắt buộc. Overlay là đủ nếu điều hướng đúng. Scene được đăng ký hiện có đúng 4: Home, CharacterLab, Upgrade, Treasure.

## Screens / overlays còn thiếu hoặc chưa đầy đủ

| Ưu tiên | Mục | Bằng chứng tham chiếu | Đánh giá / phạm vi đề nghị |
| --- | --- | --- | --- |
| P0 | Settings + Pause + xác nhận rời trận | Game ta không có UI/controller tương ứng; đây là đề xuất sử dụng, chưa xác minh layout APK | Âm thanh, tốc độ, resume/restart/exit; popup xác nhận ngắn |
| P0 | Relics roll result / reward reveal | APK có GetRewardUIEffect, LobbyItemGetAction_Start/End; TreasureScreen chỉ đổi sprite rương và in summary | Overlay cho x1; danh sách cuộn kết quả cho x10/x100; Close/Skip/Roll Again |
| P0 | Tutorial / first-run hướng dẫn roll–xếp–evolve–start | Không tìm thấy hệ hướng dẫn trong mã ta; đề xuất, chưa chứng minh màn APK | 4–5 bước ngắn, lưu trạng thái đã xem; không thêm scene dài |
| P1 | Collection / character detail hoàn chỉnh | Ảnh tham chiếu có thanh điều hướng; APK có nhiều UI nhân vật và EquipIconWearing/Merge; chưa xác minh màn collection riêng | Mở rộng Upgrade đang có thay vì tạo 2 danh sách trùng nhau |
| P1 | Shop | APK có PackageShopView, EventShopProductView, Weekly/MonthlyShopPackage; ta có bg_shop nhưng không có Shop scene/controller | Bản offline: đổi vàng/chìa trước; không tự thêm IAP/quảng cáo |
| P1 | Thiếu tiền/chìa + chỉ đường kiếm tài nguyên | Ảnh tham chiếu có nút + ở thanh tài nguyên; ta chỉ báo text thiếu | Popup gọn, chỉ đường tới Shop/Reward; không trừ/roll khi không đủ |
| P1 | Profile/account/settings menu | Ảnh tham chiếu có avatar/cấp và menu; APK có TitleUILoginAuto/Manual, TitleUIAppSync/GameSync | Profile local trước. Đăng nhập/cloud save cần thiết kế backend riêng |
| P2 | Daily Quest / claim reward | APK có DailyQuestRoot, Quest1..5 | Chưa có hệ nhiệm vụ, không chỉ thiếu ảnh màn; để sau core loop |
| P2 | Battle Pass / season rewards | APK có PassExpRoot/PassExp1..5 | Chỉ chứng minh asset hệ pass, không chứng minh màn/luật; chưa cần làm cho MVP |
| P2 | Mode / challenge / ranking | Ảnh tham chiếu có Mode; APK có ChallengRankingPointRoot và UI_Rank_1..5 | Không tạo nút chết. Cần có gameplay mode thật rồi mới làm màn |
| P2 | Energy + gem economy | Ảnh tham chiếu có energy/gold/gem; APK reward có StaminaRoot/DiaRoot | Ta đang dùng Gold/Keys có chủ ý. Đây là lựa chọn kinh tế, không tự gọi là bug hay thêm vào |
| P2 | Title/loading + transition | APK có SplashScene, TitleScene, SceneTransitionManager | Local game có thể dùng overlay fade/loading; chưa cần login pipeline của game online |

Không đủ bằng chứng để khẳng định APK này có Guild, Mail, PvP, revive, offline reward hoặc mọi màn event; không đưa chúng vào danh sách “thiếu chắc chắn”.

## Animation roll: có gì, thiếu gì

### Battle roll hiện tại

`RollPanel.SpinThenReveal`: 6 ô đổi icon mỗi 0,05s; dừng từ trái sang cách nhau 0,13s; ô thắng đổi khung, ô thua alpha 0,45; giữ bảng rồi fade. Vì vậy **ta không thiếu hoàn toàn animation roll**.

| Ưu tiên | Phần còn thiếu | Việc nên làm |
| --- | --- | --- |
| P0 | Đồng bộ chốt kết quả và trao thưởng | RollService cộng coin/buff ngay; Rolled cũng làm RollRewardPlacer spawn quân ngay, trong lúc panel còn quay. Tách kết quả đã quyết khỏi nhịp trình diễn; không quyết RNG lại khi reveal |
| P0 | Feedback thưởng tới đích | Coin bay về thanh vàng; buff về icon chỉ số; quân xuất hiện bằng pop nhỏ đúng ô. APK có GetStatUIEffect/GetRewardUIEffect/BattleRewardEffect, nhưng chưa đo đường bay/timing |
| P1 | Ô dừng có nhịp rõ | Ease-out hoặc nảy rất nhỏ, nhấn ô thắng; tránh chớp/ngẫu nhiên cả bảng sau khi chốt |
| P1 | SFX/haptic và khóa thao tác hợp lý | Tick khi quay, click khi dừng, cue cho phần thưởng; khóa roll trong reveal hoặc định nghĩa queue/skip rõ. Ta hiện cắt coroutine cũ khi roll mới |
| P1 | Hiếm / tiến hóa / thất bại | Reveal theo rarity, hiệu ứng evolve; không dùng cùng một hiệu ứng cho mọi kết quả. APK có EquipIconMerge 0,25s và Fx_UI_PickUp_Fail, nhưng tên không chứng minh đây chính là roll quân |
| P2 | Reel trượt dọc thay vì thay icon | Là lựa chọn art/motion nâng chất lượng, không khẳng định APK dùng reel. Cần nhìn video gameplay để chốt chính xác |

### Relics roll hiện tại

`TreasureScreen.DoRoll → Report → Refresh` chạy tức thì. `Report` đổi rương sang ảnh mở, đếm rarity và chỉ ghi tên món hiếm nhất. Không có coroutine mở rương, không có chuỗi reveal từng món hoặc trang kết quả x10/x100.

Thứ tự đề nghị: đóng → rung nhẹ → mở → hiện icon/rarity → ghi NEW hoặc +level → Collect/Close. Bulk roll có Skip và danh sách đầy đủ; kết quả RNG/save chỉ ghi một lần dù Skip/Close.

APK có `LobbyItemGetAction_Start` 0,6667s và `LobbyItemGetAction_End` 0,1333s. Có thể dùng làm bằng chứng một nhịp nhận vật phẩm chia đầu/cuối, **không gán chúng chắc chắn cho rương Relics khi chưa kiểm gameplay**.

## Ngoài screen: thiếu hụt quan trọng phát hiện khi thay model

- Skill của 5 Legend đã nhập vào UnitAnimationSet.Skill, nhưng IUnitVisual/FrameUnitVisual/UnitBrain chưa có đường gọi PlaySkill. Clip chưa đồng nghĩa skill đã chạy. Chưa tự thêm cooldown/damage/shield ngoài yêu cầu.
- UI battle/result/Relics còn nhiều text tiếng Việt, dù Home đã tiếng Anh.
- Machine Epic giữ bản cũ, vì người dùng đã yêu cầu để sau.
- Bộ nền vẽ nhiều chi tiết và khung/card cũ chưa đồng bộ với nút và nhân vật mới. Cần một pass chung, không sinh mỗi màn một style.

## Thứ tự làm, tránh mở quá nhiều việc

1. Test 19 nhân vật đã thay trong trận; nối skill theo thiết kế đã duyệt, xử lý Machine Epic riêng.
2. Đồng bộ battle roll → reveal → thưởng vào đúng đích.
3. Làm Relics chest + result x1/x10/x100 + Skip.
4. Hoàn thiện Result/Choose Level + tiếng Anh + Settings/Pause/tutorial.
5. Collection/Shop; quest/pass/mode/account để sau.
