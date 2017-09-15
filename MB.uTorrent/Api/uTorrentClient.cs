using MB.Pref;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTorrentAPI;

namespace MB.uTorrent.Api
{
    public class uTorrentClient
    {
        public uTorrentPref Preferences { get; set; }
        UTorrentAPI.UTorrentClient ApiClient;

        public uTorrentClient()
        {
            Preferences = uTorrentPref.GetPref();
            ApiClient = new UTorrentClient((new Uri(Preferences.Url)), Preferences.Username, Preferences.Password);
        }

        public void Dispose()
        {
            ApiClient.Dispose();
        }

        public TorrentCollection GetTorrent()
        {
            return ApiClient.Torrents;
        }

        public UTorrentAPI.Torrent GetTorrent(string Hash)
        {
            var torrentList = from torrent in GetTorrent()
                              where torrent.Hash == Hash
                              select torrent;

            foreach (UTorrentAPI.Torrent torrent in torrentList)
                return torrent;

            return null;
        }

        public void AddTorrent(string Path, MB.uTorrent.Torrent.FileType FileType, string SavePath = null, bool DeleteFile = false)
        {
            switch (FileType)
            {
                case MB.uTorrent.Torrent.FileType.Magnet:
                    ApiClient.Torrents.AddUrl(Path, SavePath);
                    break;

                case MB.uTorrent.Torrent.FileType.Torrent:
                    ApiClient.Torrents.AddFile(@Path, SavePath);

                    if (DeleteFile && System.IO.File.Exists(@Path))
                        System.IO.File.Delete(@Path);

                    break;
            }
        }

        public void RemoveTorrent(UTorrentAPI.Torrent Torrent, UTorrentAPI.TorrentRemovalOptions RemovalOption)
        {
            ApiClient.Torrents.Remove(Torrent, RemovalOption);
        }

        public void RemoveTorrent(string Hash, UTorrentAPI.TorrentRemovalOptions RemovalOption)
        {
            ApiClient.Torrents.Remove(Hash, RemovalOption);
        }

        public DirectoryCollection GetStorageDirectory()
        {
            return ApiClient.StorageDirectories;
        }
    }
}
