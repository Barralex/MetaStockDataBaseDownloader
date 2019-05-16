using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;

namespace MetaStockDataBaseDownloader
{
    public static class FileManagement
    {

        public static void UnZipFile()
        {
            ZipFile.ExtractToDirectory(ConfigurationManager.AppSettings["LocalStorageZipPath"],
                ConfigurationManager.AppSettings["LocalStoragePath"]);
        }

        public static void DeleteFolderContent()
        {
            Array.ForEach(Directory.GetFiles(ConfigurationManager.AppSettings["LocalStoragePath"]), File.Delete);
        }

        public static void CreateStorageFolder()
        {
            Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalStoragePath"]);
        }

        public static void DeleteFile(string filename)
        {
            File.Delete($"{ConfigurationManager.AppSettings["LocalStoragePath"]}\\{filename}");
        }

        public static string GetFilename(string uri)
        {
            return Path.GetFileName(new Uri(uri).LocalPath);
        }

    }
}
