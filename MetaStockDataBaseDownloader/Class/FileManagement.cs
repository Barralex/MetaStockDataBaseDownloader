using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaStockDataBaseDownloader
{
    public class FileManagement
    {

        private string zipPath = @"C:\MetaStock Data\Acciones\IOL_Metastock.zip";
        private string extractPath = @"C:\MetaStock Data\Acciones";

        public FileManagement() { }

        public void UnZipFile()
        {
            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }

        public void DeleteFolderContent()
        {
            Array.ForEach(Directory.GetFiles(extractPath), File.Delete);
        }

        public void DeleteFile(string filename)
        {
            File.Delete(string.Format(@"C:\MetaStock Data\Acciones\{0}", filename));
        }

    }
}
