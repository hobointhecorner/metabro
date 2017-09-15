using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MB.Emby.Api;

namespace MB.Emby
{
    public class EmbyUser : EmbyItem
    {
        public bool HasPassword { get; set; }
        public DateTime LastActivityDate { get; set; }

        public EmbyUser(string UserId)
        {
            this.Id = UserId;
        }

        public EmbyUser(Api.EmbyUser ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
            this.HasPassword = ApiObject.HasPassword;

            if (ApiObject.LastActivityDate != null)
                this.LastActivityDate = DateTime.Parse(ApiObject.LastActivityDate);
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyUser))]
    [OutputType(typeof(EmbyUser))]
    public class GetEmbyUserCmdlet : PSCmdlet
    {
        [Parameter(Position = 0)]
        public string Name { get; set; } = "*";

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

            foreach (Api.EmbyUser apiUserObject in ApiClient.GetUser())
            {
                if (matchRegex.IsMatch(apiUserObject.Name.ToLower()))
                    WriteObject(new EmbyUser(apiUserObject));
            }
        }
    }
}
