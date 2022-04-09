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
using System.Diagnostics;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class CommandsPage : Page

    {

        private List<MyLabel> usageLabelList = new List<MyLabel>();

        public CommandsPage()
        {
            InitializeComponent();

            


            TTSBotCommands commands = (TTSBotCommands)Application.Current.Properties["commands"];
            int i = 0;
            foreach (var item in commands.commands)
            {
                
                //Grid rowGrid = new Grid();
                RowDefinition commandRow = new RowDefinition();
                commandColumn.RowDefinitions.Add(commandRow);

                RowDefinition usageRow = new RowDefinition();
                usageColumn.RowDefinitions.Add(usageRow);

                RowDefinition descriptionRow = new RowDefinition();
                descriptionColumn.RowDefinitions.Add(descriptionRow);

                RowDefinition enabledRow = new RowDefinition();
                enabledColumn.RowDefinitions.Add(enabledRow);

                MyTextBox myTextBox = new MyTextBox();
                myTextBox.Text = item.Value.ttsComparisonCommand;
                myTextBox.name = item.Value.name;
                myTextBox.Foreground = Brushes.Red;
                myTextBox.TextChanged += new TextChangedEventHandler(CommandTextBoxChanged);

                Grid commandGrid = new Grid();
                Grid.SetRow(commandGrid, i);
                commandColumn.Children.Add(commandGrid);
                commandGrid.Children.Add(myTextBox);


                MyLabel usageLabel = new MyLabel();
                usageLabel.Content = String.Format(item.Value.usage, item.Value.ttsComparisonCommand);
                usageLabel.name = item.Value.name;
                usageLabel.Foreground = Brushes.Blue;
                usageLabel.HorizontalAlignment = HorizontalAlignment.Center;
                usageLabelList.Add(usageLabel);

                Grid usageGrid = new Grid();
                Grid.SetRow(usageGrid, i);
                usageColumn.Children.Add(usageGrid);
                usageGrid.Children.Add(usageLabel);


                Label descriptionLabel = new Label();
                descriptionLabel.Content = item.Value.description;
                descriptionLabel.Foreground = Brushes.Blue;
                descriptionLabel.HorizontalAlignment = HorizontalAlignment.Center;

                Grid descriptionGrid = new Grid();
                Grid.SetRow(descriptionGrid, i);
                descriptionColumn.Children.Add(descriptionGrid);
                descriptionGrid.Children.Add(descriptionLabel);

                MyCheckBox myCheckBox = new MyCheckBox();
                //myCheckBox.Content = "Enabled";
                myCheckBox.IsChecked = item.Value.enabled;
                myCheckBox.name = item.Value.name;
                myCheckBox.Checked += new RoutedEventHandler(CheckBoxToggled);
                myCheckBox.Unchecked += new RoutedEventHandler(CheckBoxToggled);
                myCheckBox.HorizontalAlignment = HorizontalAlignment.Right;    

                Grid enabledGrid = new Grid();
                Grid.SetRow(enabledGrid, i);
                enabledColumn.Children.Add(enabledGrid);    
                enabledGrid.Children.Add(myCheckBox);
                i++;



            }
            
        }


        private void CheckBoxToggled(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Checkbox Toggled");
                TTSBotCommands commands = (TTSBotCommands)Application.Current.Properties["commands"];
                foreach (var item in commands.commands)
                {
                    if (((MyCheckBox)sender).name == item.Value.name)
                    {
                        item.Value.enabled = (bool)((MyCheckBox)sender).IsChecked;
                        commands.Save();
                        ((TextToSpeech)Application.Current.Properties["tts"]).commands.Load();

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CommandTextBoxChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("TextBox Changed");
                TTSBotCommands commands = (TTSBotCommands)Application.Current.Properties["commands"];
                foreach (var item in commands.commands)
                {
                    if (((MyTextBox)sender).name == item.Value.name)
                    {
                        item.Value.ttsComparisonCommand = (string)((MyTextBox)sender).Text;
                        commands.Save();
                        ((TextToSpeech)Application.Current.Properties["tts"]).commands.Load();
                        // change usage
                        foreach(var label in usageLabelList)
                        {
                            if (label.name == item.Value.name)
                            {
                                label.Content = String.Format(item.Value.usage, item.Value.ttsComparisonCommand);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

    }
}
