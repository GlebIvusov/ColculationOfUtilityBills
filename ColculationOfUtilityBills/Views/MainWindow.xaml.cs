using ColculationOfUtilityBills.ViewModels;
using ColculationOfUtilityBills.Views;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColculationOfUtilityBills
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel mainViewModel = new MainViewModel();
            DataContext = mainViewModel;
            ServiceListWindow serviceListWindow = new ServiceListWindow();
            HistoryCostWindow historyCostWindow = new HistoryCostWindow();
            PersonPeriodWindow personPeriodWindow = new PersonPeriodWindow();
            mainViewModel.RequestHideMainWindow = this.Hide;
            mainViewModel.RequestHideServiceListWindow = serviceListWindow.Hide;
            mainViewModel.RequestShowMainWindow = this.Show;
            mainViewModel.RequestShowServiceListWindow = () => { serviceListWindow.Show(); serviceListWindow.DataContext = mainViewModel; };
            mainViewModel.RequestShowHistoryCostWindow = () => { historyCostWindow.Show(); historyCostWindow.DataContext = mainViewModel; };
            mainViewModel.RequestHideHistoryCostWindow = historyCostWindow.Hide;
            mainViewModel.RequestShowPersonPeriodWindow = () => { personPeriodWindow.Show(); personPeriodWindow.DataContext = mainViewModel; };
            mainViewModel.RequestHidePersonPeriodWindow = personPeriodWindow.Hide;
        }
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return Regex.IsMatch(text, @"^[0-9]*(?:[.,][0-9]*)?$");
        }
    }
}