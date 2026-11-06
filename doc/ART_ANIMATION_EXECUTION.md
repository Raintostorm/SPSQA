# Art và hoạt ảnh UI — kế hoạch thực thi

Ngày: 2026-11-05 · Codex · Trạng thái: kế hoạch, CHƯA triển khai.
Đọc cùng `chat/PLAN_TOAN_DIEN.md`, `chat/PLAN_CHO_CHATGPT.md`, `doc/COORDINATION.md`.
Đây là kế hoạch triển khai bổ sung, không thay luật gameplay hoặc tự ghi các task Claude thành DONE.

## 1. Đích hoàn thành

Bàn giao một bộ asset tách lớp đã được duyệt, importer chạy lại được, demo chuyển động kiểm được,
và các luồng Summon / relic / upgrade / merge / kết quả dùng trong Unity mà không đổi kết quả kinh tế.
Chỉ gọi xong khi đủ: art đạt → đường dẫn đúng → chuyển động đúng → hành vi đúng → có bằng chứng Unity.
Ảnh nháp, compile sạch và video/demo trình duyệt là ba mức bằng chứng khác nhau; không thay cho nhau.

## 2. Quyết định đã chốt — không hỏi lại

- Contribution: biểu đồ 3 cột bạc trắng, viền đen.
- Summon: ẩn tỉ lệ trên màn chính; hiện bộ đếm bảo hiểm theo trạng thái thật.
- Buff: tối đa 3 nhóm Mythic trong thẻ, +N mở danh sách nhóm còn lại.
- Defeat: xám đá #4A5570.
- Summon: cầu sáng rồi bung tia; quả cầu không vỡ thành mảnh, không đổi sang cổng phép.
- Vibration: giữ điện thoại rung.
- Art mới cần user duyệt trước cắt/gắn. Những bản đã duyệt không hỏi lại.

## 3. Những gì thực tế đang có

| Phần | Hiện trạng | Khoảng cách cần xử lý |
|---|---|---|
| D1 | `chat/check/049-cleanup.md`, 47 asset; 31 có dòng mã ứng viên; 0 GUID serialized | Đây là phép dò bảo thủ, chưa phải danh sách được phép dọn. Còn helper/dynamic load và tools |
| G1 | 054-g1-glyphs + bản thử nền sáng; ribbon trong nửa dưới 054-g1-banner-source | Chờ duyệt; bỏ hai đế icon sai trong bản đầu; ribbon cần fit đúng ô atlas |
| A1 | 051-card-back, 051-light-layers-v2, 051-burst-v2; orb hiện tại có sẵn | Chưa normalize; quầng cần kiểm cắt cụt/leak, chuẩn RGB trắng và tâm frame |
| A2 | 052-chest-anim: closed guide + body + lid + shard | Chưa chứng minh khép nắp khớp; light bên phải sát mép, dùng bản v2 thay |
| A3 | Chưa có bộ mới | Soát hiệu ứng hiện có rồi chỉ tạo thứ còn thiếu |
| Hạ tầng | Có AnimatedCounter, MergeReveal, CoinRewardEffect | Không dựng bản song song trùng chức năng |
| Hạ tầng UI chung | Chưa thấy UiTween trong lần kiểm này | Claude xác minh lại khi nhận, rồi bổ sung nếu chưa có |
| Summon | TryRoll cấp và save trước SummonResultsView.Show | Hoạt ảnh chỉ trình bày dữ liệu đã quyết định |
| Relic | OpenEveryKey gọi GameProgress.TryOpenChest | Không gọi giao dịch thêm trong animation / callback / Skip |

## 4. Ownership và giới hạn

**Codex:** nguồn art, kiểm alpha/crop/pivot, importer, manifest, demo trình duyệt độc lập,
kiểm asset và báo cáo. Không sửa `.cs` theo hợp đồng mới, không dọn asset bằng suy đoán.
**Claude:** UiTween và mọi `.cs`, nối resource, lifecycle, giao dịch và tích hợp UI.
**User:** duyệt hình/chuyển động, phản hồi máy thật. Codex/Claude có thể hỗ trợ chụp Unity khi điều khiển được.
Đây là phân công đề xuất theo plan đã có; không tự tuyên bố Claude đã nhận hoặc đã bắt đầu.
Mỗi bên kiểm claim mới trước sửa. Chỉ một bên điều khiển Unity tại một thời điểm.
Không đổi rates, giá, pity, shard20, relic1shard/roll, skill hay test fixture trong công việc này.

## 5. Các lô triển khai và thứ tự

