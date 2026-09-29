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

        [Header("Sell")]
        // Index = (int)Rarity: 0 Common · 1 Rare · 2 Epic · 3 Legendary
        public int[] SellPriceByRarity = { 2, 5, 10, 20 };
        [Header("ClusterReward")]
        // Index = (int)ClusterReward: 3 slots same · 4 slots same · 5 slots same · 6 slots same
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
    }
}