namespace Project.Data
{
    /// <summary>
    /// Bậc. Trục TIẾN HOÁ — ba con cùng bậc cùng giống gộp thành một con bậc trên.
    ///
    /// Thay cho Era cũ. Khác Era ở hai chỗ:
    ///   • bốn bậc, không phải năm (Era có thêm Mythic)
    ///   • đây mới là trục tiến hoá; giống loài giờ là nhánh ngang
    ///
    /// Thứ tự KHÔNG được đổi: (int)Rank dùng làm index cho mảng trong
    /// GameBalance và đã serialize vào .asset.
    /// </summary>
    public enum Rank
    {
        Common = 0,
        Rare = 1,
        Epic = 2,
        Legendary = 3,
    }
}
