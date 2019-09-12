using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Net.Torrent;


namespace tordle
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "/data/transmission-downloads/";
            string torrentDirectory = "/var/lib/transmission-daemon/info/torrents";

            var fileSystemEntryList = Directory.EnumerateFileSystemEntries(filePath);
            var torrentList = Directory.EnumerateFiles(torrentDirectory);
            var torrentSerializer = new TorrentSerializer();

            
            var fsEntryQuery = fileSystemEntryList.Where(file => {
                var fileName = file.Replace("/data/transmission-downloads/", "");
                return !(torrentList.Select( torrentFile => 
                                                torrentSerializer.Deserialize(File.Open(torrentFile, FileMode.Open,
                                                                           FileAccess.Read, FileShare.Read)))
                                    .Where(t => t.Info.Name == fileName).Any());
            });
            foreach (var fsEntry in fsEntryQuery) {
                Console.WriteLine($"Deleting {fsEntry}...");
                try { 
                    if (File.Exists(fsEntry))
                        File.Delete(fsEntry);
                    if (Directory.Exists(fsEntry))
                        Directory.Delete(fsEntry, true);
                } 
                catch {
                    Console.WriteLine( $"\tFailed to delete {fsEntry}..."); 
                }
                if (!File.Exists(fsEntry) && !Directory.Exists(fsEntry)) { Console.WriteLine($"\tSuccesfully deleted {fsEntry}"); }
           }
         }
    }
}
