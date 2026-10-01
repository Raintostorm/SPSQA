namespace Project.Data
{
    /// <summary>
    /// Bậc tiến hoá theo thời đại. Thay cho hệ rarity Common→Legendary cũ.
    ///
    /// Thứ tự KHÔNG được đổi: (int)Era dùng làm index cho các mảng trong
    /// GameBalance, và giá trị số đã serialize vào các file .asset.
    /// StoneAge phải là 0. Đừng chèn giá trị mới vào GIỮA.
    /// </summary>
    public enum Era
    {
        StoneAge = 0,
        Medieval = 1,
        Modern = 2,
        Future = 3,
        Mythic = 4,     // không bán, không phân giải, giới hạn số lượng trên sân
    }
}
