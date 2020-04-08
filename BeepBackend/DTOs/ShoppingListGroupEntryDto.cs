namespace BeepBackend.DTOs
{
    public class ShoppingListGroupEntryDto
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int EnvironmentId { get; set; }
        public int KeepStockAmount { get; set; }
        public int OnStock { get; set; }
        public int Needed { get; set; }
    }
}