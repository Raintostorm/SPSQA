namespace Project.Data
{
    /// <summary>Chủng tộc. Mỗi cặp (Race, Rarity) ứng với ĐÚNG MỘT UnitDefinition
    /// → 5 race × 4 rarity = 20 lính gốc (Mythic tính riêng).
    /// Đừng chèn giá trị mới vào GIỮA: số đã serialize vào các file .asset.</summary>
    public enum Race
    {
        Human = 0,
        Elf = 1,
        Necro = 2,
        Robot = 3,
        Beast = 4,
    }
}
