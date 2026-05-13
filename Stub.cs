using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using Microsoft.Win32;
using System.Net;

namespace SoftPortal {
    public class InstallerForm : Form {
        private ListView appList;
        private ProgressBar progressBar;
        private Label statusLabel;
        private List<string> appIds;
        private BackgroundWorker worker;
        private Button btnClose;

        public InstallerForm() {
            InitializeUI();
            ParseApps();
        }

        private void InitializeUI() {
            this.Text = "SoftPortal Installer";
            this.Size = new Size(450, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 15, 15);
            this.ForeColor = Color.White;

            Label title = new Label();
            title.Text = "SoftPortal";
            title.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            title.Location = new Point(20, 20);
            title.Size = new Size(200, 40);
            title.ForeColor = Color.FromArgb(0, 112, 243);

            statusLabel = new Label();
            statusLabel.Text = "Preparing to install...";
            statusLabel.Location = new Point(20, 65);
            statusLabel.Size = new Size(400, 20);
            statusLabel.Font = new Font("Segoe UI", 9);

            appList = new ListView();
            appList.Location = new Point(20, 100);
            appList.Size = new Size(395, 280);
            appList.View = View.Details;
            appList.FullRowSelect = true;
            appList.BackColor = Color.FromArgb(25, 25, 25);
            appList.ForeColor = Color.White;
            appList.BorderStyle = BorderStyle.None;
            appList.HeaderStyle = ColumnHeaderStyle.None;
            appList.Columns.Add("App", 250);
            appList.Columns.Add("Status", 120);

            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 400);
            progressBar.Size = new Size(395, 20);
            progressBar.Style = ProgressBarStyle.Continuous;

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new Point(335, 430);
            btnClose.Size = new Size(80, 30);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Visible = false;
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btnClose.Click += (s, e) => Application.Exit();

            this.Controls.Add(title);
            this.Controls.Add(statusLabel);
            this.Controls.Add(appList);
            this.Controls.Add(progressBar);
            this.Controls.Add(btnClose);

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void ParseApps() {
            string filePath = Process.GetCurrentProcess().MainModule.FileName;
            string filename = Path.GetFileNameWithoutExtension(filePath);
            string[] parts = filename.Split('_');
            appIds = new List<string>();
            
            for (int i = 1; i < parts.Length; i++) {
                if (!string.IsNullOrEmpty(parts[i])) {
                    appIds.Add(parts[i]);
                    ListViewItem item = new ListViewItem(parts[i]);
                    item.SubItems.Add("Pending");
                    appList.Items.Add(item);
                }
            }

            if (appIds.Count > 0) {
                this.Shown += (s, e) => worker.RunWorkerAsync();
            } else {
                statusLabel.Text = "No apps found in filename.";
                btnClose.Visible = true;
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e) {
            for (int i = 0; i < appIds.Count; i++) {
                string id = appIds[i];
                worker.ReportProgress(i, "Installing");

                bool success = false;
                if (id.StartsWith("Extension.")) {
                    success = InstallChromeExtension(id, i);
                } else if (id.StartsWith("Custom.")) {
                    success = InstallCustomApp(id, i);
                } else {
                    success = InstallWingetApp(id);
                }

                worker.ReportProgress(i, success ? "Success" : "Failed");
            }
        }

        private bool InstallWingetApp(string id) {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "winget";
            psi.Arguments = "install --id " + id + " --silent --accept-package-agreements --accept-source-agreements";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            try {
                Process p = Process.Start(psi);
                if (p != null) {
                    p.WaitForExit();
                    return p.ExitCode == 0;
                }
            } catch {}
            return false;
        }

        private bool InstallChromeExtension(string id, int index) {
            string extensionId = "";
            switch (id) {
                case "Extension.CryptoPro": extensionId = "iifghbkadhacfocbbcdhbaicpngaknkd"; break;
                case "Extension.CryptoProNew": extensionId = "edhifhklioihhjhpfgejndnclbeaklee"; break;
                case "Extension.Gosuslugi": extensionId = "pbcmomffndpancndfocafiafoidocpde"; break;
                case "Extension.Gosplugin": extensionId = "lpgmffmjjdbonclononmdoeakieideid"; break;
            }

            if (string.IsNullOrEmpty(extensionId)) return false;

            try {
                // Install for Chrome
                string keyPath = @"SOFTWARE\Google\Chrome\Extensions\" + extensionId;
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(keyPath)) {
                    key.SetValue("update_url", "https://clients2.google.com/service/update2/crx");
                }
                
                // Install for Edge
                string edgeKeyPath = @"SOFTWARE\Microsoft\Edge\Extensions\" + extensionId;
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(edgeKeyPath)) {
                    key.SetValue("update_url", "https://clients2.google.com/service/update2/crx");
                }

                if (id == "Extension.CryptoPro" || id == "Extension.CryptoProNew") {
                    ConfigureCryptoPro();
                }

                return true;
            } catch {
                return false;
            }
        }

