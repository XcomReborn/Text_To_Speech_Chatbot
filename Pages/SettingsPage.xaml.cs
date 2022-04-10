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
        public SettingsPage()
        {
            InitializeComponent();

            TextToSpeech tts = (TextToSpeech)Application.Current.Properties["tts"];
            TwitchTTSBotSettingsManager botSettingsManager = tts.bot.botSettingManager;


            botTwitchUserName.Text = botSettingsManager.settings.botName;
            botTwitchOAuthKey.Password = botSettingsManager.settings.botOAuthKey;
            channelName.Text = botSettingsManager.settings.defaultJoinChannel;
            adminUserName.Text = botSettingsManager.settings.botAdminUserName;

        }
    }
}
