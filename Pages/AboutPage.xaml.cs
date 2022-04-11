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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo(e.Uri.AbsoluteUri);
                psi.UseShellExecute = true;
                System.Diagnostics.Process.Start(psi);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);    
            }
        }
    }
}
