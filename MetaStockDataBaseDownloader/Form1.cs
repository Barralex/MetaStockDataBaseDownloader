using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace MetaStockDataBaseDownloader
{
    public partial class Form1 : Form
    {

        public string filename { get; set; }

        WebClient client;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            DeleteFolderContent();
            DownloadFile();
        }

        private void DownloadFile()
        {

            client = new WebClient();

            listBox1.Items.Add(string.Format("Download start at {0}", DateTime.Now));

            button1.Enabled = false;
            button2.Enabled = true;

            string desktopPath = @"C:\MetaStock Data\Acciones";

            string url = "http://resources.invertironline.com/iolresearch/IOL_Metastock.zip";

            filename = GetFilename(url);

            using (client)
            {
                client.DownloadProgressChanged += Wc_DownloadProgressChanged;
                client.DownloadFileCompleted += Wc_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(url), desktopPath + "/" + filename);
            }
        }

        private string GetFilename(string hreflink)
        {
            Uri uri = new Uri(hreflink);

            string filename = System.IO.Path.GetFileName(uri.LocalPath);

            return filename;
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " +
                e.TotalBytesToReceive + " bytes retrieven.";
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            toolStripProgressBar1.Value = 0;

            button1.Enabled = true;
            button2.Enabled = false;

            if (e.Cancelled)
            {
                listBox1.Items.Add("The download has been cancelled");

                return;
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {
                listBox1.Items.Add("An error ocurred while trying to download file");

                return;
            }

            listBox1.Items.Add(string.Format("Download completed at {0}", DateTime.Now));

            UnZipFile();

        }

        private void CancelDownload()
        {
            client.CancelAsync();
        }

        private void UnZipFile()
        {
            listBox1.Items.Add(string.Format("Unzipping downloaded file {0}", filename));

            string zipPath = @"C:\MetaStock Data\Acciones\IOL_Metastock.zip";
            string extractPath = @"C:\MetaStock Data\Acciones";

            ZipFile.ExtractToDirectory(zipPath, extractPath);

            listBox1.Items.Add(string.Format("Unzipping completed"));

            listBox1.Items.Add(string.Format("Deleting downloaded file"));

            DeleteFile();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            CancelDownload();
        }

        private void DeleteFolderContent()
        {
            listBox1.Items.Add(string.Format("Deleting folder content"));
            Array.ForEach(Directory.GetFiles(@"C:\MetaStock Data\Acciones"), File.Delete);
        }

        private void DeleteFile()
        {
            listBox1.Items.Add(string.Format("Deleting file {0}", filename));
            File.Delete(string.Format(@"C:\MetaStock Data\Acciones\{0}", filename));
        }

    }
}
