
using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace RedAlert
{
    public partial class Monitor : ServiceBase
    {
        private OrefListener _listener;
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
        private bool IsTelegramEnabled = bool.Parse(ConfigurationManager.AppSettings["IsTelegramEnabled"]);
        private bool IsMqttEnabled = bool.Parse(ConfigurationManager.AppSettings["IsMqttEnabled"]);
        MqttPublisher _mqttPublisher;

        public Monitor(string[] args)
        {
            InitializeComponent();
            _listener = new OrefListener();
            _listener.OnAlert += _listener_OnAlert;
            _mqttPublisher = new MqttPublisher();
            _mqttPublisher.OnError += _mqttPublisher_OnError;

        }

      



        /// <summary>
        /// Start Redalert Service
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
          
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
           
            //Start the listener
            _listener.Start();

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }


        /// <summary>
        /// Stop Redalert Service
        /// </summary>
        protected override void OnStop()
        {

            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //Stop the listener
            _listener.Stop();


            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        //When there is alert, event is rising and you will catch it here
        void _listener_OnAlert(object sender, AlertEventArgs e)
        {
         
            PublishMqttTopic(e);

            if (IsTelegramEnabled)
                SendTelegramAlert(e);

        }

        private void PublishMqttTopic(AlertEventArgs e)
        {
            
            _mqttPublisher.Publish(e.Alert);
        }

        private void SendTelegramAlert(AlertEventArgs e)
        {
            TelegramBot bot = new TelegramBot();
            StringBuilder Areas = new StringBuilder();
            foreach (var alert in e.Alert.data)
            {
                Areas.Append(alert + "\n");
            }
            bot.Message = String.Format("{0}\n{1}\n{2}", e.Alert.title, e.AlertDate.ToString("dd/MM/yyyy HH:MM"), Areas);
            bot.SendMessage();

            bot.Dispose();
        }

        
        void _mqttPublisher_OnError(object sender, ExceptionEventArgs e)
        {
            Logger(e.exeption);
        }

        private void Logger(Exception exception)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(Application.StartupPath, "Redalert.log"), true, Encoding.UTF8);
            sw.WriteLine(exception.ToString());
            sw.Flush();
            sw.Dispose();
        }

        private void Logger(string exception)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(Application.StartupPath, "Redalert.log"), true, Encoding.UTF8);
            sw.WriteLine(exception);
            sw.Flush();
            sw.Dispose();
        }
    }







}
