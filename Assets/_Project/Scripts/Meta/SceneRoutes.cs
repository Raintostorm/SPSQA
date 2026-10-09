using UnityEngine.SceneManagement;

namespace Project.Meta
{
    /// <summary>
    /// Tên scene, gom một chỗ.
    ///
    /// Rải chuỗi "Home" khắp nơi thì đổi tên scene là phải đi tìm từng chỗ,
    /// mà gõ sai một chữ thì Unity chỉ ném lỗi lúc CHẠY tới dòng đó — có
    /// thể là sau mười phút chơi.
    /// </summary>
    public static class SceneRoutes
    {
        public const string Home = "Home";
        public const string Battle = "CharacterLab";
        public const string Upgrade = "Upgrade";
        public const string Treasure = "Treasure";

        public static void GoUpgrade()  => SceneManager.LoadScene(Upgrade);
        public static void GoTreasure() => SceneManager.LoadScene(Treasure);

        public static void GoHome() => SceneManager.LoadScene(Home);

        public static void PlayLevel(int level)
        {
            GameProgress.CurrentLevel = level;
            SceneManager.LoadScene(Battle);
        }
    }
}
