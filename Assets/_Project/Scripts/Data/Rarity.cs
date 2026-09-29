namespace Project.Data
{
    /// <summary>Thứ tự KHÔNG được đổi: (int)Rarity dùng làm index cho
    /// các mảng trong GameBalance. Common phải là 0.</summary>
    public enum Rarity
    {
        Common = 0,
        Rare = 1,
        Epic = 2,
        Legendary = 3,
        Mythic = 4,     // không bán, không phân giải, giới hạn số lượng
    }
}