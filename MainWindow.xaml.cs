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
        public CommandsPage commandsPage;
        public SettingsPage settingsPage;
        public AboutPage aboutPage;

        private LowLevelKeyboardListener _listener;

        private bool paused = false;

        private SettingsManager settings_manager;


        public MainWindow()
        {
            Console.WriteLine("Hello");

            this.settings_manager = new SettingsManager();
            Application.Current.Properties["settings_manager"] = settings_manager;

            TextToSpeech tts = new TextToSpeech();
            Application.Current.Properties["tts"] = tts;

            CommandsManager commands = new CommandsManager(tts);
            Application.Current.Properties["commands"] = commands;

            InitializeComponent();

            frontPage = new FrontPage(settings_manager);
            Application.Current.Properties["frontPage"] = frontPage;

            commandsPage = new CommandsPage();
            settingsPage = new SettingsPage(settings_manager);
            aboutPage = new AboutPage();

            MainFrame.Content = frontPage;

            tts.run();

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
            //SettingsManager? settings_manager = Application.Current.Properties["SettingsManager"] as SettingsManager;
            if (e.KeyPressed == (Key)this.settings_manager.settings.skipKey)
            {
                    frontPage.skipIndicator.Fill = new SolidColorBrush(Colors.LightGreen);
                    frontPage.pauseIndicator.Fill = new SolidColorBrush(Colors.LightGreen);
                    ResumeSpeech();

            }
            if (e.KeyPressed == (Key)settings_manager.settings.skipAllKey)
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

            if (e.KeyPressed == (Key)settings_manager.settings.pauseKey)
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
            if (e.KeyPressed == (Key)settings_manager.settings.skipKey)
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
            if (e.KeyPressed == (Key)settings_manager.settings.skipAllKey)
            {
                // add skip all messages logic
                //
                try
                {
                    // requires resume before disposal or will not speak on new object
                    ResumeSpeech();
                    ((TextToSpeech)Application.Current.Properties["tts"]).messageBuffer.Clear();
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

        private void MouseDownOnCommandsText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = commandsPage;
        }

        private void MouseDownOnTextToSpeechText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = frontPage;
        }

        private void MouseDownOnSettingsText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = settingsPage;

        }

        private void MouseDownOnAboutText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = aboutPage;
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            // cleanup thread
            try
            {

                if (settings_manager.settings.settingDictionary["displayDisconnectionMessage"])
                {
                    ((TextToSpeech)Application.Current.Properties["tts"]).twitch_bot.client.SendMessage(settings_manager.settings.defaultJoinChannel, "Closing TTS TwitchBot.");
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



        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Foreground = Brushes.Red;
        }

        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Foreground = Brushes.White;
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
