using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace RedAlert
{
    public class OrefListener
    {
        private System.Timers.Timer timer;
        private bool IsDebugMode = bool.Parse(ConfigurationManager.AppSettings["IsDebugMode"]);
        public event EventHandler<AlertEventArgs> OnAlert;
        private string OrefApiUrl = "https://www.oref.org.il/WarningMessages/alert/alerts.json";
     
        /// <summary>
        /// Oref Listenr is running Http request to Oref.org.il in order to recive Red Alert Messages
        /// </summary>
        public OrefListener()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //init time Instance
            timer = new System.Timers.Timer();
            //Set Timer Tick Interval (In miliseconds)
           
            //Register to Tick Event
            timer.Elapsed += timer_Tick;
            timer.Interval = 1000;
            timer.Enabled = true;
           
            if(IsDebugMode)
            {
                this.OrefApiUrl = "https://techblog.co.il/alerts.json";
                
            }
         
        }

        public bool Start()
        {
            try
            {
             
                timer.Start();
                return true;
            }
            catch(Exception ex)
            {
                Logger(ex);
                return false;
            }
        }
        public bool Stop()
        {
            try
            {
                timer.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Logger(ex);
                return false;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            using (var _webClient = new WebClient())
            {
                try
                {

                   
                    //Setting The Http Headers needed to make successful request
                    _webClient.Headers.Add("Referer", "https://www.oref.org.il/");

                    //Using User-Agent Header as Chrome Web Browser ontop of Windows 10
                    _webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36");
                    
                    _webClient.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    
                    _webClient.Encoding = Encoding.UTF8;
                   
                    var jsonResult = _webClient.DownloadString(this.OrefApiUrl);
                    //If there is no alert, the json response is Empty

                    if (!String.IsNullOrEmpty(jsonResult))
                    {
                        //Deserializing Json Result into Alert Object
                        Alert alert = JsonConvert.DeserializeObject<Alert>(jsonResult);
                        
                        //if someone registerd to OnAlert Event, rise it
                        if(this.OnAlert!=null)
                        {
                            AlertEventArgs args = new AlertEventArgs(alert, DateTime.Now);
                            this.OnAlert(this, args);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger(ex);
                    //In case there is a response error or Exception sleep for 3 seconds and than retry.
                    Thread.Sleep(3000);
                    
                }
            }
        }


        private  void Logger(Exception ex)
        {

            StreamWriter sw = new StreamWriter(Path.Combine(Application.StartupPath,"Redalert.log"), true, Encoding.UTF8);
            sw.WriteLine(ex.ToString());
            sw.Flush();
            sw.Dispose();
        }


        private void Logger(string info)
        {

            StreamWriter sw = new StreamWriter(Path.Combine(Application.StartupPath, "Redalert.log"), true, Encoding.UTF8);
            sw.WriteLine(info);
            sw.Flush();
            sw.Dispose();
        }


    }
}
