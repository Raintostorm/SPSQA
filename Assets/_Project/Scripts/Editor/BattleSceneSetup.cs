using Project.Battle;
using Project.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Project.EditorTools
{
    /// <summary>
    /// Dựng cả bộ khung thử trận bằng một lệnh: hai lưới, ô lưới, Spawner,
    /// và nối hết tham chiếu.
    ///
    /// Vì sao là Editor script chứ không phải code chạy lúc Play: scene phải
    /// NHÌN THẤY ĐƯỢC khi không Play, và sửa được bằng tay sau đó. Dựng bằng
    /// code lúc chạy thì mở scene lên chỉ thấy một GameObject rỗng.
    ///
    /// Chạy lại được nhiều lần — tìm thấy thì sửa, không thấy thì tạo mới.
    /// </summary>
    public static class BattleSceneSetup
    {
        const string UnitPrefab = "Assets/_Project/Prefabs/Units/Unit.prefab";
        const string BalanceAsset = "Assets/_Project/Data/GameBalance.asset";
        const string UnitDir   = "Assets/_Project/Data/Units";
        // Hai phe dung CHUNG mot roster 20 con. He truoc phai tach thu muc
        // vi dich co art rieng; gio ca 20 con deu dung duoc cho ca hai ben,
        // va phe la thuoc tinh cua con linh tren san, khong phai cua dinh nghia.

        /// <summary>
        /// Mỗi wave trôi qua thì địch đông thêm bấy nhiêu con.
        ///
        /// Level 1 có 3 con địch ở wave 1, nên 1 ở đây cho wave 2 bốn con.
        /// </summary>
        const int ExtraEnemiesPerWave = 1;
        // Sinh boi tools/make_road_bg.py — con duong ve san trong anh, khong
        // phai sprite roi dan len. Vi tri dai duong tinh tu BattleLane.laneY
        // va ortho ben duoi, nen sua mot trong hai thi phai chay lai script.
        // Hai ảnh nền, đổi qua lại theo giai đoạn — xem BattleBackground.
        const string PrepBg   = "Assets/_Project/Art/Backgrounds/bg_prep_portrait.png";
        const string BattleBg = "Assets/_Project/Art/Backgrounds/bg_battle_portrait.png";
        const string TileDir  = "Assets/_Project/Art/UI/";

        /// <summary>
        /// Màu hai dải ven màn khi cửa sổ không phải 16:9.
        ///
        /// Xanh rêu đậm, KHÔNG phải gần đen: 0.07 đo ra màn là (18,29,18) và
        /// mắt đọc thẳng ra "đen". Phải lên quãng 0.3 mới tách hẳn.
        /// </summary>
        static readonly Color BarColor = new Color(0.30f, 0.40f, 0.25f);

        // ── bố cục đo được từ hai ảnh ─────────────────────────────────
        //
        // Tất cả là TỈ LỆ, không phải pixel: ảnh được phóng cho phủ kín khung
        // nhìn, nên pixel chẳng nói lên gì. Nhân với kích thước đã phóng
        // (BattleBackground.CoverSize) mới ra toạ độ thế giới.
        //
        // Bốn số lưới là ĐỊNH NGHĨA, không phải số đo. Ảnh nền KHÔNG có ô
        // nào vẽ sẵn — 32 ô là sprite rời do CellTiles đặt vào đúng lattice
        // này, nên không có chuyện ô vẽ lệch ô logic.
        //
        // Mẫu số là 1242 / 2796, cỡ của artsrc/portrait/bg_prep_portrait.png.
        //
        // Chọn ra bằng một phép quét vét cạn trên chính tấm nền: tìm bộ cho
        // DIỆN TÍCH Ô LỚN NHẤT mà không ô nào chạm vào cây, đá hay hàng rào,
        // dưới ba ràng buộc —
        //   • lưới nằm trong y 180..920  (ngoài dải HUD trên và nút ROLL dưới)
        //   • tỉ lệ ô lệch tối đa 8% so với ảnh ô đất (1.026)
        //   • không ô nào có quá 0.2% điểm tối
        // Kết quả: ô 142×129, kéo 7.2% so với ảnh ô, rộng hơn bộ trước 11%.

        // MAN DOC: MOT luoi 4x4 duy nhat, chiem het vung giua. Hai ban
        // Grid_Ally va Grid_Enemy nam CHUNG mot cho, bat tat qua lai bang
        // nut XEM DICH — nen khong con "mep trong" chia trai phai nua.
        //
        // Do tren khung 1242x2796. Luoi y 774..1883, bon cot nam giua.

        /// <summary>Bước giữa hai cột, theo tỉ lệ bề ngang ảnh.</summary>
        const float GridPitchXFrac = 286f / 1242f;

        /// <summary>Tâm hàng TRÊN CÙNG, theo tỉ lệ chiều cao, đo từ đỉnh ảnh.</summary>
        const float GridRow0Frac = 913f / 2796f;

        /// <summary>Bước giữa hai hàng, theo tỉ lệ chiều cao ảnh.</summary>
        const float GridPitchYFrac = 277f / 2796f;

        /// <summary>
        /// Mép mặt đường trong ảnh giao tranh, tỉ lệ tính từ đỉnh.
        /// Đo được trên bg_battle_portrait: y 643..1930 trên 2796.
        /// </summary>
        const float RoadTopFrac = 643f / 2796f;
        const float RoadBotFrac = 1930f / 2796f;

        // ── đội hình ba con trong một ô ───────────────────────────────
        //
        // Đo được: lính cao 0.589, rộng 0.424 đơn vị (sprite 131×165 ở PPU
        // 201, prefab scale 0.65). Ô rộng 1.033, cao 0.732.
        //
        // Một con VỪA ô (thấp hơn 20%). Chỗ vỡ là đội hình: giãn đều hai trục
        // thì cụm ba con cao 0.776 — quá ô 6% — mà chỉ dùng 0.696 trên 1.033
        // bề ngang, phí một phần ba. Nên tách hai trục ra.
        //
        // Hai số dưới là PHẦN của ô, chọn sao cho cụm ba con vừa khít:
        // Ô đã nới lên 1.109 × 0.818 và lính lên scale 0.75 (cao 0.680):
        //   ngang: 0.476 + 2 × (1.109 × 0.26)  = 1.066  <=  1.109
        //   dọc  : 0.680 + 2 × (0.818 × 0.080) = 0.811  <=  0.818
        /// <summary>
        /// Thu nhỏ mọi con lính. Gán vào UnitDefinition.Scale, không đụng art.
        ///
        /// Vì sao cần: lính rộng trung bình 1.09 đơn vị, con rộng nhất 1.42 —
        /// mà ô đất chỉ rộng 1.19. MỘT con đã gần kín ô, nên cụm ba con tràn
        /// ra ngoài 0.28 mỗi bên. 0.85 kéo mức tràn xuống 0.10.
        ///
        /// Không thể hết tràn: muốn vừa hẳn phải xuống 0.70, lúc đó lính nhỏ
        /// tới mức mất chi tiết. Con máy thì tràn ở mọi mức vì tự nó đã rộng
        /// hơn ô — chỗ đó phải vẽ lại art, không chỉnh số được.
        /// </summary>
        public const float UnitScale = 0.85f;

        /// <summary>Chiều cao lính ở scale 1, đo trên cả 20 con.</summary>
        const float AvgUnitHeight = 0.92f;

        // Giãn ngang HẸP lại, giãn dọc RỘNG ra. Chồng ngang nhiều hơn nhưng
        // tách theo chiều sâu nhiều hơn — mà SortingOrderFor vẽ con đứng
        // thấp đè lên con đứng cao, nên ba con vẫn đọc ra ba con.
        // 0.12 chu khong 0.18: long o dat chi rong 1.086 don vi (buoc luoi
        // 1.267, tru CellTiles.fill 0.94 va vien da 4.4% moi ben). Buoc xep
        // rong 0.18 an mat 0.228 trong so do, con lai 0.858 cho mot con —
        // hep toi muc 12/20 con phai thu nho qua muc moi vua.
        // PHAI khop SPREAD_X trong tools/fit_cell_scale.py.
        const float SpreadXFrac = 0.12f;
        const float SpreadYFrac = 0.13f;

        /// <summary>
        /// Chân lính đứng ở đâu trong bề dày mặt đường, 0 = mép trên.
        /// 0.45 tức hơi trên giữa một chút — để mặt đường còn trải ra phía
        /// trước mặt lính, nhìn mới ra "đang đứng trên đường" chứ không phải
        /// "đang đứng ở mép đường".
        /// </summary>
        const float FeetDownRoad = 0.45f;

        // Không còn BattleZoom. Ảnh giao tranh cũ là 2.67:1 với mặt đường
        // chỉ chiếm 24% chiều cao nên phải phóng 1.5 để che bớt cỏ thừa.
        // Ảnh dọc vẽ sẵn mặt đường — đo được y 643..1930 trên 2796 — nên
        // phóng là thừa.

        /// <summary>
        /// Tô tối nền để lính nổi lên. Không sửa file ảnh — chỉ là màu nhân
        /// trên SpriteRenderer, chỉnh lại trong Inspector lúc nào cũng được.
        ///
        /// Đã nhẹ tay hơn (0.70 → 0.90): hồi đó nền là ảnh AI vẽ rậm nên phải
        /// dìm mạnh. Nền bây giờ tôi tự chọn màu, vốn đã nhạt sẵn — nhân thêm
        /// 0.70 nữa thì cỏ xám hết, mà lính cũng không nổi hơn.
        /// </summary>
        static readonly Color BgTint = new Color(0.90f, 0.92f, 0.96f);

        /// <summary>
        /// Bố cục bàn đấu. Hai phe xếp đâu là ràng buộc của TỈ LỆ MÀN HÌNH,
        /// không phải gu: màn dọc chỉ rộng 4.62 đơn vị, hai lưới cạnh nhau
        /// cần 11 — không vừa. Màn ngang rộng 14.22 thì vừa thoải mái.
        /// </summary>
        enum Layout { LandscapeSideBySide, PortraitStacked }

        [MenuItem("SpinSquad/Dựng scene — màn NGANG (hai phe trái–phải)")]
        static void SetupLandscape() => Setup(Layout.LandscapeSideBySide);

        [MenuItem("SpinSquad/Dựng scene — màn DỌC (hai phe trên–dưới)")]
        internal static void SetupPortrait() => Setup(Layout.PortraitStacked);

        static void Setup(Layout layout)
        {
            // Chieu cao khung nhin SUY RA tu be ngang co dinh, khong go tay.
            // Dung dung phep tinh cua LetterboxCamera de scene dung luc chua
            // Play va luc dang Play khong lech nhau.
            var aspect = Mathf.Clamp(
                (float)Screen.width / Mathf.Max(1, Screen.height),
                Visual.LetterboxCamera.TallestAspect,
                Visual.LetterboxCamera.WidestAspect);
            var ortho = Visual.LetterboxCamera.DesignWidth / aspect * 0.5f;

            var balance  = Load<GameBalance>(BalanceAsset);
            var prefab   = Load<GameObject>(UnitPrefab);
            var roster = LoadAll(UnitDir);
            if (roster.Count == 0)
            {
                Debug.LogError($"[BattleSceneSetup] {UnitDir} rỗng. Chạy " +
                               $"python3 tools/import_units.py --write rồi " +
                               $"SpinSquad ▸ Nhập 20 nhân vật.");
                return;
            }
            var prepBg   = AssetDatabase.LoadAssetAtPath<Sprite>(PrepBg);
            var battleBg = AssetDatabase.LoadAssetAtPath<Sprite>(BattleBg);
            if (balance == null || prefab == null) return;
            if (prepBg == null) { Debug.LogError($"[BattleSceneSetup] Không thấy {PrepBg}"); return; }

            SetCamera(ortho);
            var prepSize = Visual.BattleBackground.FitDesign(prepBg, ortho);
            var cellW = prepSize.x * GridPitchXFrac;
            var cellH = prepSize.y * GridPitchYFrac;

            // Tâm ô (0,0) = hàng DƯỚI CÙNG, cột trái nhất. BattleGrid cho
            // row tăng thì y tăng, nên hàng 0 là hàng thấp nhất.
            var row0Y = prepSize.y * (0.5f - GridRow0Frac) - cellH * 3f;

            // MOT luoi duy nhat, nam giua. Hai ban CHUNG mot goc toa do —
            // chung khong bao gio hien cung luc, nut XEM DICH bat tat qua
            // lai. Nho vay o to gap doi so voi kieu chia doi trai-phai.
            var origin = new Vector2(-cellW * 1.5f, row0Y);

            MakeBackground(prepBg, battleBg, ortho);
            var allyBoard  = MakeBoard("Grid_Ally",  origin, cellW, cellH, balance);
            var enemyBoard = MakeBoard("Grid_Enemy", origin, cellW, cellH, balance);

            MakeCells(allyBoard,  "tile_ally",  "tile_ally_sel");
            MakeCells(enemyBoard, "tile_enemy", "tile_enemy_sel");

            // Hai ban nam chung mot cho nen BAT BUOC co cai nay, khong thi
            // hai doi hinh chong len nhau ngay tu khung hinh dau.
            var boards = Need<BoardView>(Find("Flow") ?? Create("Flow"));
            Set(boards, so =>
            {
                so.FindProperty("allyBoard").objectReferenceValue = allyBoard;
                so.FindProperty("enemyBoard").objectReferenceValue = enemyBoard;
            });

            var drag = AddDrag(allyBoard);
            var lane = MakeLane(battleBg, ortho);
            var spawner = MakeSpawner(prefab, allyBoard, enemyBoard, roster);
            var pool = MakeRoll(balance, allyBoard, spawner);
            var flow = MakeFlow(balance, allyBoard, enemyBoard, lane, drag);

            // Nối sau cùng: BattleSetup dựng trước Flow nên lúc đó chưa có
            // Flow để gán. Nó tự tìm được lúc chạy, nhưng gán sẵn thì nhìn
            // Inspector là thấy, đỡ phải đoán.
            var setup = Object.FindFirstObjectByType<BattleSetup>();
            if (setup != null)
                Set(setup, so => so.FindProperty("flow").objectReferenceValue = flow);

            BattleHudBuilder.Build(flow);
            WireCellPanel(balance, pool, spawner, allyBoard, enemyBoard, flow);
            RemoveLooseUnits();

            EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log($"[BattleSceneSetup] Xong. Ô {cellW:0.00}×{cellH:0.00} đơn vị, " +
                      $"gốc chung {origin}. " +
                      $"Bấm Play, rồi bấm nút XUẤT QUÂN (hoặc phím Space).");
        }

        // ── các bước ──────────────────────────────────────────────────

        /// <summary>
        /// Một SpriteRenderer duy nhất, hai ảnh, đổi qua lại theo giai đoạn.
        ///
        /// Cỡ ảnh do BattleBackground tự tính lúc chạy, không đặt cứng ở đây:
        /// hai ảnh khác tỉ lệ khung (16:9 và 2.67:1) nên hệ số phóng khác
        /// nhau — đặt một lần là ảnh thứ hai sai cỡ.
        /// </summary>
        static void MakeBackground(Sprite prep, Sprite battle, float ortho)
        {
            var go = Find("Background") ?? Create("Background");
            go.transform.position = new Vector3(0f, 0f, 1f);   // sau moi thu

            var sr = Need<SpriteRenderer>(go);
            Undo.RecordObject(sr, "Đặt nền");
            sr.sprite = prep;
            sr.color = BgTint;
            sr.sortingOrder = Battle.BattleGrid.BackgroundOrder;

            // Đặt sẵn cỡ cho ảnh chuẩn bị để Scene view nhìn đúng lúc chưa
            // Play. Vào Play thì BattleBackground đặt lại theo từng ảnh.
            var size = Visual.BattleBackground.FitDesign(prep, ortho);
            var b = prep.bounds.size;
            go.transform.localScale = new Vector3(size.x / b.x, size.y / b.y, 1f);
            EditorUtility.SetDirty(sr);

            var bg = Need<Visual.BattleBackground>(go);
            Set(bg, so =>
            {
                so.FindProperty("prepSprite").objectReferenceValue = prep;
                so.FindProperty("battleSprite").objectReferenceValue = battle;
                so.FindProperty("tint").colorValue = BgTint;
            });
        }

        /// <summary>
        /// Dựng 16 ô đất cho một bàn, bằng sprite rời.
        ///
        /// Vẽ sẵn vào ảnh nền thì rẻ hơn, nhưng ô không đổi được khi bấm
        /// chọn — muốn đổi thì phải có 16 ảnh nền khác nhau. Nên ô là sprite.
        ///
        /// Thiếu ảnh ô thì bỏ qua và kêu một tiếng, KHÔNG dừng cả hàm dựng:
        /// chưa có art thì phần còn lại của scene vẫn phải dựng được.
        /// </summary>
        static void MakeCells(BattleBoard board, string normal, string selected)
        {
            var a = AssetDatabase.LoadAssetAtPath<Sprite>(TileDir + normal + ".png");
            var b = AssetDatabase.LoadAssetAtPath<Sprite>(TileDir + selected + ".png");
            if (a == null || b == null)
            {
                Debug.LogWarning(
                    $"[BattleSceneSetup] Chưa có {normal}.png / {selected}.png trong " +
                    $"{TileDir} — bỏ qua ô đất. Chạy tools/import_tiles.py --write.");
                return;
            }

            var tiles = board.GetComponent<Visual.CellTiles>();
            if (tiles == null)
                tiles = Undo.AddComponent<Visual.CellTiles>(board.gameObject);

            // GHI số thứ tự vẽ, không trông vào mặc định trong CellTiles.cs.
            // Component đã nằm sẵn trong scene giữ giá trị CŨ đã serialize,
            // và giá trị đó thắng mặc định trong code. Đây là lần thứ ba
            // dự án này mắc đúng cái bẫy ấy (barColor, extraEnemiesPerWave).
            Set(tiles, so => so.FindProperty("sortingOrder").intValue =
                Battle.BattleGrid.TileOrder);

            tiles.Rebuild(a, b);
        }

        /// <summary>
        /// Camera phải khớp bố cục. Đổi lưới mà quên đổi camera thì lưới vẫn
        /// đúng toạ độ nhưng nằm ngoài khung nhìn — và không có gì báo.
        /// </summary>
        static void SetCamera(float orthoSize)
        {
            var cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("[BattleSceneSetup] Không thấy Main Camera.");
                return;
            }
            Undo.RecordObject(cam, "Đặt camera");
            Undo.RecordObject(cam.transform, "Đặt camera");
            cam.orthographic = true;
            cam.orthographicSize = orthoSize;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.backgroundColor = Color.black;

            // Mau hai dai ven man PHAI dat o day, khong the chi doi gia tri
            // mac dinh trong LetterboxCamera.cs: component da nam san trong
            // scene thi Unity doc gia tri DA LUU, khong doc mac dinh moi.
            // Da vap — doi mac dinh roi bao nguoi dung tu sua Inspector, va
            // tat nhien no van den.
            var lb = Need<Visual.LetterboxCamera>(cam.gameObject);
            Set(lb, so => so.FindProperty("barColor").colorValue = BarColor);
            EditorUtility.SetDirty(cam);
        }

        static BattleBoard MakeBoard(string name, Vector2 origin,
                                     float cellW, float cellH, GameBalance balance)
        {
            var go = Find(name) ?? Create(name);
            go.transform.position = Vector3.zero;   // Origin là nguồn sự thật DUY NHẤT

            var grid = Need<BattleGrid>(go);
            Set(grid, so =>
            {
                so.FindProperty("balance").objectReferenceValue = balance;
                so.FindProperty("origin").vector2Value = origin;
                so.FindProperty("cellSize").floatValue = cellW;
                so.FindProperty("cellHeight").floatValue = cellH;

                var spread = new Vector2(cellW * SpreadXFrac, cellH * SpreadYFrac);
                so.FindProperty("formationSpread").vector2Value = spread;

                // Neo sao cho CẢ CỤM nằm GIỮA ô theo chiều dọc.
                //
                // Bản cũ neo chân sát đáy ô. Ô ngang hồi đó cao 0.818 còn
                // lính cao 0.93 nên lính tràn lên trên — nhìn vẫn "đứng trên
                // ô". Ô dọc cao 1.227 mà lính chỉ cao 0.92, neo sát đáy thì
                // thừa hẳn một khoảng trống trên đầu, nhìn ra là lính tụt
                // xuống dưới ô.
                //
                // Neo chân ở nửa chiều cao lính. Tính từ CHIỀU CAO LÍNH chứ
                // không từ bề cao ô: đổi cỡ ô thì lính vẫn phải nằm giữa, mà
                // đổi cỡ lính thì neo phải theo. Lấy mốc là ô thì sai cả hai.
                //
                // Trừ thêm spread.y/3: FormationOffset cân đội hình theo TÂM
                // KHỐI, nhưng "vừa ô" là chuyện BAO HÌNH. Cụm 3 con có một
                // con trên hai con dưới, nên bao hình lệch lên 1/3 độ giãn so
                // với tâm khối. Không trừ thì đỉnh đầu thòi lên 0.027 trong
                // khi dưới chân còn thừa 0.080.
                // KHONG con bu gi o day nua. Hai phan truoc kia gop vao
                // mot con so chung gio ve dung cho cua chung:
                //   - "day than vao giua o" -> UnitDefinition.CellOffset,
                //     tinh rieng tung nhan vat
                //   - "cum 3 con lech len"  -> BattleGrid.Center tru tam
                //     KHUNG BAO cua doi hinh, tu dung cho moi so luong
                so.FindProperty("unitAnchor").vector2Value = Vector2.zero;
            });
            return Need<BattleBoard>(go);
        }

        /// <summary>Chỉ lưới phe mình mới kéo thả được.</summary>
        static UnitDragHandler AddDrag(BattleBoard board) =>
            Need<UnitDragHandler>(board.gameObject);

        /// <summary>
        /// Con đường giao tranh. Lính ra đây khi vào wave.
        ///
        /// laneY bám theo mặt đường VẼ TRONG ảnh giao tranh, không phải số
        /// chọn sẵn — đổi ảnh hay đổi camera thì nó tự theo.
        /// </summary>
        static BattleLane MakeLane(Sprite battleBg, float ortho)
        {
            var go = Find("Lane") ?? Create("Lane");
            go.transform.position = Vector3.zero;
            var lane = Need<BattleLane>(go);
            if (battleBg == null)
            {
                Debug.LogWarning($"[BattleSceneSetup] Không thấy {BattleBg}, " +
                                 $"giữ nguyên laneY đang có.");
                return lane;
            }

            // Ảnh đặt tâm ở y = 0. Tỉ lệ đo từ TRÊN xuống nên phải trừ dần.
            var h = Visual.BattleBackground.FitDesign(battleBg, ortho).y;
            var top = h * (0.5f - RoadTopFrac);
            var bottom = h * (0.5f - RoadBotFrac);
            var y = top - FeetDownRoad * (top - bottom);

            // Quan ta vao tu MEP TRAI di sang phai, dich tu MEP PHAI sang
            // trai. (Thu tu tha o thi nguoc: phe ta tha cot PHAI nhat truoc,
            // vi do la cot gan mat tran nhat — xem BattleFlow.Fill.)
            //
            // Diem vao nam NGOAI khung nhin mot chut (be ngang 5.5 nen nua la
            // 2.75) de linh di vao chu khong hien ra giua san.
            var entry = Visual.LetterboxCamera.DesignWidth * 0.5f + 0.7f;

            // Chia deu mat duong thanh 4 hang, dung HET be day chu khong
            // dồn vào giữa — man doc chi rong 5.5 don vi nen tach theo chieu
            // doc la cach duy nhat de doc ra tung con.
            var rows = 4;
            var spacing = (top - bottom) / (rows + 1f);

            Set(lane, so =>
            {
                so.FindProperty("laneY").floatValue = y;
                so.FindProperty("allyEntryX").floatValue  = -entry;
                so.FindProperty("enemyEntryX").floatValue =  entry;
                so.FindProperty("laneRows").intValue = rows;
                so.FindProperty("rowSpacing").floatValue = spacing;
                // Đặt lại từ lệnh dựng: scene cũ còn giữ 0.45 của bản thả lẻ.
                so.FindProperty("entrySpacing").floatValue = 0f;
            });
            Debug.Log($"[BattleSceneSetup] Mặt đường y {bottom:0.00}..{top:0.00}, laneY = {y:0.00}.");
            return lane;
        }

        /// <summary>
        /// Nối bảng thông tin ô. Phải chạy SAU BattleHudBuilder vì bảng do
        /// nó dựng ra.
        /// </summary>
        static void WireCellPanel(GameBalance balance, Gacha.UnitPool pool,
                                  UnitSpawner spawner, BattleBoard ally,
                                  BattleBoard enemy, BattleFlow flow)
        {
            // Nối những tham chiếu chỉ có SAU khi Flow và HUD đã dựng xong.
            var svc = Object.FindFirstObjectByType<Gacha.RollService>();
            if (svc != null)
                Set(svc, so => so.FindProperty("flow").objectReferenceValue = flow);

            var result = Object.FindFirstObjectByType<UI.ResultPanel>();
            if (result != null)
                Set(result, so => so.FindProperty("flow").objectReferenceValue = flow);

            var panel = Object.FindFirstObjectByType<UI.CellPanel>();
            if (panel == null) return;

            Set(panel, so =>
            {
                so.FindProperty("balance").objectReferenceValue = balance;
                so.FindProperty("pool").objectReferenceValue = pool;
                so.FindProperty("spawner").objectReferenceValue = spawner;
                so.FindProperty("roll").objectReferenceValue = svc;
            });

            var insp = Need<UI.CellInspector>(Find("Flow") ?? Create("Flow"));
            Set(insp, so =>
            {
                so.FindProperty("flow").objectReferenceValue = flow;
                so.FindProperty("allyBoard").objectReferenceValue = ally;
                so.FindProperty("enemyBoard").objectReferenceValue = enemy;
                so.FindProperty("panel").objectReferenceValue = panel;
            });
        }

        /// <summary>Ví coin, máy quay gacha, và chỗ đặt lính thắng được.</summary>
        static Gacha.UnitPool MakeRoll(GameBalance balance, BattleBoard allyBoard,
                                       UnitSpawner spawner)
        {
            var go = Find("Roll") ?? Create("Roll");
            var svc = Need<Gacha.RollService>(go);
            Set(svc, so =>
            {
                so.FindProperty("balance").objectReferenceValue = balance;
                so.FindProperty("treasures").objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<Data.TreasureDatabase>(
                        "Assets/_Project/Data/TreasureDatabase.asset");
            });

            // Nạp cả roster. Quét thư mục chứ không liệt kê tay: thêm một
            // con lính mới là tự có mặt, không phải nhớ sửa chỗ này.
            var defs = LoadAll(UnitDir);

            var pool = Need<Gacha.UnitPool>(go);
            Set(pool, so =>
            {
                var a = so.FindProperty("all");
                a.arraySize = defs.Count;
                for (var i = 0; i < defs.Count; i++)
                    a.GetArrayElementAtIndex(i).objectReferenceValue = defs[i];
            });

            var placer = Need<Gacha.RollRewardPlacer>(go);
            Set(placer, so =>
            {
                so.FindProperty("roll").objectReferenceValue = svc;
                so.FindProperty("allyBoard").objectReferenceValue = allyBoard;
                so.FindProperty("spawner").objectReferenceValue = spawner;
                so.FindProperty("pool").objectReferenceValue = pool;
                var arr = so.FindProperty("roster");
                arr.arraySize = defs.Count;
                for (var i = 0; i < defs.Count; i++)
                    arr.GetArrayElementAtIndex(i).objectReferenceValue = defs[i];
            });
            Need<Gacha.WaveRewards>(go);    // tu tim Flow va RollService luc chay
            Debug.Log($"[BattleSceneSetup] Roll: nạp {defs.Count} UnitDefinition.");
            return pool;
        }

        static BattleFlow MakeFlow(GameBalance balance, BattleBoard ally, BattleBoard enemy,
                                   BattleLane lane, UnitDragHandler drag)
        {
            var go = Find("Flow") ?? Create("Flow");
            var flow = Need<BattleFlow>(go);
            Set(flow, so =>
            {
                so.FindProperty("balance").objectReferenceValue = balance;
                so.FindProperty("allyBoard").objectReferenceValue = ally;
                so.FindProperty("enemyBoard").objectReferenceValue = enemy;
                so.FindProperty("lane").objectReferenceValue = lane;
                so.FindProperty("dragHandler").objectReferenceValue = drag;
                // Để trống: 16 ô giờ vẽ trong ảnh nền, không còn sprite rời
                // để bật tắt. BattleBackground đổi hẳn sang ảnh con đường khi
                // vào giao tranh, nên lưới biến mất theo.
                so.FindProperty("allyCells").objectReferenceValue = null;
                so.FindProperty("enemyCells").objectReferenceValue = null;
            });
            return flow;
        }

        static UnitSpawner MakeSpawner(GameObject prefab, BattleBoard ally, BattleBoard enemy,
                                       System.Collections.Generic.List<UnitDefinition> roster)
        {
            var go = Find("Spawner") ?? Create("Spawner");

            var spawner = Need<UnitSpawner>(go);
            Set(spawner, so => so.FindProperty("unitPrefab").objectReferenceValue = prefab);

            var setup = Need<BattleSetup>(go);
            Set(setup, so =>
            {
                so.FindProperty("spawner").objectReferenceValue = spawner;
                so.FindProperty("allyBoard").objectReferenceValue = ally;
                so.FindProperty("enemyBoard").objectReferenceValue = enemy;
                var common = roster.FindAll(d => d.Rank == Rank.Common);
                if (common.Count == 0) common = roster;

                // Phe ta khai man BA CON CUNG MOT LOAI -> cung chui vao MOT o.
                // BattleBoard gop theo UnitDefinition, nen ba con khac loai la
                // ba o khac nhau. Muon thay doi hinh tam giac trong mot o thi
                // bat buoc phai cung loai.
                FillSide(so.FindProperty("allies"), common[0], 3);

                // Phe dich van ba loai khac nhau, moi loai mot con — de nhin
                // ra ngay con nao la con nao luc bam XEM DICH.
                FillSide(so.FindProperty("enemies"), Take(common, 1, 3), 1);
                // Đặt thẳng, không ỷ vào giá trị mặc định: component đã nằm
                // trong scene thì Unity đọc số ĐÃ LƯU, không đọc mặc định mới.
                so.FindProperty("extraEnemiesPerWave").intValue = ExtraEnemiesPerWave;
            });
            return spawner;
        }

        static void FillSide(SerializedProperty array, UnitDefinition def, int count)
        {
            if (def == null) return;
            array.arraySize = 1;
            var e = array.GetArrayElementAtIndex(0);
            e.FindPropertyRelative("definition").objectReferenceValue = def;
            e.FindPropertyRelative("count").intValue = count;
        }

        /// <summary>Lấy <paramref name="n"/> phần tử từ <paramref name="at"/>, quay vòng khi hết.</summary>
        static System.Collections.Generic.List<UnitDefinition> Take(
            System.Collections.Generic.List<UnitDefinition> src, int at, int n)
        {
            var o = new System.Collections.Generic.List<UnitDefinition>();
            for (var i = 0; i < n && src.Count > 0; i++) o.Add(src[(at + i) % src.Count]);
            return o;
        }

        /// <summary>Mỗi loại <paramref name="each"/> con, xếp theo thứ tự thư mục.</summary>
        static void FillSide(SerializedProperty array,
                             System.Collections.Generic.List<UnitDefinition> defs, int each)
        {
            array.arraySize = defs.Count;
            for (var i = 0; i < defs.Count; i++)
            {
                var e = array.GetArrayElementAtIndex(i);
                e.FindPropertyRelative("definition").objectReferenceValue = defs[i];
                e.FindPropertyRelative("count").intValue = each;
            }
        }

        /// <summary>Mọi UnitDefinition trong một thư mục, sắp theo tên cho ổn định.</summary>
        static System.Collections.Generic.List<UnitDefinition> LoadAll(string dir)
        {
            var list = new System.Collections.Generic.List<UnitDefinition>();
            if (!System.IO.Directory.Exists(dir)) return list;
            foreach (var guid in AssetDatabase.FindAssets("t:UnitDefinition", new[] { dir }))
            {
                var d = AssetDatabase.LoadAssetAtPath<UnitDefinition>(
                    AssetDatabase.GUIDToAssetPath(guid));
                if (d != null) list.Add(d);
            }
            list.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            return list;
        }

        /// <summary>
        /// Xoá lính đặt tay. Chỉ xoá object GỐC có component Unit mà không nằm
        /// dưới lưới nào — lính do BattleSetup sinh ra lúc chạy, không nằm
        /// trong scene đã lưu, nên không bị đụng tới.
        /// </summary>
        static void RemoveLooseUnits()
        {
            foreach (var u in Object.FindObjectsByType<Unit>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (u.transform.parent != null) continue;
                Debug.Log($"[BattleSceneSetup] Xoá lính đặt tay '{u.name}'.");
                Undo.DestroyObjectImmediate(u.gameObject);
            }
        }

        // ── tiện ích ──────────────────────────────────────────────────

        static T Load<T>(string path) where T : Object
        {
            var a = AssetDatabase.LoadAssetAtPath<T>(path);
            if (a == null) Debug.LogError($"[BattleSceneSetup] Không tìm thấy {path}");
            return a;
        }

        static GameObject Create(string name)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Dựng scene thử trận");
            return go;
        }

        /// <summary>
        /// Tìm object gốc theo tên. Bọc lại GameObject.Find chỉ để mọi chỗ
        /// trong file dùng chung một lối — đỡ phải nhớ chỗ nào trả null thật.
        /// </summary>
        static GameObject Find(string name)
        {
            var go = GameObject.Find(name);
            return go != null ? go : null;
        }

        /// <summary>
        /// Lấy component, chưa có thì thêm.
        ///
        /// KHÔNG được viết `GetComponent<T>() ?? AddComponent<T>()`. Toán tử
        /// `??` của C# so với null THẬT, nó KHÔNG gọi toán tử `==` mà Unity
        /// nạp chồng. Một component đã bị huỷ ở phía C++ vẫn còn vỏ bọc C#
        /// khác null thật, nên `??` trả về cái vỏ rỗng đó thay vì thêm mới.
        ///
        /// Đã vấp đúng thế: object "Background" giữ một SpriteRenderer đã
        /// huỷ, `??` trả nó về, rồi `Undo.RecordObject` ném "Undo object may
        /// not be null" và cả lệnh dựng scene chết giữa chừng.
        ///
        /// `c != null` thì gọi đúng toán tử của Unity, và coi vỏ rỗng là null.
        /// </summary>
        static T Need<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : Undo.AddComponent<T>(go);
        }

        /// <summary>
        /// Gán vào field private [SerializeField]. Phải đi qua SerializedObject —
        /// gán thẳng bằng reflection thì Unity không đánh dấu scene bẩn và
        /// giá trị mất khi đóng Editor.
        /// </summary>
        static void Set<T>(T target, System.Action<SerializedObject> edit) where T : Object
        {
            var so = new SerializedObject(target);
            edit(so);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
