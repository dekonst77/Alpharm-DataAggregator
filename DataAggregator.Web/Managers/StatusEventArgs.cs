using System;

namespace DataAggregator.Web.Managers
{
    public class StatusEventArgs : EventArgs
    {
        public string Text { get; set; }

        public int Number { get; set; }
    }
}