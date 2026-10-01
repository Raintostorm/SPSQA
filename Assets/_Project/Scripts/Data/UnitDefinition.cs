using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(
        fileName = "UnitDefinition",
        menuName = "SpinSquad/Unit Definition"
    )]
    public sealed class UnitDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string UnitId;          // chi de ĐỌC (save/log). KHONG parse ra Role/Era tu chuoi nay.
        public string DisplayName;
        public UnitRole Role;
        public Era BaseEra;
        public UnitTeam Team;

        [Header("Stats")]
        public float BaseHp = 200f;
        public float BaseAttack = 30f;

        [Header("Combat")]
        public bool IsRanged;
        public float AttackRange = 1.65f;
        public float AttackInterval = 0.52f;
        public float MoveSpeed = 1.35f;

        [Header("Visual")]
        public GameObject BattlePrefab;
        public Sprite Portrait;
    }
}