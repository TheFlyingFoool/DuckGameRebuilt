using System;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;
using System.Net;

namespace Installer_Windows
{
    public partial class RebuiltInstaller : Form
    {
        private static RebuiltInstaller _current = null;
        private static string _dgPath = String.Empty;
        
        public RebuiltInstaller()
        {
            InitializeComponent();
            _current = this;
            DuckGameSearch();
        }

        private static void DuckGameSearch()
        {
            if (File.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Duck Game\\DuckGame.exe"))
            {
                _dgPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Duck Game\\";
                _current.lblAutoSearch.Text = "Duck Game Found! Press next to install.";
                _current.txtPath.Text = _dgPath;
                _current.btnInstall.Enabled = true;
            }
            else
                _current.lblAutoSearch.Text = "Duck Game isn't installed in the default location, please specify path to Duck Game installation (folder with DuckGame.exe).";

        }

        private void btnChoosePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialog.SelectedPath + "\\Content\\"))
                {
                    _dgPath = folderBrowserDialog.SelectedPath;
                    txtPath.Text = _dgPath;
                    _current.btnInstall.Enabled = true;
                }
                else
                {
                    MessageBox.Show($"Could not find content folder in {folderBrowserDialog.SelectedPath}", "Error", MessageBoxButtons.OK);
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            btnInstall.Enabled = false;
            btnChoosePath.Enabled = false;
            chkReplace.Enabled = false;
            string installPath = "";
            if (!chkReplace.Checked)
            {
                installPath = Directory.GetParent(Directory.GetParent(_dgPath).FullName).FullName + "\\Duck Game Rebuilt\\";
                Directory.CreateDirectory(installPath);
                Directory.CreateDirectory(installPath + "\\Content\\Shaders\\");
                Directory.CreateDirectory(installPath + "\\Content\\title\\");
            }
            else
            {
                installPath = _dgPath;
            }

            WebClient client = new WebClient();
            byte[] data = client.DownloadData(new Uri("https://klof44.games/rebuilt/DuckGameRebuilt.zip"));
            client.Dispose();
            Stream memStream = new MemoryStream(data);
            ZipArchive zip = new ZipArchive(memStream);
            foreach (var entry in zip.Entries)
            {
                switch (entry.Name)
                {
                    case "sbenergyBlade.xnb":
                    case "sbgold.xnb":
                        entry.ExtractToFile(Path.Combine(installPath + "\\Content\\Shaders\\", entry.FullName), true);
                        break;
                    case "logo.png":
                        entry.ExtractToFile(Path.Combine(installPath + "\\Content\\title\\", entry.FullName), true);
                        break;
                    default:
                        entry.ExtractToFile(Path.Combine(installPath, entry.FullName), true);
                        break;
                }
            }
            zip.Dispose();
            memStream.Dispose();

            if (!chkReplace.Checked)
            {
                foreach (string dirPath in Directory.GetDirectories(_dgPath + "\\Content\\", "*", SearchOption.AllDirectories)) 
                { 
                    Directory.CreateDirectory(dirPath.Replace(_dgPath + "\\Content\\", installPath + "\\Content\\"));
                }
                foreach (string newPath in Directory.GetFiles(_dgPath + "\\Content\\", "*.*",SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(_dgPath + "\\Content\\", installPath + "\\Content\\"), true);
                }
            }

            MessageBox.Show($"Duck Game Rebuilt installed in {installPath}", "Install Complete", MessageBoxButtons.OK);
            Application.Exit();
        }
    }
}