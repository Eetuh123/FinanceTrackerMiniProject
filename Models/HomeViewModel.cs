namespace FinanceTracker.Models
{
    public class HomeViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<Transactions> Transactions { get; set; } = new();

        public string[] ChartLabels { get; set; } = Array.Empty<string>();
        public int[] IncomeSeries { get; set; }
        public int[] ExpenseSeries { get; set; }
    }
}
