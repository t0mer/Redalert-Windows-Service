using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace RedAlert
{
    public class MqttPublisher : IDisposable
    {
        #region Private Properties

        // to detect redundant calls
        private bool disposed = false;

        //Mqtt Host
        private string MqttHost = ConfigurationManager.AppSettings["MqttHost"];

        //Mqtt Username
        private string MqttUser = ConfigurationManager.AppSettings["MqttUser"];

        //Mqtt PAssword
        private string MqttPass = ConfigurationManager.AppSettings["MqttPass"];

        private bool IsMqttEnabled = bool.Parse(ConfigurationManager.AppSettings["IsMqttEnabled"]);

        private string MqttClientId = Guid.NewGuid().ToString();

        private MqttClient mqttClient;
        #endregion

        #region Public Properties
        /// <summary>
        /// The alert to send over MQTT Protocol
        /// </summary>
        public Alert alert { get; set; }

        public bool IsConnected { get; private set; }

        public event EventHandler<ExceptionEventArgs> OnError;

        #endregion


        public MqttPublisher()
        {
            if (IsMqttEnabled)
            {
                this.mqttClient = new MqttClient(this.MqttHost);
                mqttClient.Connect(this.MqttClientId, this.MqttUser, this.MqttPass);
                this.IsConnected = mqttClient.IsConnected;
            }
        }

        #region disposing

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                }

                // shared cleanup logic
                disposed = true;
            }
        }

        #endregion

        internal void Publish(Alert alert)
        {
            try
            {
                if (this.mqttClient != null)
                {
                    if (this.IsConnected)
                    {
                        string Alert = JsonConvert.SerializeObject(alert);
                        mqttClient.Publish("/redalert/", Encoding.UTF8.GetBytes(Alert), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,false);
                    }
                }
            }
            catch(Exception ex)
            {
                if(this.OnError!=null)
                {
                    ExceptionEventArgs args = new ExceptionEventArgs();
                    args.exeption = ex;
                    args.MethodName = "MqttPublisher.Publish";
                    this.OnError(this,args);
                }
            }
        }
    }
}
