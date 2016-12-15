using System;
using RSM.Artifacts.Log;
using RSM.Support;

namespace RSM.Models.Admin
{
    public class BosiStatusModel
    {
        public DateTime LastAction { get; set; }
        public int SystemId { get; set; }
        public string SystemName { get; set; }
        public ExternalSystemDirection Direction { get; set; }
        public int Severity { get; set; }
        public string SeverityName { get; set; }
        public int LogCount { get; set; }
        public BatchOutcome Outcome { get; set; }
        public string Message { get; set; }

        public BosiStatusModel()
        {
            
        }

        public BosiStatusModel(ExternalSystem system, Severity severity, DateTime lastAction)
        {
            SystemId = system.Id;
            SystemName = system.Name;
            Direction = (ExternalSystemDirection)system.Direction;
            Severity = (int) severity;
            SeverityName = LogEntry.GetSeverityName(severity);
            LastAction = lastAction;
        }

        public BosiStatusModel(ExternalSystem system, Severity severity, BatchHistory lastBatch)
        {
            var defaultDate = DateTime.Parse("01/01/1970");

            SystemId = system.Id;
            SystemName = system.Name;
            Direction = (ExternalSystemDirection)system.Direction;
            Severity = (int)severity;
            SeverityName = LogEntry.GetSeverityName(severity);

            if (lastBatch == null)
            {
                LastAction = defaultDate;
                Outcome = BatchOutcome.Success;
                Message = "Success";
            }
            else
            {
                LastAction = lastBatch.RunEnd;
                Outcome = BatchOutcome.DataError;
                Message = lastBatch.Message;
            }
        }
    }
}