### P0 — Chốt đầu vào và khóa phạm vi
Owner: Codex. Đọc lại claims/code hiện tại, lập manifest asset nào được duyệt / chưa duyệt / bỏ.
Không tiếp tục generate đại trà khi chưa biết chính xác lỗi nào cần sửa.
Output: trạng thái từng asset và quyết định Q1–Q5 trong chat; task Claude vẫn giữ trạng thái thực.
Đạt khi không còn yêu cầu tạo ảnh đã có, không nhầm preview với runtime.

### P1 — G1: icon và ribbon
Owner: Codex sau duyệt art; Claude nối code.
1. Contribution256², sort150²: trim theo alpha, pad đều, giữ outline không cụt ở48px.
2. Sort là một mũi tên lên; code xoay180° cho xuống, không tạo hai bản dễ lệch.
3. Ribbon: backup atlas + meta; thay DUY NHẤT rect Defeat Unity(793,669,707,216).
   Convert top-left bằng chiều cao atlas, giữ nguyên các pixel ngoài rect.
4. Không copy `.meta` mới đè GUID cũ. Viết kiểm ảnh ngoài rect có hash không đổi.
5. Claude đổi Contribution/sort callsite, đồng thời tái dùng glyph có sẵn cho HomeMenu/MythicNext,
   Leave/Home/Close và coin Sell theo047. Không generate lại glyph đã có.
Output: importerG1 + manifest + preview48px/nền sáng-tối + diff atlas.
Đạt khi 2 glyph đọc rõ48px, không đế vuông/glow; Victory và phần khác atlas không đổi;
chữ kết quả vẫn do Unity vẽ, không nằm trong PNG.

### P2 — A1/A2: chuẩn hóa asset và chứng minh ghép lớp
Owner: Codex. Chỉ cắt các bản user đã duyệt.
- Đích Summon: orb-core512² (tái dùng), orb-glow512², rarity-ray512², sparkle64²,
  burst-0..5 mỗi512², card-back400×560.
- Đích Chest: body512², lid512×256, light512², shard96².
- Ghi JSON: source/crop rect, alpha bbox, canvas, visible bounds, pivot, scale, attachment point.
- Glow/ray/burst: RGB mọi pixel visible là255/255/255; chỉ alpha mang sáng/tối.
  Không dùng xóa màu đen toàn ảnh, vì sẽ mất outline của vật thể.
- PNG có alpha thật: xem trên navy, nền sáng và checker; không rectangle/halo bẩn.
- Burst không auto-fit từng frame cho lớn bằng nhau: giữ một hệ tọa độ và tâm để nhìn được nở rồi tan.
  Nếu sheet lệch tâm/khe nhỏ, xác định bbox và tâm từng pha, kiểm loop ở tốc độ thật rồi mới xuất.
- Quầng không được đụng cạnh canvas với alpha đáng kể. Mép có alpha>5/255 là dấu hiệu cần pad/cắt lại;
  không âm thầm trim mạnh làm quầng cụt. Đây là ngưỡng QA, không phải yêu cầu alpha mọi pixel bằng0.
- Chest: dùng CÙNG scale cho nắp/thân; attachment points trên hai cạnh miệng tương ứng.
  Dựng ảnh closed overlay và kiểm ở kích thước hiển thị; không có khe sáng >2px hoặc lệch mép >2px
  tại preview512px (ngưỡng mục tiêu, chưa đạt thì sửa).
- Nếu bản exploded052 không khớp hình học, không ép bằng scale méo: dựng lại từ closed master,
  phần mở ra cần bổ sung đáy nắp/miệng rương đúng góc; tạo lại đúng lớp cần thiết.
- Pivot lid dùng điểm bản lề đã đo, không mặc định center. Cardflip pivotcenter.
Output: import_anim_layers.py (tên đề xuất), manifest, contact sheet, closed chest proof.
Đạt khi ảnh sạch, góc nhìn nhất quán, nắp khép đúng, tất cả path/cỡ/pivot đã được ghi.

### P3 — Demo chuyển động ngoài game
Owner: Codex, không ghi đè runtime/C#; tái dùng asset P2.
Trang demo có2mục Summon/Chest, nút Replay, Skip, speed0.5x/1x/2x;
đổi nền sáng/tối và hiển thị tùy chọn bounds/pivot để bắt lỗi. Không có roll ngẫu nhiên hay cấp vật phẩm.
Các kết quả hiếm là mẫu có nhãn DEMO; chưa lộ màu hiếm trước lúc thẻ lật.
Chụp đầu/đỉnh/hoàn tất, kiểm Replay10lần không còn lớp cũ.
Đạt khi user duyệt nhịp ở1x, không thấy giật do registration, không đổi silhouette mỗi frame.
Demo web chỉ kiểm mỹ thuật và timing, không chứng minh integration Unity.

