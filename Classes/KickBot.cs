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



namespace TTSBot
{

    public class KickBot{


        public IKickApi kickApi = new KickApi();
        private SettingsManager? botSettingManager;
        public string userName = "xereborn";
        public Queue<ChatMessageEventArgs> messageBuffer = new Queue<ChatMessageEventArgs>();
        public TextToSpeech text_to_speech;

        public KickBot(TextToSpeech text_to_speech)
        {
            this.text_to_speech = text_to_speech;
            this.botSettingManager = Application.Current.Properties["settings_manager"] as SettingsManager;
        }

        public async Task Connect()
        {
            var channelInfo = await this.kickApi.Channels.GetChannelInfoAsync(this.botSettingManager.settings.kickChannelUserName);

            IKickClient client = new KickClient();

            client.OnMessage += Client_OnMessageReceived;

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

            Console.WriteLine(e.Data.Content);
            ChatData chat_data = new ChatData(e);
            text_to_speech.messageBuffer.Enqueue(chat_data);

        }


    }

}