using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Hai bàn nằm chung một chỗ; cái nào được xem thì nằm đúng chỗ, cái kia
    /// bị dời ra xa.
    ///
    /// Vì sao DỜI chứ không SetActive(false): lính là con của bàn, tắt bàn
    /// là tắt luôn lính — mà Unit tự rút tên khỏi UnitRegistry khi bị tắt.
    /// Lúc đó mọi phép đếm quân, mọi phép tìm địch đều sai, và chẳng có gì
    /// báo. Dời đi thì không ai bị tắt, chỉ là không nhìn thấy.
    ///
    /// Chỉ có nghĩa ở giai đoạn chuẩn bị. Ra trận thì DỜI CẢ HAI: lúc đó
    /// lính đã rời lưới ra đường, để bàn nào lại cũng chỉ tổ che mặt đường.
    /// </summary>
    public sealed class BoardView : MonoBehaviour
    {
        /// <summary>Xa đủ để không lọt vào khung nhìn ở mọi tỉ lệ máy.</summary>
        const float Away = 500f;

        [SerializeField] BattleBoard allyBoard;
        [SerializeField] BattleBoard enemyBoard;
        [SerializeField] BattleFlow flow;

        /// <summary>Đang xem bàn địch hay bàn ta.</summary>
        public bool ShowingEnemy { get; private set; }

        Vector3 allyHome, enemyHome;
        bool captured;

        void Awake()
        {
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
            Capture();
        }

        /// <summary>
        /// Nhớ chỗ gốc MỘT LẦN. Nhớ lại lần nữa trong lúc đang ẩn thì chỗ
        /// gốc hoá ra là chỗ đã dời — bàn biến mất vĩnh viễn.
        /// </summary>
        void Capture()
        {
            if (captured) return;
            if (allyBoard != null) allyHome = allyBoard.transform.position;
            if (enemyBoard != null) enemyHome = enemyBoard.transform.position;
            captured = true;
        }

        void OnEnable()
        {
            Capture();
            if (flow != null) flow.PhaseChanged += OnPhase;
            Apply();
        }

        void OnDisable()
        {
            if (flow != null) flow.PhaseChanged -= OnPhase;
        }

        void OnPhase(BattleFlow.Phase p)
        {
            // Sang wave mới thì luôn về bàn ta — người chơi vào để xếp quân,
            // không phải để xem lại địch.
            if (p == BattleFlow.Phase.Prep) ShowingEnemy = false;
            Apply();
        }

        public void Toggle()
        {
            if (flow != null && flow.Current != BattleFlow.Phase.Prep) return;
            ShowingEnemy = !ShowingEnemy;
            Apply();
        }

        void Apply()
        {
            var prep = flow == null || flow.Current == BattleFlow.Phase.Prep;
            Place(allyBoard,  allyHome,  prep && !ShowingEnemy);
            Place(enemyBoard, enemyHome, prep && ShowingEnemy);
        }

        static void Place(BattleBoard b, Vector3 home, bool visible)
        {
            if (b == null) return;
            b.transform.position = visible ? home : home + Vector3.up * Away;
        }
    }
}
