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
        // Index = (int)Rarity: 0 Common · 1 Rare · 2 Epic · 3 Legendary · 4 Mythic
        // Mythic = 0 chi de mang an toan voi moi gia tri enum.
        // Luat "Mythic khong ban duoc" phai viet thanh dieu kien ro rang o cho xu ly ban.
        public int[] SellPriceByRarity = { 2, 5, 10, 20, 0 };
        [Header("ClusterReward")]
        // Index 0..3 ↔ cum 3..6 o cung loai · tra bang [soO - 3]
        public int[] ClusterRewardHP = {10, 25, 100, 200};
        public int[] ClusterRewardDamage = {10, 25, 100, 200};
        public int[] ClusterRewardCritChance = {1, 2, 5, 15};
        public int[] ClusterRewardAtkSpeed = {5, 15, 50, 100};
        public int[] ClusterRewardCoin = { 15, 40, 80, 200 };

        [Header("Combat")]
        public float DefaultMoveSpeed = 1.35f;
        public float MeleeStrikeInterval = 0.24f;
        public float RangedShotInterval = 0.52f;
        public float DefaultAttackRange = 1.65f;
        public float EngageDelay = 2f;

        [Header("Battle")]
        public int LevelCount = 3;

        [Header("Grid")]
        public int GridRows = 4;
        public int GridCols = 4;
        public int MaxUnitsPerCell = 3;     // Common → Legendary
        public int MaxMythicPerCell = 1;
        public int StartingMythicLimit = 2;   // buff treasure tăng CÁI NÀY trong RunState, không sửa asset
    }
}