### P4 — Claude: hạ tầng ANIM-001/006
Có thể bắt đầu độc lập khi P1/P2 đang chuẩn bị, sau khi Claude tự claim.
Một UiTween dùng chung cho scale/position/alpha/number, easing, complete/cancel.
- UI dùng unscaledDeltaTime; speedtrận x1/x2/x3 không đổi thời gian popup/Summon.
- AnimatedCounter hiện dùng Time.deltaTime và gate SharedSettings.IsPaused.
  Phải phân biệt counterUI với VFXworld: không đổi toàn bộ sang unscaled làm phá pause combat.
- MergeReveal là worldeffect, hiện dùngTime.deltaTime, giữ pause/tốc độ trận đúng chủ đích.
- Popup0.92→1 trong0.16–0.22s EaseOutCubic, không overshoot khiến vượt safearea.
- Chuyển màn fade tổng0.20–0.35s, khóa input trong đoạn chuyển, không để overlay giữ raycast sau kết thúc.
- Cancel/OnDisable/OnDestroy trả alpha/scale/input về đúng trạng thái; bỏ coroutine/tween cũ.
Đạt khi không tạo animation layer trên mỗi Update, không callback2lần, không treo UI khi rời màn.

### P5 — Claude: Summon ANIM-002
Nhịp KHỞI ĐIỂM để review, không phải thông số bắt buộc nếu user đổi:
| Giai đoạn | Thời lượng mục tiêu | Cách chuyển |
|---|---:|---|
| Cầu nén/phồng nhẹ | 0.25s | scale1→0.96→1.04, easing mềm |
| Tích sáng | 0.30s | alpha/scale glow tăng liên tục |
| Bung tia | 0.12–0.18s | burst6pha, glow giảm, không phá quả cầu |
| Lật thẻ đầu | 0.22–0.28s | scaleX1→0→1, đổi mặt ở điểm hẹp nhất |
| ×10 thẻ sau | cách0.07–0.10s | nhịp so le, không kéo thành10lần animation đầy đủ |
| Giữ kết quả | do user | nút Continue rõ, không auto-close |
- TryRoll đúng1lần mỗi click hợp lệ; kết quả cấp/save trước animation như hiện tại.
- Từ bấm đến popup: khóa gọi lại trước cả đoạn prelude, không chỉ chặn khi ResultsView đã tạo.
- Rare glow/màu/thanh âm chỉ xuất hiện SAU mặt trước của chính thẻ được mở.
- Skip kết thúc presentation, hiển thị đủ kết quả; không reroll, không charge/grant lại.
- Pitycounter đọc save thực; không fake số, không hiện rate panel.
- MythicStone dùng20 như runtime; không lộ kết quả qua prelude.
Đạt: ×1/×10,thiếugems,catalogthiếu,Skipđầu/giữa/cuối,close/rời màn đều không mất hoặc nhân đôi thưởng.

### P6 — Claude: relic ANIM-003
- Một OpenAll = một kết quả giao dịch; không animate riêng mỗi key khi nhiều keys.
- Rung thân±2°/dịch nhẹ0.18s → xoay nắp mở0.25s → light tăng0.15s → shard bay0.35–0.50s.
- Thân không trôi; lid bám hinge; không scale cả rương để giả mở nắp.
- Giới hạn8–12shardvisual cho một chuỗi, con số thật không bị cap. Gộp kết quả theo loại khi cần.
- Nguồn dữ liệu dùng TryOpenChest đã có, không tự xây lại giao dịch.
- Nếuảnhthiếu, hiện fallback sạch/skipanimation; tuyệt đối không tạo ô trắng.
Đạt:0key/1key/nhiềukey,kho/catalognull,theotenrollfloorhiệncó,Skip/reopenkhônggrantlại.

### P7 — A3 + Claude ANIM-004/005
Codex soát sẵn có → demo những lớp còn thiếu: flash4pha256²trắng, arrow256²vàng,
merge-swirl512²trắng; tránh thay nhân vật/anim unit trong công việc UI này.
Sau duyệt cắt bằng importer, Claude dùng chung sparkle/ray khi hợp lý.
- Upgrade: giao dịch thành công rồi mới loé/counter; thiếu tài nguyên không phát FX thành công.
- Shard track hiển thị pre→post có chủ đích, cuối cùng đúng save; không cố chạy đầy nếu dữ liệu không đủ.
- Merge: consume đúng3ally theo luật hiện tại; art chỉ minh họa. Không chặn inputcombat quá lâu.
- Rewards: dùng Settlement đãcó, CoinRewardEffect đangdùngHomeChrome/coin;
  sốbay/sốvílàpresentation, khônggrantởOnComplete.
