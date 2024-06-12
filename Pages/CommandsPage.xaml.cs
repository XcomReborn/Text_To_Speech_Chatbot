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


            CommandsManager commands = (CommandsManager)Application.Current.Properties["commands"];
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

                RowDefinition privilegeRow = new RowDefinition();
                privilegeColumn.RowDefinitions.Add(privilegeRow);

                RowDefinition enabledRow = new RowDefinition();
                enabledColumn.RowDefinitions.Add(enabledRow);

                // Commands
                MyTextBox myTextBox = new MyTextBox();
                myTextBox.Text = item.Value.ttsComparisonCommand;
                myTextBox.name = item.Value.name;
                myTextBox.Foreground = Brushes.Red;
                myTextBox.TextChanged += new TextChangedEventHandler(CommandTextBoxChanged);

                Grid commandGrid = new Grid();
                Grid.SetRow(commandGrid, i);
                commandColumn.Children.Add(commandGrid);
                commandGrid.Children.Add(myTextBox);

                // Usage
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

                // Descriptions
                Label descriptionLabel = new Label();
                descriptionLabel.Content = item.Value.description;
                descriptionLabel.Foreground = Brushes.Blue;
                descriptionLabel.HorizontalAlignment = HorizontalAlignment.Center;

                Grid descriptionGrid = new Grid();
                Grid.SetRow(descriptionGrid, i);
                descriptionColumn.Children.Add(descriptionGrid);
                descriptionGrid.Children.Add(descriptionLabel);


                // Privileges
                MyComboBox comboBox = new MyComboBox();

                TextBlock myTextBlock5 = new TextBlock();
                myTextBlock5.Foreground = Brushes.Blue;
                myTextBlock5.Text = "Admin Only";
                comboBox.Items.Add(myTextBlock5);
                TextBlock myTextBlock4 = new TextBlock();
                myTextBlock4.Foreground = Brushes.Blue;
                myTextBlock4.Text = "Streamer + Admin";
                comboBox.Items.Add(myTextBlock4);
                TextBlock myTextBlock3 = new TextBlock();
                myTextBlock3.Foreground = Brushes.Blue;
                myTextBlock3.Text = "Mod + Streamer + Admin";
                comboBox.Items.Add(myTextBlock3);
                TextBlock myTextBlock2 = new TextBlock();
                myTextBlock2.Foreground = Brushes.Blue;
                myTextBlock2.Text = "VIP + Mod + Streamer + Admin";
                comboBox.Items.Add(myTextBlock2);
                TextBlock myTextBlock = new TextBlock();
                myTextBlock.Foreground = Brushes.Blue;
                myTextBlock.Text = "Everyone";
                comboBox.Items.Add(myTextBlock);

                Grid privilegeGrid = new Grid();
                Grid.SetRow(privilegeGrid, i);
                privilegeColumn.Children.Add(privilegeGrid);
                privilegeGrid.Children.Add(comboBox);

                comboBox.name = item.Value.name;
                comboBox.SelectedIndex = 4 - (int)item.Value.privilageLevel;
                comboBox.SelectionChanged += new SelectionChangedEventHandler(PrivilegeLevelChanged);

                // CheckBoxes
                MyCheckBox myCheckBox = new MyCheckBox();
                myCheckBox.IsChecked = item.Value.enabled;
                myCheckBox.name = item.Value.name;
                myCheckBox.Checked += new RoutedEventHandler(CheckBoxToggled);
                myCheckBox.Unchecked += new RoutedEventHandler(CheckBoxToggled);
                myCheckBox.HorizontalAlignment = HorizontalAlignment.Center;    

                Grid enabledGrid = new Grid();
                Grid.SetRow(enabledGrid, i);
                enabledColumn.Children.Add(enabledGrid);    
                enabledGrid.Children.Add(myCheckBox);
                i++;



            }
            
        }

        private void PrivilegeLevelChanged(object sender, SelectionChangedEventArgs e)
        {
            try {
                CommandsManager commands = (CommandsManager)Application.Current.Properties["commands"];
                foreach (var item in commands.commands)
                {
                    if (((MyComboBox)sender).name == item.Value.name)
                    {
                        item.Value.privilageLevel = 4 - (Commands.UserLevel)((MyComboBox)sender).SelectedIndex;
                        commands.Save();
                        ((TextToSpeech)Application.Current.Properties["tts"]).commands.Load();

                    }
                }

            } catch
            {

            }

        }

        private void CheckBoxToggled(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Checkbox Toggled");
                CommandsManager commands = (CommandsManager)Application.Current.Properties["commands"];
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
                CommandsManager commands = (CommandsManager)Application.Current.Properties["commands"];
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
