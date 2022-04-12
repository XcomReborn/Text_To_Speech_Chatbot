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
using WpfApp1.Pages;
using TTSBot;
using System.Diagnostics;

using System; // EventArgs
using System.ComponentModel; // CancelEventArgs
using System.Windows; // window

using DesktopWPFAppLowLevelKeyboardHook;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public FrontPage frontPage;  

        private LowLevelKeyboardListener _listener;

        private bool paused = false;

        public MainWindow()
        {

        Bot bot = new Bot();
        TextToSpeech tts = new TextToSpeech(bot);
        TTSBotCommands commands = new TTSBotCommands(tts);

        Application.Current.Properties["bot"] = bot;
        Application.Current.Properties["tts"] = tts;
        Application.Current.Properties["commands"] = commands;

        InitializeComponent();

        frontPage = new FrontPage();

        MainFrame.Content = frontPage;

        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Window Loaded Called");

            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyDown += _listener_OnKeyDown;
            _listener.OnKeyUp += _listener_OnKeyUp;

            _listener.HookKeyboard();
        }

        void _listener_OnKeyUp(object sender, KeyPressedArgs e)
        {
            TwitchTTSBotSettingsManager botSettingManager = ((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager;
            if (e.KeyPressed == (Key)botSettingManager.settings.skipKey)
            {
                    frontPage.skipIndicator.Fill = new SolidColorBrush(Colors.LightGreen);
                    frontPage.pauseIndicator.Fill = new SolidColorBrush(Colors.LightGreen);
                    ResumeSpeech();

            }
            if (e.KeyPressed == (Key)botSettingManager.settings.skipAllKey)
            {
                    frontPage.skipAllIndicator.Fill = new SolidColorBrush(Colors.LightGreen);
                    frontPage.pauseIndicator.Fill = new SolidColorBrush(Colors.LightGreen);
                ResumeSpeech();


            }


        }

        private void ResumeSpeech()
        {
            try
            {
                paused = false;
                ((TextToSpeech)Application.Current.Properties["tts"]).synth.Resume();
            }
            catch (Exception ex)    
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        void _listener_OnKeyDown(object sender, KeyPressedArgs e)
        {
            TwitchTTSBotSettingsManager botSettingManager = ((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager;

            if (e.KeyPressed == (Key)botSettingManager.settings.pauseKey)
            {
                // toggle paused
                paused = !paused;
                if (paused)
                {
                    try
                    {
                                ((TextToSpeech)Application.Current.Properties["tts"]).synth.Pause();

                        frontPage.pauseIndicator.Fill = new SolidColorBrush(Colors.Red);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    try
                    {


                        ((TextToSpeech)Application.Current.Properties["tts"]).synth.Resume();
                        frontPage.pauseIndicator.Fill = new SolidColorBrush(Colors.LightGreen);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
            if (e.KeyPressed == (Key)botSettingManager.settings.skipKey)
            {
                try
                {
                    // requires resume before disposal or will not speak on new object
                    ResumeSpeech();
                    ((TextToSpeech)Application.Current.Properties["tts"]).synth.Dispose();
                    ((TextToSpeech)Application.Current.Properties["tts"]).synth = new System.Speech.Synthesis.SpeechSynthesizer();
                    frontPage.skipIndicator.Fill = new SolidColorBrush(Colors.Red);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    // Potentially thrown if there is nothing to dequeue.
                }


            }
            if (e.KeyPressed == (Key)botSettingManager.settings.skipAllKey)
            {
                // add skip all messages logic
                //
                try
                {
                    // requires resume before disposal or will not speak on new object
                    ResumeSpeech();
                    ((TextToSpeech)Application.Current.Properties["tts"]).bot.messageBuffer.Clear();
                    ((TextToSpeech)Application.Current.Properties["tts"]).synth.Dispose();
                    ((TextToSpeech)Application.Current.Properties["tts"]).synth = new System.Speech.Synthesis.SpeechSynthesizer();
                    frontPage.skipAllIndicator.Fill = new SolidColorBrush(Colors.Red);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    // Potentially thrown if there is nothing to dequeue.
                }

            }
        }



        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ClickingTextToSpeechText(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void SettingsText(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MouseDownOnCommandsText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = new CommandsPage();
        }

        private void MouseDownOnTextToSpeechText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = frontPage;
        }

        private void MouseDownOnSettingsText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = new SettingsPage();

        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            // cleanup thread
            try
            {
                if (((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.settings.settingDictionary["displayDisconnectionMessage"])
                {
                    ((TextToSpeech)Application.Current.Properties["tts"]).bot.client.SendMessage(((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.settings.defaultJoinChannel, "Closing TTS Bot.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            try
            {
                _listener.UnHookKeyboard();
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
            try
            {
                ((TextToSpeech)Application.Current.Properties["tts"]).synth.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void MouseDownOnAboutText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = new AboutPage();
        }
    }

    class MyCheckBox : CheckBox { 
        public string name;

        public MyCheckBox()
        {
            name = "";
        }

    }

    class MyTextBox : TextBox
    {

        public string name;

        public MyTextBox()
        {
            name = "";
        }

    }

    class MyLabel : Label
    {

        public string name;

        public MyLabel()
        {
            name = "";
        }


    }

    class MyComboBox : ComboBox
    {

        public string name;

        public MyComboBox()
        {

            name = "";
        }



    }   



}
