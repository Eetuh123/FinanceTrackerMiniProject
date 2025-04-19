namespace FinanceTracker.Models
{
    public class HomeViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<Transactions> Transactions { get; set; } = new();
    }
}
