using KickLib.Interfaces;
using KickLib;
using System;
using KickLib.Client.Interfaces;
using KickLib.Client.Models.Args;
using KickLib.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;



namespace TTSBot
{

    public class KickBot{


        private IKickApi kickApi = new KickApi();
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

            Console.WriteLine(channelInfo.Chatroom.Id);

            IKickClient client = new KickClient();

            client.OnMessage += Client_OnMessageReceived;

            await client.ListenToChatRoomAsync(channelInfo.Chatroom.Id);

            await client.ConnectAsync();



        }

        private void Client_OnMessageReceived(object sender, ChatMessageEventArgs e)
        {

            Console.WriteLine(e.Data.Content);
            ChatData chat_data = new ChatData(e);
            text_to_speech.messageBuffer.Enqueue(chat_data);

        }


    }

}