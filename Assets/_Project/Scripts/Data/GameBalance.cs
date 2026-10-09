using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(fileName = "GameBalance", menuName = "SpinSquad/Game Balance")]
    public sealed class GameBalance : ScriptableObject
    {
        [Header("Roll")]
        public int RollCost = 20;              // giá 1 lần roll, treasure buff đọc rồi trừ vào RunState
        public int StartingCoins = 40;
        public int CoinPerWaveClear = 40;
        public int[] AllyCountWeights = { 1, 5, 20, 40, 24, 8, 2 }; // Index = số ô Ally: 0..6

        [Header("Sell")]
        // Index = (int)Rank: 0 Common · 1 Rare · 2 Epic · 3 Legendary
        //
        // Bon phan tu, khong phai nam. He cu co Era.Mythic "khong ban duoc";
        // bac khong con bac nao nhu vay nua, Legendary chi don gian la dat
        // nhat. Bo luat ngoai le di thi khong con cho nao de quen kiem tra.
        public int[] SellPriceByRank = { 2, 5, 10, 20 };
        [Header("ClusterReward")]
        // Index 0..3 ↔ cum 3..6 o cung loai · tra bang [soO - 3]
        public int[] ClusterRewardHP = {10, 25, 100, 200};
        public int[] ClusterRewardDamage = {10, 25, 100, 200};
        public int[] ClusterRewardCritChance = {1, 2, 5, 15};
        public int[] ClusterRewardAtkSpeed = {5, 15, 50, 100};
        public int[] ClusterRewardCoin = { 15, 40, 80, 200 };

        [Header("Combat")]
        // Nhip danh va tam danh nam trong TUNG UnitDefinition (AttackCooldown,
        // AttackRange), khong nam o day. Da bo MeleeStrikeInterval 0.24,
        // RangedShotInterval 0.52 va DefaultAttackRange 1.65 — ca ba chep tu
        // ban cu dung PPU 128; voi PPU 512 thi 1.65 la tam CAN CHIEN, do o M3.

        // Nhat chi mang gay gap bao nhieu lan. Handoff/01 ta "roll xac suat
        // moi nhat" nhung KHONG noi nhan bao nhieu — 2.0 la toi dat, de o day
        // de doi duoc chu khong chon ngam trong code.
        public float CritMultiplier = 2f;

        // Chi la moc de so sanh khi dat so cho linh moi. Khong code nao doc.
        public float DefaultMoveSpeed = 2.2f;

        // Thoi gian dem nguoc truoc khi vao tran.
        public float EngageDelay = 2f;

        // Giay giua hai lan THA MOT O ra duong — theo o, khong theo tung con.
        //
        // Doi tu 0.7 xuong 0.45, va don vi doi tu "con" sang "o": truoc day
        // 48 con moi con 0.7s la mat 33 giay chi de ra quan. Gio nhieu nhat
        // 16 o, moi o 0.45s, con 7 giay.
        //
        // Ca hai phe dung chung con so nay, nen ben nao dong o hon thi ra het
        // lau hon — do la mot phan can bang.
        public float WaveSpawnInterval = 0.45f;

        // Giay dung lai o man ket qua truoc khi ve san chuan bi. De 0 thi
        // tran vua xong la nhay man ngay, nguoi choi khong kip thay ai thang.
        public float PostBattlePause = 1.5f;

        [Header("Battle")]
        public int LevelCount = 3;

        // Handoff/01 muc 1 noi 10 wave. Ha xuong 3 de mot van ngan lai,
        // ve Homepage. Thieu con so nay thi wave chay vo tan va khong bao gio
        // co man thang.
        // Dich dong them 1 con moi wave (BattleSetup.extraEnemiesPerWave),
        // nen wave 1 ba con, wave 10 muoi hai con.
        public int WavesPerLevel = 3;

        [Header("Treasure — tỉ lệ roll, theo bậc Common→Legendary")]
        [Tooltip("Trọng số, KHÔNG phải phần trăm — tool tự chia tổng. " +
                 "Để trọng số thì thêm bậc mới không phải tính lại cả bảng.")]
        public int[] TreasureWeights = { 70, 23, 6, 1 };

        [Tooltip("Quay bấy nhiêu lượt không ra Legendary thì ép ra. " +
                 "Không có nó thì 36.6% lượt x100 trắng tay — mà x100 tốn " +
                 "100 chìa, bằng 33 lần clear level.")]
        public int LegendaryPity = 80;

        [Tooltip("Lượt x10 bảo đảm ít nhất một món từ bậc này trở lên. " +
                 "Không có thì 48.4% lượt x10 không ra nổi thứ gì trên Rare.")]
        public Rank TenRollFloor = Rank.Epic;

        [Header("Treasure — thang giá trị theo bậc")]
        [Tooltip("ATK/Máu/Tốc đánh/ST chí mạng: phần trăm chỉ số gốc.")]
        public float[] TreasureMultiplierBase = { 3f, 6f, 10f, 16f };

        [Tooltip("Chí mạng/Hút máu/Giáp: điểm phần trăm tuyệt đối.")]
        public float[] TreasureProbabilityBase = { 1.5f, 3f, 10f, 14f };

        [Tooltip("Cấp tối đa mỗi bậc. Giảm dần vì mảnh bậc cao hiếm hơn hẳn.")]
        public int[] TreasureMaxLevel = { 10, 8, 6, 5 };

        [Header("Nâng cấp nhân vật")]
        [Tooltip("Giá lên cấp N = giá này × N. Mỗi cấp +10% máu và sát thương.")]
        public int UpgradeBaseCost = 500;
        public int UnitMaxLevel = 10;

        [Header("Thưởng khi qua một màn")]
        [Tooltip("Lần ĐẦU qua màn đó.")]
        public int GoldPerLevelClear = 1000;
        public int KeysPerLevelClear = 3;

        [Tooltip("Chơi LẠI màn đã qua. Tách riêng vì thưởng đủ cho mọi lần " +
                 "chơi lại thì vàng thành vô hạn — level 1 có 3 wave, " +
                 "khoảng 2-3 phút, tức ~400 vàng/phút không trần. " +
                 "Hiện để bằng lần đầu đúng như đã chốt; hạ xuống là sửa " +
                 "đúng hai con số này.")]
        public int GoldPerLevelReplay = 1000;
        public int KeysPerLevelReplay = 3;

        [Header("Grid")]
        public int GridRows = 4;
        public int GridCols = 4;
        public int MaxUnitsPerCell = 3;     // Common → Legendary
        // Gioi han so con bac cao tren san. He cu goi la "Mythic"; bac khong
        // con bac do nua nen doi ten theo Rank.Legendary.
        public int MaxLegendaryPerCell = 1;
        public int StartingLegendaryLimit = 2;   // buff treasure tăng CÁI NÀY trong RunState
    }
}