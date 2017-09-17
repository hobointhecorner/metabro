﻿using System;
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
}
