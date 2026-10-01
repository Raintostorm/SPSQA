namespace Project.Data
{
    /// <summary>
    /// Vai trò chiến đấu. Thay cho hệ 5 chủng tộc cũ.
    ///
    /// Mỗi cặp (UnitRole, Era) ứng với ĐÚNG MỘT UnitDefinition
    /// → 3 vai × 4 thời đại = 12 lính gốc (Mythic tính riêng).
    ///
    /// Evolution bốc NGẪU NHIÊN 1 trong 3 vai ở thời đại kế tiếp.
    /// Đừng chèn giá trị mới vào GIỮA: số đã serialize vào các file .asset.
    /// </summary>
    public enum UnitRole
    {
        Damage = 0,
        Range = 1,
        Tank = 2,
    }
}
