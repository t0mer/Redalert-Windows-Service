using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RedAlert
{
    public class TelegramBot : IDisposable
    {

        #region Public Properties

        /// <summary>
        /// Message - String Message to send to channel
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Private Properties
        /// <summary>
        /// Apit token for accessing Telegram Api
        /// </summary>
        private string apiToken = ConfigurationManager.AppSettings["telegramApi"];
       
        //ChatId - The channel to send the message to
        private string chatId = ConfigurationManager.AppSettings["channelId"];
        
        //Telegram API Url
        private string telegramApiUrl = ConfigurationManager.AppSettings["telegramApiUrl"];
        
        private bool disposed = false; // to detect redundant calls
        
        private WebRequest request ;
        
        private Stream rs;

        #endregion
     
        public void SendMessage()
        {
            try
            {
                telegramApiUrl = String.Format(telegramApiUrl, this.apiToken, this.chatId, this.Message);
                request = WebRequest.Create(telegramApiUrl);
                rs = request.GetResponse().GetResponseStream();
                                
            }
            catch (Exception ex)
            {
                
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
                    telegramApiUrl = null;
                    apiToken = null;
                    chatId = null;
                    telegramApiUrl = null;
                    Message = null;
                    request = null;
                    rs.Dispose();
                }

                // shared cleanup logic
                disposed = true;
            }
        }

        #endregion
    }
}
