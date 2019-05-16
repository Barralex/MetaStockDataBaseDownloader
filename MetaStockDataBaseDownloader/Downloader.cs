using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using System.Configuration;
using System.Threading.Tasks;

namespace MetaStockDataBaseDownloader
{
    public partial class Downloader : Form
    {

        public string FileName { get; set; }
        public WebClient Client { get; set; }

        public Downloader()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add(string.Format("Deleting folder content"));
            FileManagement.CreateStorageFolder();
            FileManagement.DeleteFolderContent();
            DownloadFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Download_btn.Enabled = true;
            Cancel_btn.Enabled = false;
            CancelDownload();
        }

        private void DownloadFile()
        {
            string desktopPath = ConfigurationManager.AppSettings["LocalStoragePath"];

            Client = new WebClient();

            listBox1.Items.Add(string.Format("Download start at {0}", DateTime.Now));

            Download_btn.Enabled = false;
            Cancel_btn.Enabled = true;

            string url = ConfigurationManager.AppSettings["MetaStockUri"];
            FileName = FileManagement.GetFilename(url);

            using (Client)
            {
                Client.DownloadProgressChanged += Wc_DownloadProgressChanged;
                Client.DownloadFileCompleted += Wc_DownloadFileCompletedAsync;
                Client.DownloadFileAsync(new Uri(url), desktopPath + "/" + FileManagement.GetFilename(url));
            }
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " +
                e.TotalBytesToReceive + " bytes retrieven.";
        }

        private async void Wc_DownloadFileCompletedAsync(object sender, AsyncCompletedEventArgs e)
        {
            toolStripProgressBar1.Value = 0;

            if (e.Cancelled)
            {
                listBox1.Items.Add("The download has been cancelled");
                return;
            }

            if (e.Error != null)
            {
                listBox1.Items.Add("An error ocurred while trying to download file");
                return;
            }

            listBox1.Items.Add(string.Format("Download completed at {0}", DateTime.Now));
            listBox1.Items.Add(string.Format("Unzipping downloaded file {0}", FileName));

            await Task.Run(() => FileManagement.UnZipFile());

            listBox1.Items.Add(string.Format("Unzipping completed"));
            listBox1.Items.Add(string.Format("Deleting downloaded file"));

            await Task.Run(() => FileManagement.DeleteFile(FileName));

            listBox1.Items.Add(string.Format("Deleting file {0}", FileName));

            Download_btn.Enabled = true;
            Cancel_btn.Enabled = false;

        }

        private void CancelDownload()
        {
            Client.CancelAsync();
        }

    }
}
