using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Suite;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Earthquake
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class EarthquakeData
        {
            public List<EarthquakeFeature> Features { get; set; }
           
        }
        public class EarthquakeFeature
        {
            public EarthquakeProperties Properties { get; set; }
            public Geometry Geometry { get; set; }
        }

        public class Geometry
        {
            public string Type { get; set; }
            public List<double> Coordinates { get; set; }
        }
        

        public class EarthquakeProperties
        {
            public double MAG { get; set; }
            public string TITLE { get; set; }
            public long TIME { get; set; }
           public string alert { get; set; }
            public string tsunami { get; set; }
           


            //public DateTime time
            //{
            //    get
            //    {
            //        return DateTimeOffset.FromUnixTimeMilliseconds(time).UtcDateTime;
            //    }
            //}
            public DateTime DateTimeSriLanka
            {
                get
                {
                    TimeZoneInfo sriLankaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");
                    return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1).AddMilliseconds(TIME), sriLankaTimeZone);
                }
            }
            // Add more properties as needed
        }
        private async Task RetrieveEarthquakeData(string minimumMagnitude, string startTime)
        {
            string url = $"https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime={startTime}&minmagnitude={minimumMagnitude}";

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadProgressChanged += (sender, e) =>
                    {
                        // Update the progress bar value based on the download progress
                        pgrDataload.Value = e.ProgressPercentage;
                    };

                    client.DownloadStringCompleted += async (sender, e) =>
                    {
                        if (e.Error == null)
                        {
                            string json = e.Result;

                            EarthquakeData data = JsonConvert.DeserializeObject<EarthquakeData>(json);

                            dgvData.Columns.Clear();
                            dgvData.DataBindings.Clear();

                            dgvData.DataSource = data.Features.Select(f => f.Properties).ToList();
                        }
                        else
                        {
                            MessageBox.Show("An error occurred: " + e.Error.Message);
                        }

                        // Reset the progress bar value to 0
                        pgrDataload.Value = 0;
                    };

                    await client.DownloadStringTaskAsync(url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        

        private async void Form1_Load(object sender, EventArgs e)
        {
            //lblDateTime.Text = DateTime.Now.ToString();
            timerDateTime.Start();
            
            
            guna2ComboBox1.SelectedIndex = 1;
           
            
                       
               
                string minimumMagnitude = guna2ComboBox1.SelectedItem.ToString();
                await RetrieveEarthquakeData(minimumMagnitude, DateTime.Now.ToShortDateString());              
          

            timer1.Start();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            string minimumMagnitude = guna2ComboBox1.SelectedItem.ToString();
          
            await RetrieveEarthquakeData(minimumMagnitude, DateTime.Now.ToShortDateString());
        }

        private void timerDateTime_Tick(object sender, EventArgs e)
        {
           
            DateTime localTime = DateTime.Now;
            string formattedDateTime = localTime.ToString("yyyy-MM-dd HH:mm:ss");
            lblDateTime.Text = formattedDateTime;
        }

       

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
