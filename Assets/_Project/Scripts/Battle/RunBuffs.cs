using Project.Data;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Các buff đã tích luỹ trong trận, ở dạng lính ĐỌC ĐƯỢC.
    ///
    /// Vì sao là static: buff thay đổi GIỮA chừng (mỗi lần roll trúng ô Buff),
    /// và phải áp cho cả lính đã có trên sân chứ không chỉ lính sinh sau. Tức
    /// là mô hình KÉO — lính hỏi lúc cần — chứ không phải đẩy lúc spawn. Luồn
    /// một tham chiếu qua Spawner tới từng lính thì vẫn phải có chỗ chung để
    /// trỏ tới, nên thà nhận luôn là nó chung.
    ///
    /// Vì sao nằm ở Battle chứ không phải Gacha: lính KHÔNG được biết gacha
    /// là gì. Gacha ghi vào đây, Battle đọc ra — một cái hộp phẳng ở giữa,
    /// không bên nào phải tham chiếu bên kia.
    ///
    /// Một trận một bộ số. Bắt đầu trận mới thì PHẢI gọi Reset, không thì
    /// buff của ván trước chảy sang ván sau.
    /// </summary>
    public static class RunBuffs
    {
        /// <summary>Phân số: 0.25 = +25%.</summary>
        public static float HpPct;
        public static float DmgPct;
        public static float AtkSpeedPct;

        /// <summary>0..1. Xác suất mỗi nhát đánh thành chí mạng.</summary>
        public static float CritChance;

        /// <summary>0..1. Phần sát thương gây ra được hồi lại thành máu.</summary>
        public static float LifeSteal;

        /// <summary>
        /// Điểm giáp. KHÔNG phải phần trăm — xem <see cref="DamageAfterArmor"/>.
        /// </summary>
        public static float ArmorPoints;

        /// <summary>Nhát chí mạng nhân bao nhiêu. RollService nạp từ GameBalance.</summary>
        public static float CritMultiplier = 2f;

        /// <summary>
        /// Xoá sạch mỗi lần vào Play, KHÔNG phụ thuộc thứ tự Awake.
        ///
        /// Biến static sống dai hơn scene. Mặc định Unity nạp lại domain khi
        /// bấm Play nên chúng tự về 0, nhưng ai tắt "Reload Domain" cho nhanh
        /// là buff của ván trước chảy sang ván sau — và chảy âm thầm, lính
        /// chỉ mạnh lên một cách khó hiểu.
        ///
        /// Thuộc tính này chạy trước mọi Awake, nên cũng không còn phải lo
        /// RollService hay Unit ai tỉnh trước.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetOnPlay() => Reset();

        /// <summary>
        /// Buff CHỈ ăn cho phe ta. Địch luôn nhận 0.
        ///
        /// Bốn hàm này là chỗ DUY NHẤT quyết định điều đó. Để từng nơi tự
        /// viết `team == Ally ? ... : 0` thì sớm muộn sót một chỗ, và sót
        /// chỗ nào là địch được buff miễn phí ở đúng chỉ số đó — một lỗi
        /// cân bằng gần như không thể thấy bằng mắt.
        /// </summary>
        public static float Hp(UnitTeam t)    => t == UnitTeam.Ally ? HpPct : 0f;
        public static float Dmg(UnitTeam t)   => t == UnitTeam.Ally ? DmgPct : 0f;
        public static float Speed(UnitTeam t) => t == UnitTeam.Ally ? AtkSpeedPct : 0f;
        public static float Crit(UnitTeam t)  => t == UnitTeam.Ally ? CritChance : 0f;
        public static float Steal(UnitTeam t) => t == UnitTeam.Ally ? LifeSteal : 0f;
        public static float Armor(UnitTeam t) => t == UnitTeam.Ally ? ArmorPoints : 0f;

        /// <summary>
        /// Sát thương còn lại sau khi trừ giáp.
        ///
        /// giảm = giáp / (giáp + 100) — BÃO HOÀ, không bao giờ chạm 100%.
        /// Trừ thẳng "giáp %" thì đủ bộ cấp tối đa là 60 điểm, tức giảm 60%
        /// đã gần miễn nhiễm, và quá 100 là BẤT TỬ. Công thức cộng thẳng
        /// luôn vỡ ở một mức nào đó; công thức này thì không, mà thêm giáp
        /// vẫn luôn có ích.
        ///
        /// 60 điểm → giảm 38%. 200 điểm → giảm 67%.
        /// </summary>
        public static float DamageAfterArmor(float damage, UnitTeam victim)
        {
            var a = Armor(victim);
            return a <= 0f ? damage : damage * (100f / (a + 100f));
        }

        public static void Reset()
        {
            HpPct = DmgPct = AtkSpeedPct = CritChance = 0f;
            LifeSteal = ArmorPoints = 0f;
            // CritMultiplier KHONG reset ve 0: no la hang so can bang, khong
            // phai thu tich luy. Reset ve 0 la chi mang gay 0 sat thuong.
        }
    }
}
