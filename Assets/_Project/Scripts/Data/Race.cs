namespace Project.Data
{
    /// <summary>
    /// Giống loài. Nhánh NGANG — đổi giống không làm con lính mạnh lên.
    ///
    /// Thay cho Variant cũ. Thứ tự KHÔNG được đổi: giá trị số đã nằm trong
    /// các file .asset, chèn vào giữa là mọi con lính đổi giống.
    /// </summary>
    public enum Race
    {
        Human = 0,
        Elf = 1,
        Dead = 2,
        Monster = 3,
        Machine = 4,
    }
}