        private void ConfigureCryptoPro() {
            try {
                // Add trusted domains to CryptoPro
                // This is a simplified version, usually involves setting registry keys for the plugin
                string pluginKey = @"SOFTWARE\Crypto Pro\CAdES Browser Plug-in";
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(pluginKey)) {
                    // Example trusted sites from Kontur
                    key.SetValue("TrustedSites", "https://*.kontur.ru;https://*.gosuslugi.ru;https://*.cryptopro.ru", RegistryValueKind.String);
                }
            } catch {}
        }

        private bool InstallCustomApp(string id, int index) {
            string fileName = "";
            string downloadUrl = "";
            bool isMsi = true;

            switch (id) {
                case "Custom.TeamDesk": 
                    fileName = "TeamDesk.msi"; 
                    downloadUrl = "https://temofeika.github.io/SoftPortal/msi/TeamDesk.msi";
                    break;
                case "Custom.OpenVPN": 
                    fileName = "OpenVPN-2.5.3-I601-amd64.msi"; 
                    downloadUrl = "https://temofeika.github.io/SoftPortal/msi/OpenVPN-2.5.3-I601-amd64.msi";
                    break;
                case "Custom.Rutoken": 
                    bool is64 = IntPtr.Size == 8;
                    fileName = is64 ? "rtDrivers.x64.msi" : "rtDrivers.x86.msi"; 
                    downloadUrl = "https://download.rutoken.ru/Rutoken/Drivers/Current/" + fileName;
                    break;
                case "Custom.CryptoPro": 
                    fileName = "CSPSetup.exe"; 
                    downloadUrl = "https://temofeika.github.io/SoftPortal/msi/CSPSetup.exe";
                    isMsi = false;
                    break;
                default: 
                    fileName = id.Replace("Custom.", "") + ".msi"; 
                    downloadUrl = "https://temofeika.github.io/SoftPortal/msi/" + fileName;
                    break;
            }

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (!File.Exists(filePath)) {
                worker.ReportProgress(index, "Downloading"); 
                try {
                    using (WebClient wc = new WebClient()) {
                        wc.DownloadFile(downloadUrl, filePath);
                    }
                } catch {
                    return false;
                }
            }

            ProcessStartInfo psi = new ProcessStartInfo();
            if (isMsi) {
                psi.FileName = "msiexec.exe";
                psi.Arguments = "/i \"" + filePath + "\" /qn /norestart";
            } else {
                psi.FileName = filePath;
                psi.Arguments = "/silent /norestart"; // Common flags for CryptoPro
            }
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            try {
                Process p = Process.Start(psi);
                if (p != null) {
                    p.WaitForExit();
                    return p.ExitCode == 0 || p.ExitCode == 3010;
                }
            } catch {}
            return false;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            int index = e.ProgressPercentage;
            string status = (string)e.UserState;
            appList.Items[index].SubItems[1].Text = status;
            
            if (status == "Installing") {
                appList.Items[index].ForeColor = Color.FromArgb(0, 112, 243);
                statusLabel.Text = "Installing " + appIds[index] + "...";
            } else if (status == "Success") {
                appList.Items[index].ForeColor = Color.LightGreen;
            } else {
                appList.Items[index].ForeColor = Color.Salmon;
            }

            int percent = (int)(((float)(index + 1) / appIds.Count) * 100);
            progressBar.Value = percent;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            statusLabel.Text = "All tasks completed!";
            btnClose.Visible = true;
        }

        [STAThread]
        static void Main() {
            try {
                // Enable TLS 1.2 for downloads
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            } catch {}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstallerForm());
        }
    }
}
