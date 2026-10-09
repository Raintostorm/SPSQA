using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Data
{
    [CreateAssetMenu(
        fileName = "UnitDefinition",
        menuName = "SpinSquad/Unit Definition"
    )]
    public sealed class UnitDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string UnitId;          // chi de ĐỌC (save/log). KHONG parse ra Race/Rank tu chuoi nay.
        public string DisplayName;

        [Tooltip("Giong loai. Nhanh NGANG — doi giong khong lam manh len.")]
        public Race Race = Race.Human;

        [Tooltip("Bac. Truc TIEN HOA — ba con cung bac cung giong gop thanh " +
                 "mot con bac tren. Day la cho khac he cu: truoc kia Era la " +
                 "truc tien hoa con Variant la nhanh ngang, gio nguoc lai.")]
        public Rank Rank = Rank.Common;
        // KHONG co Team o day. Phe la thuoc tinh cua CON LINH TREN SAN,
        // khong phai cua dinh nghia — neu khong thi moi lính phai co 2 asset
        // (ban ally + ban enemy) va sua chi so phai sua ca hai. Xem Unit.team.

        [Header("Stats")]
        public float BaseHp = 200f;
        public float BaseAttack = 30f;

        [Header("Combat")]
        public bool IsRanged;
        public float AttackRange = 1.65f;

        /// <summary>
        /// Thoi gian NGHI sau khi clip tan cong chay xong, truoc cu tiep theo.
        /// KHONG phai tong chu ky — clip dai bao nhieu khong anh huong con so nay.
        /// Doi ten tu AttackInterval vi "interval" khong noi ro moc tinh tu dau.
        /// </summary>
        [FormerlySerializedAs("AttackInterval")]
        public float AttackCooldown = 1f;

        public float MoveSpeed = 1.35f;

        [Header("Ranged — bo qua neu IsRanged = false")]
        public GameObject ProjectilePrefab;
        public float ProjectileSpeed = 6f;

        /// <summary>
        /// Dinh vong cung cao hon duong thang bao nhieu don vi.
        /// 0 = bay thang (ten, dan sung, tia laser).
        /// ~0.6 = nem lao.
        /// </summary>
        public float ProjectileArcHeight = 0f;

        [Header("Visual")]
        [Tooltip("Bo khung hinh: dung, di, danh, chet, chieu. QUYET DINH con " +
                 "linh trong the nao va cu dong ra sao.")]
        public UnitAnimationSet Animations;

        [Tooltip("Phong to ca con linh. Dung de can chinh con cao thap khac " +
                 "nhau ve cung mot tam, va de lam con trum to hon lu nho.")]
        public float Scale = 1f;

        /// <summary>
        /// Day lính lech khoi tam o bao nhieu, de KHUNG BAO cua no nam giua
        /// o chu khong phai diem xoay.
        ///
        /// Vi sao phai rieng tung con: diem xoay nam o CHAN, con cai phai
        /// vua o la toan bo hinh ve. Hai cho do cach nhau mot khoang khac
        /// nhau o moi nhan vat — con cao thi than moc len cao hon, con cam
        /// kiem chia phai thi khung bao lech phai. Dung mot so chung thi
        /// do duoc: khe duoi luon 0.082 trong khi khe tren toi 0.34, va
        /// human_rare thua trai 0.241 ma tran phai 0.065.
        ///
        /// Da nhan san Scale. tools/fit_cell_scale.py ghi ca hai cung luc —
        /// doi Scale bang tay ma khong chay lai tool la so nay thanh sai.
        /// </summary>
        public Vector2 CellOffset;

        /// <summary>
        /// De TRONG trong hau het truong hop. Ca roster dung chung mot prefab
        /// (UnitSpawner giu), chi khac 3 sprite o tren. Chi gan rieng khi con
        /// nay can bo khung hinh khac han.
        /// </summary>
        public GameObject BattlePrefab;

        public Sprite Portrait;
    }
}