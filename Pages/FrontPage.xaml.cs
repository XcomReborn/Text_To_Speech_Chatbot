using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

using TTSBot;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for FrontPage.xaml
    /// </summary>
    public partial class FrontPage : Page
    {
        public FrontPage()
        {
            InitializeComponent();

            if ( !((TextToSpeech)Application.Current.Properties["tts"]).bot.client.IsConnected)
            {
                connectButton.Content = "Connect";
            }
            else
            {
                connectButton.Content = "Disconnect";
            }

        }


        private void OnClickConnectButton(object sender, RoutedEventArgs e)
        {
            if (  !((TextToSpeech)Application.Current.Properties["tts"]).bot.client.IsConnected ){
                // reload commands
                ((TextToSpeech)Application.Current.Properties["tts"]).commands.Load();


                bool success = ((TextToSpeech)Application.Current.Properties["tts"]).bot.Connect();
                if (success)
                {
                    ((TextToSpeech)Application.Current.Properties["tts"]).run();

                    connectButton.Content = "Disconnect";
                }
            }
            else {

                //Send Disconnect Message before disconnecting:
                try
                {
                    if (((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.settings.settingDictionary["displayDisconnectionMessage"])
                    {
                        ((TextToSpeech)Application.Current.Properties["tts"]).bot.client.SendMessage(((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.settings.defaultJoinChannel, "Disconnecting TTS Bot.");
                    }

                ((TextToSpeech)Application.Current.Properties["tts"]).bot.client.Disconnect();
                }
                catch (Exception ex) 
                {
                    Debug.WriteLine(ex.ToString());
                }
                try
                {
                    ((TextToSpeech)Application.Current.Properties["tts"]).cts.Cancel();
                }
                catch (Exception ex)
                { 
                    Debug.WriteLine(ex.ToString());
                }
                connectButton.Content = "Connect";

            }


        }

    }
}


