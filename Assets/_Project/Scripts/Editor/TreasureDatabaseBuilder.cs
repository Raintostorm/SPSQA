using System.Collections.Generic;
using System.IO;
using Project.Data;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools
{
    /// <summary>
    /// Sinh TreasureDatabase.asset từ tools/treasure_items.json.
    ///
    /// Một nguồn sự thật: tên và chỉ số nằm ở file JSON, cả bảng để người
    /// đọc lẫn prompt art đều sinh từ đó. Gõ tay 30 món vào Inspector thì
    /// sớm muộn ba nơi lệch nhau mà không ai biết.
    /// </summary>
    public static class TreasureDatabaseBuilder
    {
        const string Json = "tools/treasure_items.json";
        const string Asset = "Assets/_Project/Data/TreasureDatabase.asset";
        const string IconDir = "Assets/_Project/Art/UI";

        // Thu tu trong JSON khop thu tu icon da nhap: item_c1..12, r1..8,
        // e1..6, l1..4. Doi mot ben ma quen ben kia la gan nham hinh — da
        // dinh dung loi do voi icon vang va chia khoa.
        static readonly Dictionary<string, string> Prefix = new()
        {
            { "Common", "c" }, { "Rare", "r" }, { "Epic", "e" }, { "Legendary", "l" },
        };

        [System.Serializable] class StatRow { public string stat; public float @base; public float perLevel; }
        [System.Serializable] class ItemRow { public string name, tier, art; public int maxLevel; public StatRow[] stats; }
        [System.Serializable] class Wrap { public ItemRow[] items; }

        [MenuItem("SpinSquad/Nhập 30 vật phẩm Treasure")]
        public static void Run()
        {
            if (!File.Exists(Json))
            {
                Debug.LogError($"[TreasureDatabaseBuilder] Không thấy {Json}.");
                return;
            }

            // JsonUtility khong doc duoc mang o goc, phai boc vao mot object.
            var rows = JsonUtility.FromJson<Wrap>(
                "{\"items\":" + File.ReadAllText(Json) + "}").items;

            var db = AssetDatabase.LoadAssetAtPath<TreasureDatabase>(Asset);
            var isNew = db == null;
            if (isNew) db = ScriptableObject.CreateInstance<TreasureDatabase>();

            var items = new List<TreasureItem>();
            var perTier = new Dictionary<string, int>();
            var missing = 0;

            foreach (var r in rows)
            {
                perTier.TryGetValue(r.tier, out var n);
                perTier[r.tier] = ++n;

                var iconName = $"item_{Prefix[r.tier]}{n}";
                var icon = AssetDatabase.LoadAssetAtPath<Sprite>($"{IconDir}/{iconName}.png");
                if (icon == null) missing++;

                var stats = new TreasureItem.Entry[r.stats.Length];
                for (var i = 0; i < r.stats.Length; i++)
                    stats[i] = new TreasureItem.Entry
                    {
                        Stat = Parse(r.stats[i].stat),
                        Base = r.stats[i].@base,
                        PerLevel = r.stats[i].perLevel,
                    };

                items.Add(new TreasureItem
                {
                    Id = iconName,          // dung luon ten icon lam id: mot ten, khong the lech
                    DisplayName = r.name,
                    Tier = (Rank)System.Enum.Parse(typeof(Rank), r.tier),
                    MaxLevel = r.maxLevel,
                    Icon = icon,
                    Stats = stats,
                });
            }

            db.Items = items.ToArray();
            if (isNew) AssetDatabase.CreateAsset(db, Asset);
            else EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();

            Debug.Log($"[TreasureDatabaseBuilder] {items.Count} vật phẩm vào {Asset}." +
                      (missing > 0 ? $" THIẾU {missing} icon." : " Đủ icon."));
        }

        /// <summary>Tên tiếng Việt trong JSON → enum. Sai tên thì KÊU, không im.</summary>
        static TreasureStat Parse(string s) => s switch
        {
            "ATK" => TreasureStat.Atk,
            "Máu" => TreasureStat.Hp,
            "Tốc đánh" => TreasureStat.AtkSpeed,
            "ST chí mạng" => TreasureStat.CritDamage,
            "Chí mạng" => TreasureStat.CritChance,
            "Hút máu" => TreasureStat.LifeSteal,
            "Giáp" => TreasureStat.Armor,
            _ => throw new System.ArgumentException(
                $"[TreasureDatabaseBuilder] Chỉ số lạ: '{s}'. Sửa tools/treasure_items.py."),
        };
    }
}
