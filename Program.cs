using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Net.Torrent;


namespace tordle
{
    class Program
    {
        private static void PrintUsage() {
            Console.WriteLine("USAGE: ClientOutputDirectory TorrentDirectory AuditMode");
        }
        static void Main(string[] args)
        {
            if (args.Length != 3) {
                PrintUsage();
                return; 
            }

            string filePath = args[0];
            string torrentDirectory = args[1];
            bool auditMode = true;
            if (!Boolean.TryParse(args[2], out auditMode)) {
                PrintUsage();
                return;
            }

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
                Console.WriteLine($"{((auditMode) ? "AUDIT MODE: " : "")} Deleting {fsEntry}...");
                try { 
                    if (File.Exists(fsEntry) && !auditMode)
                        File.Delete(fsEntry);
                    if (Directory.Exists(fsEntry) && !auditMode)
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
