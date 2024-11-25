namespace oa.Models
{
    public class ChartMonth
    {
        public ChartMonth() { }
        public int? month { get; set; }
        public string? monthName { get; set; }
        public double? enpensesTotal { get; set; } = 0;
        public double? revenueTotal { get; set; } = 0;
    }
}
