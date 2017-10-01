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
    public class EmbyPlaylist : EmbyItem
    {
        public EmbyPlaylist(string Id)
        {
            this.Id = Id;
        }

        public EmbyPlaylist(Api.EmbyPlaylist ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
        }

        public static List<EmbyPlaylist> GetPlaylist(Dictionary<string,string> Parameter, ApiClient ApiClient = null)
        {
            List<EmbyPlaylist> outputList = new List<EmbyPlaylist>();
            if (ApiClient == null)
                ApiClient = new ApiClient();

            foreach (Api.EmbyPlaylist apiPlaylistObject in ApiClient.GetPlaylist(Parameter))
                outputList.Add(new EmbyPlaylist(apiPlaylistObject));

            return outputList;
        }

        public List<Api.EmbyPlaylistItem> GetPlaylistItem(ApiClient ApiClient = null)
        {
            List<Api.EmbyPlaylistItem> PlaylistItemList = new List<Api.EmbyPlaylistItem>();

            if (ApiClient == null)
                ApiClient = new ApiClient();

            foreach (Api.EmbyPlaylistItem apiPlaylistItem in ApiClient.GetPlaylistItem(Id))
                PlaylistItemList.Add(apiPlaylistItem);

            return PlaylistItemList;
        }
    }

    public class EmbyPlaylistItem : EmbyItem
    {
        public string PlaylistItemId { get; set; }

        public EmbyPlaylistItem(string Id)
        {
            this.PlaylistItemId = Id;
        }

        public EmbyPlaylistItem(Api.EmbyPlaylistItem ApiObject)
        {
            this.Name = ApiObject.Name;
            this.Id = ApiObject.Id;
            this.PlaylistItemId = ApiObject.PlaylistItemId;
            this.Type = ApiObject.Type;
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyPlaylist))]
    [OutputType(typeof(EmbyPlaylist))]
    public class GetEmbyPlaylistCmdlet : PSCmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; } = "*";

        [Parameter(ValueFromPipeline = true)]
        public EmbyUser User { get; set; }

        [Parameter]
        public SwitchParameter DefaultUser { get; set; } = true;

        ApiClient ApiClient;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            ApiClient = new ApiClient();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            string regString = string.Format("^{0}$", Regex.Escape(Name.ToLower()).Replace("\\*", ".*"));
            Regex matchRegex = new Regex(regString);
            Dictionary<string, string> requestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Playlist);

            List<Api.EmbyPlaylist> apiPlaylistList = new List<Api.EmbyPlaylist>();
            
            if (User != null)
            {
                requestParam["UserId"] = User.Id;
                apiPlaylistList = ApiClient.GetPlaylist(requestParam, User.Id);
            }
            else if (DefaultUser)
            {
                requestParam["UserId"] = EmbyPref.GetPref().DefaultUserId;
                apiPlaylistList = ApiClient.GetPlaylist(requestParam, EmbyPref.GetPref().DefaultUserId);
            }
            else
            {
                apiPlaylistList = ApiClient.GetPlaylist(requestParam);
            }

            foreach (Api.EmbyPlaylist apiPlaylistObject in apiPlaylistList)
            {
                if (matchRegex.IsMatch(apiPlaylistObject.Name.ToLower()))
                    WriteObject(new EmbyPlaylist(apiPlaylistObject));
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyPlaylistItem))]
    [OutputType(typeof(EmbyPlaylistItem))]
    public class GetEmbyPlaylistItemCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<EmbyPlaylist> Playlist { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (EmbyPlaylist playlistObject in Playlist)
            {
                foreach (Api.EmbyPlaylistItem apiPlaylistItem in playlistObject.GetPlaylistItem())
                {
                    WriteObject(new EmbyPlaylistItem(apiPlaylistItem));
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Add, nameof(EmbyPlaylistItem))]
    public class AddEmbyPlaylistItemCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<EmbyPlaylistItem> EmbyItem { get; set; }

        [Parameter(Mandatory = true)]
        public EmbyPlaylist Playlist { get; set; }

        [Parameter(Mandatory = true)]
        public EmbyUser User { get; set; }

        ApiClient ApiClient;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            ApiClient = new ApiClient();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            string idString = string.Empty;
            foreach (EmbyItem item in EmbyItem)
            {
                idString += string.Format("{0},", item.Id);
            }
            idString = idString.Remove(idString.Length - 1, 1);

            Dictionary<string, string> requestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.PlaylistItem);
            requestParam["UserId"] = User.Id;
            requestParam["Ids"] = idString;

            ApiClient.AddPlaylistItem(Playlist.Id, requestParam);
        }
    }

    [Cmdlet(VerbsCommon.Remove, nameof(EmbyPlaylistItem))]
    public class RemoveEmbyPlaylistItemCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<EmbyPlaylistItem> EmbyItem { get; set; }

        [Parameter(Mandatory = true)]
        public EmbyPlaylist Playlist { get; set; }

        [Parameter(Mandatory = true)]
        public EmbyUser User { get; set; }

        ApiClient ApiClient;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            ApiClient = new ApiClient();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            string idString = string.Empty;
            foreach (EmbyPlaylistItem item in EmbyItem)
            {
                idString += string.Format("{0},", item.PlaylistItemId);
            }
            idString = idString.Remove(idString.Length - 1, 1);

            Dictionary<string, string> requestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.PlaylistItem);
            requestParam["UserId"] = User.Id;
            requestParam["EntryIds"] = idString;

            ApiClient.RemovePlaylistItem(Playlist.Id, requestParam);
        }
    }
}


