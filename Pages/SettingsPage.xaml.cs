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

using TTSBot;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page

    {
        private TwitchTTSBotSettingsManager botSettingsManager;

        private List<MyCheckBox> checkboxList;

        public SettingsPage()
        {
            InitializeComponent();

            TextToSpeech tts = (TextToSpeech)Application.Current.Properties["tts"];
            botSettingsManager = tts.bot.botSettingManager;

            // initalize checkboxes to current values
            botTwitchUserNameTextBox.Text = botSettingsManager.settings.botName;
            botTwitchOAuthKey.Password = botSettingsManager.settings.botOAuthKey;
            channelNameTextBox.Text = botSettingsManager.settings.defaultJoinChannel;
            adminUserNameTextBox.Text = botSettingsManager.settings.botAdminUserName;


            checkboxList = new List<MyCheckBox>();

            streamerCheckbox.name = "broadcasterSpeaks";
            checkboxList.Add(streamerCheckbox);
            moderatorCheckbox.name = "modSpeaks";
            checkboxList.Add(moderatorCheckbox);
            vipCheckbox.name = "vipSpeaks";
            checkboxList.Add(vipCheckbox);
            usersCheckbox.name = "userSpeaks";
            checkboxList.Add(usersCheckbox);
            subscribersCheckbox.name = "subscriberSpeaks";
            checkboxList.Add(subscribersCheckbox);


            substituteCheckbox.name = "substituteEnabled";
            checkboxList.Add(substituteCheckbox);
            substituteRegexCheckbox.name = "substituteRegexEnabled";
            checkboxList.Add(substituteRegexCheckbox);


            // set checkbox to value saved in setttings
            foreach (var item in botSettingsManager.settings.settingDictionary)
            {
                foreach (var checkbox in checkboxList)
                {
                    checkbox.Checked += new RoutedEventHandler(OnCheckBoxSettingsChanged);
                    checkbox.Unchecked += new RoutedEventHandler(OnCheckBoxSettingsChanged);
                    if(checkbox.name == item.Key)
                    {
                        checkbox.IsChecked = (bool)item.Value;
                    }
                }


            }

            // initialize all the hotkeys to the saved values.
            pauseKeyTextBox.Text = ((Key)botSettingsManager.settings.pauseKey).ToString();
            skipKeyTextBox.Text = ((Key)botSettingsManager.settings.skipKey).ToString();
            skipAllKeyTextBox.Text = ((Key)botSettingsManager.settings.skipAllKey).ToString();

        }


        private void OnCheckBoxSettingsChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in botSettingsManager.settings.settingDictionary)
                {
                    foreach (var checkbox in checkboxList)
                    {
                        if (((MyCheckBox)sender).name == item.Key)
                        {
                            botSettingsManager.settings.settingDictionary[item.Key] = (bool)((MyCheckBox)sender).IsChecked;
                        }
                    }
                }
                botSettingsManager.Save();
                //((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.Load();


            }
            catch
            {

            }

        }

        private void botUserNameLostFocus(object sender, RoutedEventArgs e)
        {
            botSettingsManager.settings.botName = botTwitchUserNameTextBox.Text;
            botSettingsManager.Save();
            //((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.Load();
        }

        private void BotOAuthKeyLostFocus(object sender, RoutedEventArgs e)
        {
            botSettingsManager.settings.botOAuthKey = botTwitchOAuthKey.Password;
            botSettingsManager.Save();
            //((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.Load();
        }

        private void ChannelNameLostFocus(object sender, RoutedEventArgs e)
        {
            botSettingsManager.settings.defaultJoinChannel = channelNameTextBox.Text;
            botSettingsManager.Save();
            //((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.Load();
        }

        private void AdminUserNameLostFocus(object sender, RoutedEventArgs e)
        {
            botSettingsManager.settings.botAdminUserName = adminUserNameTextBox.Text;
            botSettingsManager.Save();
            //((TextToSpeech)Application.Current.Properties["tts"]).bot.botSettingManager.Load();
        }

        private void PauseKeySelectionKeyDown(object sender, KeyEventArgs e)
        {
            pauseKeyTextBox.Text = e.Key.ToString();
            botSettingsManager.settings.pauseKey = (int)e.Key;
            botSettingsManager.Save();
        }

        private void SkipKeySelectionKeyDown(object sender, KeyEventArgs e)
        {
            skipKeyTextBox.Text = e.Key.ToString();
            botSettingsManager.settings.skipKey = (int)e.Key;
            botSettingsManager.Save();
        }

        private void SkipAllKeySelectionKeyDown(object sender, KeyEventArgs e)
        {
            skipAllKeyTextBox.Text = e.Key.ToString();
            botSettingsManager.settings.skipAllKey = (int)e.Key;
            botSettingsManager.Save();
        }
    }
}
