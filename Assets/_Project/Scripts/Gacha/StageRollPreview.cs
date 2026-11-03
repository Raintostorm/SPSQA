namespace Project.Gacha
{
    // Requested per-run Chapter 1-3 preview. Five buff cells resolve as Epic;
    // six would be Legendary. Spending/phase validation remains in RollService.
    public static class StageRollPreview
    {
        // Free opening review choices, once per fresh run. No currency or roll payout.
        public static Project.Data.Rank[] OpeningChoices(int stage,int wave)
        {
            if(stage!=3||wave!=1)return System.Array.Empty<Project.Data.Rank>();
            return new[]{Project.Data.Rank.Epic,Project.Data.Rank.Legendary,Project.Data.Rank.Epic,Project.Data.Rank.Legendary,Project.Data.Rank.Epic,Project.Data.Rank.Legendary};
        }
        public static RollCellKind[] FirstEpicBuff(int stage,int successfulRolls)
        {
            if(stage!=3||successfulRolls!=0)return null;
            return new[]{RollCellKind.Buff,RollCellKind.Buff,RollCellKind.Coin,
                RollCellKind.Buff,RollCellKind.Buff,RollCellKind.Buff};
        }
    }
}
