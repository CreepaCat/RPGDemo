namespace RPGDemo.Inventories.Tooltips
{
    public interface ISpawnTooltip
    {
        bool CanCreateTooltip();


        void CreateTooltip();
        void ClearTooltip();
    }
}