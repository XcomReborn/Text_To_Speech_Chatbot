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
                ((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.settings.volume = Convert.ToInt32(value);
                ((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.Save();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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

        private void volumeSliderLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set volume slider initial value
                TwitchTTSBotSettingsManager twitchTTSBotSettingManager = ((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager;
                volumeSlider.Value = Convert.ToDouble(twitchTTSBotSettingManager.settings.volume);
            }catch (Exception ex) { Debug.WriteLine(ex.ToString()); };
        }
    }
}


