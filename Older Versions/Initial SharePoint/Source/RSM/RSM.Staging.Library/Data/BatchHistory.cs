using System;
using RSM.Support;

namespace RSM.Staging.Library.Data
{
    public class BatchHistory
    {
        public static Support.BatchHistory S2In1 = Factory.CreateBatchHistory(1, ExternalSystems.S2In.Id, DateTime.Parse("01/10/2013"), DateTime.Parse("01/10/2013"));
        public static Support.BatchHistory S2In2 = Factory.CreateBatchHistory(1, ExternalSystems.S2In.Id, DateTime.Parse("01/09/2013"), DateTime.Parse("01/09/2013"), null, "Errors found", BatchOutcome.DataError);

    }
}