Đạt: gọi nhanh2lần,closegiữachừng,đủ/thiếugoldshard,MythicStone,maxLv30,thưởngđãnhận không cấp lại.

### P8 — QA, báo cáo, dọn cuối
Chạy compile + test transaction phù hợp thay đổi; Claude chọn test có ý nghĩa,
không viết test chỉ soi lại constants/layoutcùngimplementation.
D1 chạy lại SAU khi đổi code: kiểm exactpath, helpername/dynamicprefix, tấtcảserializedGUID,
Editor setup vàtools/import templates. Asset0/0 vẫn không tựdời khi còn tooldependency.
Dời asset được xác minh vào retired-date theo danh sách review riêng, không xóa hẳn.
Đạt khi report có screenshot/video thật + mức kiểmtra + tồnđọng, user chốt hình cuối.

## 6. Ma trận kiểm bắt buộc

| Nhóm | Tình huống | Bằng chứng / điều kiện |
|---|---|---|
| Nền art | navy/sáng/checker, cỡ48px/150px | không quầng/rectangle, đọc rõ |
| Chest | closed,25%,50%,100%open | hinge cốđịnh, không khe/lệch,típ không bịclip |
| Summon | ×1/×10, mỗi rarity vàstone bằng fixturekhôngđụngsavechính | khônglộraritytrướcflip, đủcount |
| Lifecycle | Replay10lần,Skip3thờiđiểm,Back/close/scenechange | khôngtweenrác/overlaychặninput/doublecallback |
| Time | TimeScale0/1/2/3, mànUIvàVFXtrận | UIđúngnhịp, worldpauseđúngluật |
| Safearea | iPhoneX1125×2436 + widescreenítcao hơn | chữ/nút khôngcắt, hìnhkhôngméo |
| Touch | nút>=44pt,scroll,itemdrag | overlaykhôngchặn thao tác sauanimation |
| Economy | đọc save trước/sau,Skip/reopen,failedrequests | charge/grant1lần, kếtquảkhôngđổi |
| Performance | cùngthiếtbị,cùngkịchbảncold/warm | ghiFPS/frame time/GC; chưađo thìkhôngkếtluận60fps |

Mục tiêu mượt: tween theo thời gian thay vì cộng khoảng cố định mỗi frame; easing liên tục;
tránh tạoTexture/Sprite/Resources.Load trong Update; cache asset; giới hạn VFX đồng thời.
Không cam kết60fps chỉ từ sốframe ảnh. Burst6pha ngắn đủ nếu phần lớn chuyển động được tween.
Mipmap/compression/filter chọn theo kíchthước thực, kiểm mép alpha trong Unity trước quyết;
không ép read/write texture cho asset không cần chỉnhCPU.

## 7. Giao theo lô — điểm dừng rõ ràng

| Lô | Codex | Claude (tự nhận) | Điểm review |
|---|---|---|---|
| I | P0/P1/P2: dọn manifest, cắt bản đãduyệt, proofalpha/hinge | P4 hạtầng | duyệt asset sạch + chestclosed |
| II | P3 demoSummon/Chest; A3preview | P5/P6 sauasset | duyệt chuyểnđộng,khôngtuyênbốUnitypassquaweb |
| III | cắtA3đãduyệt, soátassetpath | P7 + UIbuff+N,pity,icon047 | ảnhUnity và kiểmkinhtế |
| IV | P8 hỗtrợQA/artfixcụthể | integrationQA,dọnđãxácminh | checklist cuối và handoff |

Không cần user trả lời Q1–Q5 lần nữa. Bước bắtđầu là LôI, không generate thêm cho đến khi
đối chiếu duyệt các file051/052/054. Nếu art chỉ được duyệt một phần, triển khai phần đó
và tiếp tục phần độc lập; không tự coi imlặng là phêduyệt.

## 8. Hồ sơ bàn giao mỗi lô

Ghi vào chat/chatgpt.md và doc/MESSAGES.md:
- file nguồn → đầu ra, importer/crop/scale/pivot, hash/GUID khi ghiđè;
- gì đãkiểm tựđộng, gì đãxem demo, gì đãkiểmUnity;
- phần chờuser/Claude,không báo hoàntấtcảplan khi chỉxongảnh;
- claimđãrelease,khôngaiđang giữfilechungmà khôngowner.

Tài liệu doc/STATUS,WORK_QUEUE và UI_PLAN có mục cũ; Claude cần đồng bộ theo code + bằng chứng
khi nhận lô. Codex không tự ghi ANIM DONE khi chưa thấy code/test/QA thực tế.
