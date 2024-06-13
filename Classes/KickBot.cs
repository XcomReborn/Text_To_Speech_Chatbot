using KickLib.Interfaces;
using KickLib;
using System;
using KickLib.Client.Interfaces;
using KickLib.Client.Models.Args;
using KickLib.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using KickLib.Models;
using System.Net.Mail;
using WpfApp1.Pages;
using System.Diagnostics;
using KickLib.Models.Response.v1.Channels;



namespace TTSBot
{

    public class KickBot{


        public IKickApi kickApi = new KickApi();
        private SettingsManager? botSettingManager;
        public string userName = "xereborn";
        public Queue<ChatData> messageBuffer = new Queue<ChatData>();
        public TextToSpeech text_to_speech;
        public IKickClient client = new KickClient();
        private ChannelResponse? channelInfo;

        public KickBot(TextToSpeech text_to_speech)
        {
            this.text_to_speech = text_to_speech;
            this.botSettingManager = Application.Current.Properties["settings_manager"] as SettingsManager;
        }

        public async Task Connect()
        {
            channelInfo = await this.kickApi.Channels.GetChannelInfoAsync(this.botSettingManager.settings.kickChannelUserName);

            client.OnMessage += Client_OnMessageReceived;
            client.OnConnected += Client_OnConnected;

            var authSettings = new AuthenticationSettings(botSettingManager.settings.kickChannelUserName, botSettingManager.settings.KickPassword)
            {
                TwoFactorAuthCode = botSettingManager.settings.Kick2FAToken
            };

            await kickApi.AuthenticateAsync(authSettings);

            await client.ListenToChatRoomAsync(channelInfo.Chatroom.Id);

            await client.ConnectAsync();

        }

        public async void SendMessage(string channelid, string message) {

            try
            {
                var id = int.Parse(channelid);
                await kickApi.Messages.SendMessageAsync(id, message);

            }
            catch {
                return;
            }

        }


        private void Client_OnMessageReceived(object sender, ChatMessageEventArgs e)
        {

            ChatData chat_data = new ChatData(e, settingsManager:this.botSettingManager);

            Debug.WriteLine("Client_OnMessageReceived");

            //text_to_speech.ProcessChatMessage(chat_data);
            text_to_speech.messageBuffer.Enqueue(chat_data);

        }

        private void Client_OnConnected(object sender, EventArgs e) 
        {

            //((FrontPage)Application.Current.Properties["frontPage"]).connectButtonKick.Content = "Disconnect";
            Debug.WriteLine("Kick Connected");
            if (channelInfo != null) {
                SendMessage(channelInfo.Chatroom.Id.ToString(), "TTS Connected.");
                    }
            
        }



    }

}