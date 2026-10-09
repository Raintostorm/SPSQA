using Project.Data;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Con đường giao tranh: lính hai phe ra đây đánh nhau.
    ///
    /// CHỈ biết toạ độ. Không biết khi nào ai ra, không giữ danh sách lính —
    /// đó là việc của BattleFlow. Cùng một kiểu tách như BattleGrid và
    /// BattleBoard.
    /// </summary>
    public sealed class BattleLane : MonoBehaviour
    {
        [Tooltip("Độ cao của mặt đường, theo trục y.")]
        [SerializeField] float laneY = -0.5f;

        [Tooltip("Lính ra trận ở mép này rồi đi vào giữa.")]
        [SerializeField] float allyEntryX = -5.5f;
        [SerializeField] float enemyEntryX = 5.5f;

        [Tooltip("Ô ra sau lùi về phía sau ngần này. Để 0 là mọi ô vào " +
                 "cùng một điểm — xem ghi chú trong EntryPoint.")]
        [SerializeField] float entrySpacing;

        [Tooltip("Số hàng ngang trên mặt đường. Ô thứ n vào hàng n % số hàng.")]
        [SerializeField] int laneRows = 4;

        [Tooltip("Khoảng cách giữa hai hàng, đơn vị thế giới.")]
        [SerializeField] float rowSpacing = 0.95f;

        public float LaneY => laneY;

        /// <summary>
        /// Chỗ Ô thứ <paramref name="index"/> của phe này bước vào.
        ///
        /// Mặc định không lùi. Hồi còn thả lẻ từng con thì phải lùi, không
        /// thì hai con chồng khít lên nhau lúc vừa hiện. Giờ thả theo ô và
        /// các ô cách nhau 0.45 giây, mà trong 0.45 giây ô trước đã đi được
        /// 1.0 đơn vị — tách bằng THỜI GIAN là đủ, không cần tách bằng chỗ.
        ///
        /// Và lùi thì có hại: 16 ô × 0.45 là ô cuối sinh ra ở x = −12.7,
        /// ngoài khung nhìn 7 đơn vị, phải đi thêm 3 giây mới vào tới trận.
        ///
        /// CHIỀU DỌC thì ngược lại, phải tách: màn dọc chỉ rộng 5.5 đơn vị
        /// nên dồn cả 16 ô lên một vạch là một đống không đọc được con nào.
        /// Ô thứ n vào hàng n % laneRows, nên bốn ô liền nhau nằm bốn hàng
        /// khác nhau.
        ///
        /// Hàng được giữ nguyên suốt trận vì UnitBrain.Chase chỉ đi theo
        /// trục X — đó chính là lý do nó cố ý không đụng tới y.
        /// </summary>
        public Vector3 EntryPoint(UnitTeam team, int index)
        {
            var dir = team == UnitTeam.Ally ? -1f : 1f;      // lui ve phia sau
            var x = (team == UnitTeam.Ally ? allyEntryX : enemyEntryX)
                  + dir * entrySpacing * index;
            return new Vector3(x, laneY + RowOffset(index), 0f);
        }

        /// <summary>Độ lệch dọc của hàng thứ <paramref name="index"/>, quanh tâm đường.</summary>
        public float RowOffset(int index)
        {
            var n = Mathf.Max(1, laneRows);
            var row = ((index % n) + n) % n;                 // an toan voi so am
            return (row - (n - 1) * 0.5f) * rowSpacing;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.8f, 0.3f, 0.7f);
            for (var i = 0; i < Mathf.Max(1, laneRows); i++)
            {
                var y = laneY + RowOffset(i);
                var a = new Vector3(allyEntryX, y, 0f);
                var b = new Vector3(enemyEntryX, y, 0f);
                Gizmos.DrawLine(a, b);
                Gizmos.DrawWireSphere(a, 0.10f);
                Gizmos.DrawWireSphere(b, 0.10f);
            }
        }
    }
}
