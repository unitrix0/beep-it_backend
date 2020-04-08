namespace BeepBackend.Models
{
    public class ShoppingListGroupEntry
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int EnvironmentId { get; set; }
        public int KeepStockAmount { get; set; }
        public int OnStock { get; set; }
        public int Needed { get; set; }
    }
}