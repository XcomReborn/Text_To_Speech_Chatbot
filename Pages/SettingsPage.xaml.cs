using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private SettingsManager settingsManager;

        private List<MyCheckBox> checkboxList;

        public SettingsPage(SettingsManager settingsManager)
        {

            this.settingsManager = settingsManager;

            this.Loaded += LoadValues;

            InitializeComponent();

            TextToSpeech tts = (TextToSpeech)Application.Current.Properties["tts"];

            checkboxList = new List<MyCheckBox>();

            adminCheckbox.name = "adminSpeaks";
            checkboxList.Add(adminCheckbox);
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
            displayConnectionMessageCheckbox.name = "displayConnectionMessage";
            checkboxList.Add(displayConnectionMessageCheckbox);
            displayDisconnectionMessageCheckbox.name = "displayDisconnectionMessage";
            checkboxList.Add(displayDisconnectionMessageCheckbox);
            speakUserNameCheckbox.name = "speakUserNameEnabled";
            checkboxList.Add(speakUserNameCheckbox);



            // set checkbox to value saved in setttings
            foreach (var item in this.settingsManager.settings.settingDictionary)
            {
                foreach (var checkbox in checkboxList)
                {
                    // set state first
                    if (checkbox.name == item.Key)
                    {
                        checkbox.IsChecked = (bool)item.Value;
                    }

                }

            }

            foreach (var checkbox in checkboxList)
            {
                checkbox.Click += OnCheckBoxSettingsChanged;
            }

                // initialize all the hotkeys to the saved values.
                pauseKeyTextBox.Text = ((Key)this.settingsManager.settings.pauseKey).ToString();
            skipKeyTextBox.Text = ((Key)this.settingsManager.settings.skipKey).ToString();
            skipAllKeyTextBox.Text = ((Key)this.settingsManager.settings.skipAllKey).ToString();

        }


        private void LoadValues(object sender, RoutedEventArgs e)
        {
            // initalize checkboxes to current values
            botTwitchUserNameTextBox.Text = this.settingsManager.settings.botName;
            botTwitchOAuthKey.Password = this.settingsManager.settings.BotOAuthKey;
            channelNameTextBox.Text = this.settingsManager.settings.defaultJoinChannel;
            adminUserNameTextBox.Text = this.settingsManager.settings.twitchAdminUserName;
            //kickUserNameTextBox.Text = this.settingsManager.settings.kickChannelUserName;
            //kick_password.Password = this.settingsManager.settings.KickPassword;
            //kick2FATextBox.Password = this.settingsManager.settings.Kick2FAToken;
            //kickAdminUserNameTextBox.Text = this.settingsManager.settings.kickChannelAdminUserName;

            saidStringTextBox.Text = this.settingsManager.settings.saidString;

        }

        private void OnCheckBoxSettingsChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Check check");
                foreach (var item in settingsManager.settings.settingDictionary)
                {
                    foreach (var checkbox in checkboxList)
                    {
                        if (((MyCheckBox)sender).name == item.Key)
                        {
                            settingsManager.settings.settingDictionary[item.Key] = (bool)((MyCheckBox)sender).IsChecked;
                        }
                    }
                }
                settingsManager.Save();

            }
            catch
            {

            }

        }


        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            settingsManager.settings.botName = botTwitchUserNameTextBox.Text.Trim();
                if(!String.IsNullOrEmpty(botTwitchOAuthKey.Password))
            settingsManager.settings.BotOAuthKey = botTwitchOAuthKey.Password.Trim();
            settingsManager.settings.defaultJoinChannel = channelNameTextBox.Text.Trim();
            settingsManager.settings.twitchAdminUserName = adminUserNameTextBox.Text.Trim();
            //settingsManager.settings.kickChannelAdminUserName = kickAdminUserNameTextBox.Text.Trim();
            //if (!String.IsNullOrEmpty(kick_password.Password))
            //    settingsManager.settings.KickPassword = kick_password.Password.Trim();
            //if (!String.IsNullOrEmpty(kick2FATextBox.Password))
            //    settingsManager.settings.Kick2FAToken = kick2FATextBox.Password.Trim();
            //settingsManager.settings.kickChannelUserName = kickUserNameTextBox.Text.Trim();
            settingsManager.Save();

        }

        private void PauseKeySelectionKeyDown(object sender, KeyEventArgs e)
        {
            pauseKeyTextBox.Text = e.Key.ToString();
            settingsManager.settings.pauseKey = (int)e.Key;
            settingsManager.Save();
        }

        private void SkipKeySelectionKeyDown(object sender, KeyEventArgs e)
        {
            skipKeyTextBox.Text = e.Key.ToString();
            settingsManager.settings.skipKey = (int)e.Key;
            settingsManager.Save();
        }

        private void SkipAllKeySelectionKeyDown(object sender, KeyEventArgs e)
        {
            skipAllKeyTextBox.Text = e.Key.ToString();
            settingsManager.settings.skipAllKey = (int)e.Key;
            settingsManager.Save();
        }

        private void saidStringTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            settingsManager.settings.saidString = saidStringTextBox.Text.Trim();
            settingsManager.Save();
        }
    }
}
