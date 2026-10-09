using System.Collections.Generic;
using Project.Data;
using UnityEngine;

namespace Project.Gacha
{
    /// <summary>
    /// Tra lính theo thời đại.
    ///
    /// Handoff/09 chốt: "không dùng field trỏ 1-1 tới bậc kế tiếp, cần một
    /// pool tra theo Era — và gacha cũng cần đúng cấu trúc đó. Một UnitPool
    /// phục vụ cả hai."
    ///
    /// Lý do không trỏ 1-1: tiến hoá ra lính NGẪU NHIÊN ở thời đại kế, không
    /// ra con định sẵn. Trỏ 1-1 là mất hẳn chữ "spin" trong tên game.
    /// </summary>
    public sealed class UnitPool : MonoBehaviour
    {
        [Tooltip("Cả roster. BattleSceneSetup quét thư mục Data và nạp.")]
        [SerializeField] UnitDefinition[] all;

        readonly Dictionary<Rank, List<UnitDefinition>> byRank = new();

        void Awake() => Rebuild();

        void Rebuild()
        {
            byRank.Clear();
            if (all == null) return;
            foreach (var d in all)
            {
                if (d == null) continue;
                if (!byRank.TryGetValue(d.Rank, out var list))
                    byRank[d.Rank] = list = new List<UnitDefinition>();
                list.Add(d);
            }
        }

        public IReadOnlyList<UnitDefinition> Of(Rank rank)
        {
            if (byRank.Count == 0) Rebuild();      // phong khi goi truoc Awake
            return byRank.TryGetValue(rank, out var l) ? l : (IReadOnlyList<UnitDefinition>)System.Array.Empty<UnitDefinition>();
        }

        public UnitDefinition Find(Rank rank, Race race)
        {
            foreach (var d in Of(rank))
                if (d.Race == race) return d;
            return null;
        }

        /// <summary>Một con bất kỳ của thời đại đó. null nếu thời đại đó rỗng.</summary>
        public UnitDefinition Random(Rank rank, System.Random rng)
        {
            var list = Of(rank);
            return list.Count == 0 ? null : list[rng.Next(list.Count)];
        }

        public bool HasAny(Rank rank) => Of(rank).Count > 0;
    }
}
