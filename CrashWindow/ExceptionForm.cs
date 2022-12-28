using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace DuckGame.src.MonoTime.Console
{
    public partial class ExceptionForm : Form
    {
        public static string exceptionSource;
        public static bool modResponsible;
        public static bool modDisabled;
        public static bool modDisableDisabled;
        public static string commandLine;
        public static string executablePath;
        public static string modName;
        public static string pVersion;
        public static string pMods;
        public static string pAssembly;
        public static string pException;
        public static string pLogMessage;
        public static string pComment;

        public ExceptionForm()
        {
           // this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new Size(571, 424);
            InitializeComponent();

            // Program is linux
            //  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            // sets color as the default is wrong for linux etc
            Color DefaultBackground = Color.FromArgb(240, 240, 240);
            this.BackColor = DefaultBackground;
            this.richTextBox2.BackColor = DefaultBackground;
            this.checkBox1.BackColor = DefaultBackground;
            this.richTextBox1.BackColor = DefaultBackground;
            this.restartButton.BackColor = DefaultBackground;
            this.button2.BackColor = DefaultBackground;
            this.button1.BackColor = DefaultBackground;
            this.modDesc.BackColor = DefaultBackground;
            this.forumLink.BackColor = DefaultBackground;
            this.crashSourceLabel.BackColor = DefaultBackground;
            this.pictureBox1.BackColor = DefaultBackground;
            this.crashDescription.BackColor = DefaultBackground;


            Bitmap b = (Bitmap)pictureBox1.Image; // sets icon
            IntPtr pIcon = b.GetHicon();
            Icon i = Icon.FromHandle(pIcon);
            this.Icon = i;
            i.Dispose(); //

            if (modResponsible)
            {
                string descText;
                if (pAssembly != null)
                {
                    descText = "Mod \"" + (modName != null ? modName : pAssembly) + "\" crashed. ";

                    if (!modDisableDisabled && modDisabled)
                        descText += "Mod disabled. Try running game again.";
                }
                else
                {
                    descText = "Couldn't determine mod assembly. ";
                    if (!modDisableDisabled && modDisabled)
                        descText += "Mod was disabled. Try running game again.";
                }
                modDesc.Text = descText;
            }
            crashDescription.Text = pLogMessage;
            crashSourceLabel.Text = "Crash Source: " + exceptionSource;
            try
            {
                if (File.Exists(crashSettingsFile))
                {
                    string s = File.ReadAllText(crashSettingsFile);
                    if (s.Contains("false"))
                        checkBox1.Checked = false;
                    else
                        checkBox1.Checked = true;
                }
                else
                    checkBox1.Checked = true;
            }
            catch (Exception)
            {

            }
        }

        private void ExceptionForm_Load(object sender, EventArgs e)
        {
            forumLink.Links.Add(0, forumLink.Text.Length, "https://steamcommunity.com/app/312530/discussions/1/");

        }
        private void forumLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Submit();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fullString = crashSourceLabel.Text + "\r\n" + modDesc.Text + "\r\n\r\n" + pLogMessage;
            Clipboard.SetText(fullString);
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            Submit();
            Close();
            Process.Start(executablePath, commandLine);
            Application.Exit();
        }

        string crashComments;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            crashComments = richTextBox1.Text;
        }
        void Submit()
        {
            if (checkBox1.Checked)
            {
                try
                {
                    if(!crashDescription.Text.Contains("XNA Framework is not installed"))
                    {
                        if (pVersion == null && pMods == null && pAssembly == null && pException == null && pLogMessage == null && crashComments == null)
                        {
                            return;
                        }
                        CrashWindow.CrashWindow.SendBugReport(pVersion, pMods, pAssembly, pException, pLogMessage, crashComments != null ? crashComments : "");
                    }
                    if (crashComments != null)
                    {
                        MessageBox.Show("Your crash report has been uploaded, thanks for your help!", "Crash upload succeeded!");
                    }
                }
                catch (Exception sendError)
                {
                    MessageBox.Show(sendError.Message, "Crash upload failed! Please contact CORPTRON.");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(@"https://duckgame-beta.atlassian.net/rest/collectors/1.0/template/form/c0d27441?os_authType=none#");
        }

        private void crashDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private static string crashSettingsFile = "crash_settings.dat";
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (File.Exists(crashSettingsFile))
            {
                File.Delete(crashSettingsFile);
            }
            File.WriteAllText(crashSettingsFile, checkBox1.Checked ? "true" : "false");
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void crashDescription_TextChanged(object sender, EventArgs e)
        {

        }
    }
}