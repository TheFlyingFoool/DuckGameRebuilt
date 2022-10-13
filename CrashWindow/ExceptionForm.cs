using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Net;
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
            InitializeComponent();
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
    }
}