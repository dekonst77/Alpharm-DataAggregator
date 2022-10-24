namespace DataAggregator.Web.Managers
{
    public class ComplexHeaderDescription
    {
        public ComplexHeaderPeriod Period { get; private set; }
        public int Count { get; private set; }
        public int StartColumn { get; private set; }
        public int EndColumn { get; private set; }

        public ComplexHeaderDescription(ComplexHeaderPeriod period, int startColumn)
        {
            Period = period;

            Count = period.StartDate.MonthDistance(period.EndDate) + 1;

            StartColumn = startColumn;
            EndColumn = StartColumn + Count - 1;
        }
    }
}