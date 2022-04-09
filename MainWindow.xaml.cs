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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public MainWindow()
        {
           Debug.WriteLine("program started");

        Bot bot = new Bot();
        TextToSpeech tts = new TextToSpeech(bot);
        TTSBotCommands commands = new TTSBotCommands(tts);

        Application.Current.Properties["bot"] = bot;
        Application.Current.Properties["tts"] = tts;
        Application.Current.Properties["commands"] = commands;


            InitializeComponent();
            MainFrame.Content = new FrontPage();
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
            MainFrame.Content = new FrontPage();
        }

        private void MouseDownOnSettingsText(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Content = new SettingsPage();
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



}
