namespace FinanceTracker.Models
{
    public class HomeViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<Transactions> Transactions { get; set; } = new();

        public string[] ChartLabels { get; set; } = Array.Empty<string>();
        public int[] IncomeSeries { get; set; }
        public int[] ExpenseSeries { get; set; }
        public DateTime NextPayday { get; set; }
        public int DaysUntilNextPayday { get; set; }
        public decimal ExpensesThisPeriod { get; set; }
        public decimal IncomeThisPeriod { get; set; }
        public decimal DailyExpenseRate { get; set; }
        public decimal SavingsGoal { get; set; } 
        public decimal PotentialSavings { get; set; }
    }
}
