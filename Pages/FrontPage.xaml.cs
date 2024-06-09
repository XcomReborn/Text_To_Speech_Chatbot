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
using System.Windows.Controls.Primitives;
using System.Configuration;
using KickLib;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for FrontPage.xaml
    /// </summary>
    public partial class FrontPage : Page
    {
        private SettingsManager settingsManager;
        public FrontPage(SettingsManager settingsManager)
        {

            this.settingsManager = settingsManager;

            InitializeComponent();

            if ( !((TextToSpeech)Application.Current.Properties["tts"]).twitch_bot.client.IsConnected)
            {
                connectButtonTwitch.Content = "Connect";
            }
            else
            {
                connectButtonTwitch.Content = "Disconnect";
            }



        }

        private bool dragStarted = false;

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SetVolume(((Slider)sender).Value);
            this.dragStarted = false;
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
                SetVolume(((Slider)sender).Value);
        }

        private void SetVolume(double value)
        {
            string volumeString = value.ToString();
            Debug.WriteLine(String.Format("Slider value : {0}", volumeString));
            try
            {
                settingsManager.settings.volume = Convert.ToInt32(value);
                settingsManager.Save();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }


        private void OnClickConnectButton(object sender, RoutedEventArgs e)
        {
            if (  !((TextToSpeech)Application.Current.Properties["tts"]).twitch_bot.client.IsConnected ){
                // reload commands
                ((TextToSpeech)Application.Current.Properties["tts"]).commands.Load();


                bool success = ((TextToSpeech)Application.Current.Properties["tts"]).twitch_bot.Connect();
                ((TextToSpeech)Application.Current.Properties["tts"]).kick_bot.Connect();

                if (success)
                {
                    ((TextToSpeech)Application.Current.Properties["tts"]).run();

                    connectButtonTwitch.Content = "Disconnect";
                }
            }
            else {

                //Send Disconnect Message before disconnecting:
                try
                {

                    if (settingsManager.settings.settingDictionary["displayDisconnectionMessage"])
                    {
                        ((TextToSpeech)Application.Current.Properties["tts"]).twitch_bot.client.SendMessage(settingsManager.settings.defaultJoinChannel, "Disconnecting TTS TwitchBot.");
                    }

                ((TextToSpeech)Application.Current.Properties["tts"]).twitch_bot.client.Disconnect();
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
                connectButtonTwitch.Content = "Connect";

            }


        }

        private async void OnClickConnectButtonKick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Button Pressed.");
            Debug.WriteLine(settingsManager.settings.kickChannelUserName);
            Debug.WriteLine(settingsManager.settings.KickPassword);
            Debug.WriteLine(settingsManager.settings.Kick2FAToken);


            await ((TextToSpeech)Application.Current.Properties["tts"]).kick_bot.kickApi.Messages.SendMessageAsync(12267684, "This is a test message.");

        }

        private void volumeSliderLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set volume slider initial value exception will be thrown here
                
                volumeSlider.Value = Convert.ToDouble(settingsManager.settings.volume);
            }catch (Exception ex) { Debug.WriteLine(ex.ToString()); };
        }
    }
}


