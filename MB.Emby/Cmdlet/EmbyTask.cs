using MB.Emby.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MB.Emby
{
    public class EmbyTask : EmbyItem
    {
        public string State { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }

        public EmbyTask(string TaskId)
        {
            this.Id = TaskId;
        }

        public EmbyTask(Api.EmbyTask ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
            this.State = ApiObject.State;
            this.Description = ApiObject.Description;

            if (ApiObject.IsHidden == "True")
                this.Hidden = true;
            else
                this.Hidden = false;
        }

        public void StartTask(ApiClient ApiClient = null)
        {
            if (ApiClient == null)
                ApiClient = new ApiClient();

            ApiClient.StartTask(Id);
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyTask))]
    [OutputType(typeof(EmbyTask))]
    public class GetEmbyTaskCmdlet : PSCmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; } = "*";

        [Parameter]
        public bool Running { get; set; } = false;

        ApiClient ApiClient;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            ApiClient = new ApiClient();
        }

        protected override void ProcessRecord()
        {
            string regString = string.Format("^{0}$", Regex.Escape(Name.ToLower()).Replace("\\*", ".*"));
            Regex matchRegex = new Regex(regString);

            foreach (Api.EmbyTask apiTaskObject in ApiClient.GetTask(Running))
            {
                if (matchRegex.IsMatch(apiTaskObject.Name.ToLower()))
                    WriteObject(new EmbyTask(apiTaskObject));
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Start, nameof(EmbyTask))]
    public class StartEmbyTaskCmdlet : PSCmdlet
    {
        [Parameter(ParameterSetName = "Task", Mandatory = true, ValueFromPipeline = true)]
        public List<EmbyTask> Task { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string Name { get; set; }
        
        ApiClient ApiClient;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            ApiClient = new ApiClient();
        }

        protected override void ProcessRecord()
        {
            if (Task != null)
            {
                foreach (EmbyTask apiTaskObject in Task)
                {
                    apiTaskObject.StartTask(ApiClient);
                }
            }
            else if (Name != null)
            {
                string regString = string.Format("^{0}$", Regex.Escape(Name.ToLower()).Replace("\\*", ".*"));
                Regex matchRegex = new Regex(regString);

                foreach (Api.EmbyTask apiTaskObject in ApiClient.GetTask())
                {
                    if (matchRegex.IsMatch(apiTaskObject.Name.ToLower()))
                        ApiClient.StartTask(apiTaskObject.Id);
                }
            }
        }
    }
}
