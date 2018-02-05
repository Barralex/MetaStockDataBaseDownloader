using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;

namespace MetaStockDataBaseDownloader
{
    public partial class Form1 : Form
    {

        private FileManagement FileManagement;

        private string filename;

        private WebClient client;

        public Form1()
        {
            InitializeComponent();
            FileManagement = new FileManagement();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add(string.Format("Deleting folder content"));
            FileManagement.DeleteFolderContent();
            DownloadFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            CancelDownload();
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

            listBox1.Items.Add(string.Format("Unzipping downloaded file {0}", filename));

            FileManagement.UnZipFile();

            listBox1.Items.Add(string.Format("Unzipping completed"));

            listBox1.Items.Add(string.Format("Deleting downloaded file"));

            FileManagement.DeleteFile(filename);

            listBox1.Items.Add(string.Format("Deleting file {0}", filename));

        }

        private void CancelDownload()
        {
            client.CancelAsync();
        }

    }
}
