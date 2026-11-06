using Project.Data;
using Project.Meta;
using UnityEngine;

namespace Project.UI
{
    /// <summary>
    /// Một chỗ duy nhất trả lời "chỗ này có việc để làm không", cho chấm đỏ ở thanh Home.
    ///
    /// Điều kiện phải khớp với huy hiệu ⬆ đang hiện trên từng thẻ (TreasureReviewUi,
    /// AllyUpgradeView): **chỉ xét mảnh, không xét vàng**. Nếu chấm đỏ xét cả vàng mà
    /// thẻ bên trong chỉ xét mảnh, người chơi mở ra sẽ thấy ⬆ mà chấm đỏ lại tắt —
    /// hai tín hiệu đá nhau, và không ai biết cái nào đúng.
    /// </summary>
    public static class HomeSignals
    {
        /// <summary>Có relic nào đủ mảnh để lên cấp không.</summary>
        public static bool RelicsReady()
        {
            var save = SaveSystem.Current;
            if (save?.Items == null) return false;
            var database = Resources.Load<TreasureDatabase>("Shop/Relics");
            foreach (var owned in save.Items)
            {
                if (owned == null) continue;
                var item = database ? database.Find(owned.Id) : null;
                // Không tra được định nghĩa thì coi như chưa tới trần, nhưng đừng đoán
                // trần thấp hơn thật — thà bỏ sót một chấm còn hơn báo mãi không tắt.
                // TreasureItem la class thuong, khong phai UnityEngine.Object, nen phai so null that.
                int max = item != null ? item.MaxLevel : int.MaxValue;
                if (owned.Level < max && owned.Shards >= TreasureItem.ShardsToNext(owned.Level)) return true;
            }
            return false;
        }

        /// <summary>Có lính nào đủ mảnh để lên cấp không.</summary>
        public static bool AlliesReady(int maxLevel = 30)
        {
            var save = SaveSystem.Current;
            if (save?.Units == null) return false;
            foreach (var row in save.Units)
            {
                if (row == null || string.IsNullOrEmpty(row.UnitId)) continue;
                if (row.Level < maxLevel && AllySummon.UpgradeMaterials(row.UnitId) >= AllySummon.ShardsFor(row.Level)) return true;
            }
            return false;
        }
    }
}
