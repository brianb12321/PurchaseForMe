using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Code
{
    public class GetRunnerStatisticsMessage
    {
    }

    public class GetRunnerStatisticsResponseMessage
    {
        public int NumberOfRunnersUsed { get; }
        public int NumberOfRunnersAvailable { get; }

        public GetRunnerStatisticsResponseMessage(int numberOfRunnersUsed, int numberOfRunnersAvailable)
        {
            NumberOfRunnersUsed = numberOfRunnersAvailable;
            NumberOfRunnersAvailable = numberOfRunnersAvailable;
        }
    }
}