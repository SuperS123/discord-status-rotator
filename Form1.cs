using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Threading;
using System.Configuration;
using System.Collections.Specialized;

namespace Discord_Status_Rotater
{
    public partial class Form1 : Form
    {

        public DispatcherTimer timer = new DispatcherTimer();
        public Configuration config = null;

        public Form1()
        {
            InitializeComponent();
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
        public void changeStatus(string status, string token)
        {
            try
            {
                if (status == null) return;
                var url = "https://discord.com/api/v9/users/@me/settings";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "PATCH";

                httpRequest.Headers["Authorization"] = textBox1.Text;
                httpRequest.ContentType = "application/json";


                object test = new { text = status };
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(new { custom_status = test });

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                Console.WriteLine(httpResponse.StatusCode);
                label6.Text = "Current Status: " + status;
            }
            catch (WebException we)
            {
                AddOrUpdateAppSettings("Token", "");
                int statusCode = (int)((HttpWebResponse)we.Response).StatusCode;
                if(statusCode == 401)
                {
                    stop();
                    label6.Text += "(invalid token - program stopped)";
                }
                else if((int)statusCode == 429)
                {
                    stop();
                    label6.Text += "(rate limited - lower cooldown - program stopped)";
                } else
                {
                    stop();
                    label6.Text += "(unknown error occured - program stopped)";
                }
            }
            catch (Exception)
            {
                AddOrUpdateAppSettings("Token", "");
                stop();
                label6.Text += "(unknown error occured - program stopped)";
            }
        }

        private void stop()
        {
            label6.Text = "stopped ";
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            checkBox1.Enabled = true;
            timer.Stop();
            button1.BackColor = Color.Green;
            button1.Text = "Start!";
            MessageBox.Show("Auto changer stopped, it might do one more change as a pending task... please wait.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Stop")
            {
                stop();
                return;
            };
            // START EVENT
            
            // CHECKING IF ANYTHING IS BLANK
            if(textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("You must fill in the token (to obtain check the token.md file in the github) and the text seperated by a new line", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            char[] delims = new[] { '\n' };
            string[] strings = textBox2.Text.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length <= 1)
            {
                MessageBox.Show("You must have at least 2 statuses to cycle through", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(!checkBox1.Checked)
            {
                MessageBox.Show("You must agree by checking the checkbox", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            button1.Text = "Stop";
            button1.BackColor = Color.Red;
            AddOrUpdateAppSettings("Token", textBox1.Text);
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            checkBox1.Enabled = false;
            MessageBox.Show("Started automated status changing! To stop it, click stop!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // SET UP
            int i = 0;
            timer.Interval = TimeSpan.FromMilliseconds(int.Parse(textBox3.Text) * 1000); //x is the amount of milliseconds you want between each method call
            timer.Tick += (source, ae) =>
            {
                changeStatus(strings[i], textBox1.Text);
                i++;
                if (i == strings.Length) i = 0;
            };
            timer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            textBox1.Text = settings["Token"].Value;
        }
    }
}
