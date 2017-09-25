using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management.Automation;

namespace MB.Logging
{
    public enum LogType
    {
        Information,
        Warning,
        Error
    }

    public static class Logger
    {
        public static void WriteLog(string Message, string Source, LogType LogType = LogType.Information, string LogName = "Metabro")
        {
            if (!EventLog.SourceExists(Source))
                EventLog.CreateEventSource(Source, LogName);

            EventLogEntryType entryType;

            switch (LogType)
            {
                case LogType.Warning:
                    entryType = EventLogEntryType.Warning;
                    break;

                case LogType.Error:
                    entryType = EventLogEntryType.Error;
                    break;

                default:
                    entryType = EventLogEntryType.Information;
                    break;
            }

            EventLog.WriteEntry(Source, Message, entryType);
        }
    }

    [Cmdlet(VerbsCommon.New, "MBEventLog")]
    public class NewMBEventLogCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Message { get; set; }

        [Parameter(Mandatory = true)]
        public string Source { get; set; }

        [Parameter]
        public LogType LogType { get; set; } = LogType.Information;

        [Parameter]
        public string LogName { get; set; } = "Metabro";

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            Logger.WriteLog(Message, Source, LogType, LogName);
        }
    }

    [Cmdlet(VerbsCommunications.Write, "MBOutput")]
    public class WriteMBOutputCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Output { get; set; }

        [Parameter()]
        public LogType OutputType { get; set; } = LogType.Information;

        [Parameter()]
        public SwitchParameter PassThru { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            switch (OutputType)
            {
                case LogType.Error:
                    break;

                case LogType.Warning:
                    WriteWarning(Output);
                    break;

                case LogType.Information:
                    WriteInformation(new InformationRecord(Output, "Metabro"));
                    break;
            }

            if (PassThru)
            {
                WriteObject(Output);
            }
        }
    }

    [Cmdlet(VerbsCommunications.Write, "MBError")]
    public class WriteMBErrorCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public ErrorRecord Exception { get; set; }

        [Parameter()]
        public SwitchParameter WriteEventLog { get; set; }

        [Parameter()]
        public string EventMessage { get; set; } = string.Empty;

        [Parameter()]
        public string EventSource { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            WriteError(Exception);

            if (WriteEventLog)
            {
                string errorMessage = string.Format("{0}\n{1}\n{2}", EventMessage, Exception.Exception.Message, Exception.InvocationInfo.PositionMessage);
                Logger.WriteLog(errorMessage, EventSource, LogType.Error);
            }
        }
    }
}
