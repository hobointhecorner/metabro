using MB.Pref;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby.Api
{
    public class ApiClient
    {
        public enum RequestUrlType
        {
            Item,
            Library,
            PlaylistItem,
            Users,
            Tasks
        }
        public enum RequestParameterType
        {
            Library,
            Collection,
            Playlist,
            PlaylistItem,
            Movie,
            Series,
            Episode,
            DetailedEpisode
        }
        public EmbyPref Preferences { get; set; }

        public ApiClient()
        {
            Preferences = EmbyPref.GetPref();
        }
                
        string GetItemResource(bool UseUserId = true)
        {
            if (UseUserId && EmbyPref.GetPref().DefaultUserId != string.Empty) { return string.Format("users/{0}/items", EmbyPref.GetPref().DefaultUserId); }
            else { return "items"; }
        }
        string GetItemResource(string UserId)
        {
            return string.Format("users/{0}/items", UserId);
        }

        public string GetUrl(RequestUrlType UrlType, string additionalInfo = null)
        {
            string baseUrl = Preferences.Url;

            switch (UrlType)
            {
                case RequestUrlType.Item:
                        return string.Format("{0}/{1}", GetItemResource(additionalInfo));

                case RequestUrlType.Library:
                    return string.Format("{0}/Library/MediaFolders", baseUrl);

                case RequestUrlType.PlaylistItem:
                    return string.Format("{0}/Playlists/{1}/Items");

                case RequestUrlType.Tasks:
                    return string.Format("{0}/scheduledtasks/running/{1}", baseUrl, additionalInfo);

                case RequestUrlType.Users:
                    return string.Format("{0}/users", baseUrl);

                default:
                    return null;
            }
        }

        public static Dictionary<string, string> GetRequestParameter(RequestParameterType Type)
        {
            switch (Type)
            {
                case RequestParameterType.Library:
                    return new Dictionary<string, string>
                    {
                        { "SortBy", "Name" }
                    };

                case RequestParameterType.Collection:
                    return new Dictionary<string, string>
                    {
                        { "IncludeItemTypes", "BoxSet" },
                        { "Recursive" , "True" }
                    };

                case RequestParameterType.Playlist:
                    return new Dictionary<string, string>
                    {
                        { "IncludeItemTypes", "Playlist" },
                        { "UserId" , "" }
                    };

                case RequestParameterType.PlaylistItem:
                    return new Dictionary<string, string>
                    {
                        {"Ids", "" },
                        {"EntryIds", "" },
                        { "UserId", "" }
                    };

                case RequestParameterType.Movie:
                    return new Dictionary<string, string>
                    {
                        { "Limit", "500" },
                        { "ParentId", "" },
                        { "IncludeItemTypes", "Movie" },
                        { "Filters", "IsNotFolder" },
                        { "SortBy", "Name" },
                        { "Recursive", "True" }
                    };

                case RequestParameterType.Series:
                    return new Dictionary<string, string>
                    {
                        { "Limit", "100" },
                        { "ParentId", "" },
                        { "Ids", "" },
                        { "IncludeItemTypes", "Series" },
                        //{ "Filters", "IsNotFolder" },
                        { "SortBy", "Name" },
                        { "Recursive", "" }
                    };

                case RequestParameterType.Episode:
                    return new Dictionary<string, string>
                    {
                        { "Limit", "500" },
                        { "ParentId", "" },
                        { "IncludeItemTypes", "Episode" },
                        { "Filters", "IsNotFolder" },
                        { "SortBy", "ParentIndexNumber,IndexNumber" },
                        { "SortOrder","" },
                        { "Recursive", "True" },
                        { "IsMissing", "False" }
                    };

                case RequestParameterType.DetailedEpisode:
                    return new Dictionary<string, string>
                    {
                        { "Limit", "500" },
                        { "Id", "" },
                        { "UserId", "" },
                        { "Fields", "Path" },
                        { "SortBy", "ParentIndexNumber,IndexNumber" },
                        { "SortOrder","" },
                        { "IsMissing", "False" }
                    };

                default:
                    throw new Exception("Invalid ApiClient request parameter type");
            }
        }
        RestRequest AddRequestParameter(RestRequest Request, Dictionary<string, string> Parameter = null)
        {            
            Request.AddParameter("Api_Key", EmbyPref.GetPref().ApiKey);

            if (Request.Method == Method.GET)
            {
                if (Parameter != null)
                {
                    foreach (string parameterName in Parameter.Keys)
                    {
                        if (Parameter[parameterName] != null)
                        {
                            Request.AddParameter(parameterName, Parameter[parameterName]);
                        }
                    }
                }
            }
            else
            {
                Request.AddUrlSegment("Api_Key", EmbyPref.GetPref().ApiKey);
                if (Parameter != null)
                {
                    foreach (string parameterName in Parameter.Keys)
                    {
                        if (Parameter[parameterName] != null)
                        {
                            Request.AddParameter(parameterName, Parameter[parameterName]);
                            Request.AddUrlSegment(parameterName, Parameter[parameterName]);
                        }
                    }
                }
            }

            return Request;
        }
        RestRequest NewRequest(string Resource, Dictionary<string, string> Parameter = null, RestSharp.Method Method = RestSharp.Method.GET)
        {
            RestRequest request = new RestRequest(Resource, Method);
            request.RequestFormat = DataFormat.Json;
            request = AddRequestParameter(request, Parameter);

            if (Method == Method.GET)
                request.RootElement = "Items";

            return request;
        }
        RestRequest NewParameterizedRequest(string Resource, Method Method, Dictionary<string, string> Parameter = null)
        {
            RestRequest request = new RestRequest();
            request.Method = Method;
            request.RequestFormat = DataFormat.Json;

            if (Parameter != null)
            {
                string parameterString = "?";
                foreach (string parameterName in Parameter.Keys)
                {
                    if (Parameter[parameterName] != string.Empty)
                    {
                        parameterString += string.Format("{0}={{{0}}}&", parameterName);
                    }
                }

                parameterString += "Api_Key={Api_Key}";
                Resource += parameterString;
            }
            else
            {
                Resource += "?Api_Key={Api_Key}";
            }

            request.Resource = Resource;
            request = AddRequestParameter(request, Parameter);

            return request;
        }

        void InvokeRequest(RestRequest Request)
        {
            RestClient client = new RestClient(EmbyPref.GetPref().Url);
            var response = client.Execute(Request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var e = new ApplicationException(message, response.ErrorException);
                throw e;
            }
        }
        T InvokeRequest<T>(RestRequest Request) where T : new()
        {

            RestClient client = new RestClient(EmbyPref.GetPref().Url);
            var response = client.Execute<T>(Request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var e = new ApplicationException(message, response.ErrorException);
                throw e;
            }

            return response.Data;
        }
                
        public List<EmbyLibrary> GetLibrary(Dictionary<string, string> Parameter)
        {
            RestRequest request = NewRequest("Library/MediaFolders", Parameter);

            List<EmbyLibrary> result = InvokeRequest<List<EmbyLibrary>>(request);
            return result;
        }

        public List<EmbyCollection> GetCollection(Dictionary<string, string> Parameter)
        {
            string resource = GetItemResource();
            RestRequest request = NewRequest(resource, Parameter);

            List<EmbyCollection> result = InvokeRequest<List<EmbyCollection>>(request);
            return result;
        }
        public List<EmbyCollectionItem> GetCollectionItem(Dictionary<string, string> Parameter)
        {
            string resource = GetItemResource();
            RestRequest request = NewRequest(resource, Parameter);

            List<EmbyCollectionItem> result = InvokeRequest<List<EmbyCollectionItem>>(request);
            return result;
        }
        
        public List<EmbyPlaylist> GetPlaylist(Dictionary<string, string> Parameter, string UserId = null)
        {
            string resource = string.Empty;
            if (UserId == null)
            {
                resource = GetItemResource();
            }
            else
            {
                resource = GetItemResource(UserId);
            }
            RestRequest request = NewRequest(resource, Parameter);

            List<EmbyPlaylist> result = InvokeRequest<List<EmbyPlaylist>>(request);
            return result;
        }
        public List<EmbyPlaylistItem> GetPlaylistItem(string PlaylistId)
        {
            RestRequest request = NewRequest(string.Format("Playlists/{0}/Items", PlaylistId));

            List<EmbyPlaylistItem> result = InvokeRequest<List<EmbyPlaylistItem>>(request);
            return result;
        }
        public void AddPlaylistItem(string PlaylistId, Dictionary<string, string> Parameter)
        {
            string resource = string.Format("playlists/{0}/Items", PlaylistId);
            RestRequest request = NewParameterizedRequest(resource, Method.POST, Parameter);

            InvokeRequest(request);
        }
        public void RemovePlaylistItem(string PlaylistId, Dictionary<string, string> Parameter)
        {
            string resource = string.Format("playlists/{0}/Items", PlaylistId);
            RestRequest request = NewParameterizedRequest(resource, Method.DELETE, Parameter);

            InvokeRequest(request);
        }
        
        public List<EmbyMovie> GetMovie(Dictionary<string, string> Parameter)
        {
            string resource = GetItemResource();
            RestRequest request = NewRequest(resource, Parameter);

            List<EmbyMovie> result = InvokeRequest<List<EmbyMovie>>(request);
            return result;
        }
        public List<EmbySeries> GetSeries(Dictionary<string, string> Parameter)
        {
            string resource = GetItemResource();
            RestRequest request = NewRequest(resource, Parameter);

            List<EmbySeries> result = InvokeRequest<List<EmbySeries>>(request);
            return result;
        }
        public List<EmbySeries> GetSeries(string Id, Dictionary<string, string> Parameter)
        {
            Parameter["Ids"] = Id;
            string resource = GetItemResource();
            RestRequest request = NewRequest(resource, Parameter);

            List<EmbySeries> result = InvokeRequest<List<EmbySeries>>(request);
            return result;
        }
        public List<EmbyEpisode> GetEpisode(Dictionary<string, string> Parameter)
        {
            string resource = GetItemResource();
            RestRequest request = NewRequest(resource, Parameter);

            List<EmbyEpisode> result = InvokeRequest<List<EmbyEpisode>>(request);
            return result;
        }
        public List<EmbyEpisodeDetail> GetEpisodeDetail(Dictionary<string, string> Parameter)
        {
            string resource = GetItemResource();
            RestRequest request = NewRequest(resource, Parameter);

            List<EmbyEpisodeDetail> result = InvokeRequest<List<EmbyEpisodeDetail>>(request);
            return result;
        }
        
        public List<EmbyUser> GetUser()
        {
            RestRequest request = NewRequest("users");
            return InvokeRequest<List<EmbyUser>>(request);
        }

        public List<EmbyTask> GetTask(bool Running = false)
        {
            string resource;
            if (Running)
                resource = "scheduledtasks/running";
            else
                resource = "scheduledtasks";

            RestRequest request = NewRequest(resource);
            return InvokeRequest<List<EmbyTask>>(request);
        }
        public void StartTask(string TaskId)
        {
            string resource = string.Format("scheduledtasks/running/{0}", TaskId);
            RestRequest request = NewParameterizedRequest(resource, Method.POST);
            InvokeRequest(request);
        }
    }
}
