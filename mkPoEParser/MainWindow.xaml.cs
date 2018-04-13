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
using System.Windows.Threading;

namespace mkPoEParser
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JsonParser parser;
        public string league;
        public string searchItemName;
        public string searchAccountName;

        public MainWindow()
        {
            InitializeComponent();
        }        

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void BaseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            searchItemName = searchTermTextBox.Text;
            searchAccountName = searchAccountNameTextBox.Text;
            league = leagueTextBox.Text;

            if (parser != null)
            {
                parser.Stop();
            }

            parser = new JsonParser(this, league, searchItemName, searchAccountName);
            parser.Start();
        }
        
        public void AddToLog(string text)
        {
            text = DateTime.Now.ToString("[hh:mm:ss] ") + text + "\n";
            
            logTextBox.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { logTextBox.AppendText(text); logTextBox.ScrollToEnd(); }));       
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (parser != null)
            {
                parser.Stop();
            }

            Environment.Exit(0);
        }
    }
}
