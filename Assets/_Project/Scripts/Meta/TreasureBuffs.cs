using System.Collections.Generic;
using Project.Battle;
using Project.Data;
using UnityEngine;

namespace Project.Meta
{
    /// <summary>
    /// Đổ chỉ số từ kho vật phẩm vào <see cref="RunBuffs"/> khi vào trận.
    ///
    /// Cộng DỒN lên buff mua trong trận chứ không ghi đè: hai nguồn độc lập,
    /// vật phẩm là nền tảng lâu dài, buff roll là thứ của riêng ván này.
    ///
    /// Gọi SAU RunState.Reset — Reset xoá RunBuffs về 0, gọi trước là mất
    /// sạch phần vật phẩm mà không có gì báo.
    /// </summary>
    public static class TreasureBuffs
    {
        public static void Apply(TreasureDatabase db, GameBalance balance)
        {
            // Cap nang nhan vat: nap TRUOC khi kiem db, vi no khong phu
            // thuoc kho bau. Thieu dong nay thi mua cap xong vao tran linh
            // van y nguyen — va khong co gi bao.
            var rows = new List<KeyValuePair<string, int>>();
            foreach (var u in SaveSystem.Current.Units)
                rows.Add(new KeyValuePair<string, int>(u.UnitId, u.Level));
            UnitUpgrades.Load(rows);

            if (db == null) return;

            // Hai thang don vi khac nhau, va day la cho duy nhat doi chung.
            //   NHAN     : 50 (%)    -> 0.50 phan so, nhan vao chi so goc
            //   XAC SUAT : 24 (diem) -> 0.24 cho chi mang/hut mau,
            //                           con GIAP giu nguyen diem vi cong
            //                           thuc bao hoa an theo diem.
            RunBuffs.DmgPct      += TreasureRoll.Total(TreasureStat.Atk, db) / 100f;
            RunBuffs.HpPct       += TreasureRoll.Total(TreasureStat.Hp, db) / 100f;
            RunBuffs.AtkSpeedPct += TreasureRoll.Total(TreasureStat.AtkSpeed, db) / 100f;
            RunBuffs.CritChance  += TreasureRoll.Total(TreasureStat.CritChance, db) / 100f;
            RunBuffs.LifeSteal   += TreasureRoll.Total(TreasureStat.LifeSteal, db) / 100f;
            RunBuffs.ArmorPoints += TreasureRoll.Total(TreasureStat.Armor, db);

            // ST chi mang NHAN vao he so goc, khong cong them vao no: +60%
            // nghia la 2.0 thanh 3.2, chu khong phai 2.0 thanh 2.6.
            var cd = TreasureRoll.Total(TreasureStat.CritDamage, db);
            if (cd != 0f) RunBuffs.CritMultiplier *= 1f + cd / 100f;

            Debug.Log($"[TreasureBuffs] ATK +{RunBuffs.DmgPct:P0}, " +
                      $"Máu +{RunBuffs.HpPct:P0}, Tốc đánh +{RunBuffs.AtkSpeedPct:P0}, " +
                      $"Chí mạng +{RunBuffs.CritChance:P0}, Hút máu +{RunBuffs.LifeSteal:P0}, " +
                      $"Giáp {RunBuffs.ArmorPoints:0} điểm " +
                      $"(giảm {1f - 100f / (RunBuffs.ArmorPoints + 100f):P0}), " +
                      $"ST chí mạng ×{RunBuffs.CritMultiplier:0.00}");
        }
    }
}
