using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json;

namespace Raffler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            List<Player> players = new List<Player>();
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            //Open dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "json files (*.json)|*.json"; //dont want to read any other format
            openFileDialog.ShowDialog();
            

            //TODO: Read JSON file in and deserialize it to a list of objects
            //TODO: Cycle through list printing a line with the UUID and decrementing the count associated and incrementing the number of rows
            //TODO: Once List contains no records, select a number between 1 and the number of rows
            //TODO: Display the record indexed at the rand result
            //TODO: Hookup functions to events
            //TODO: Select winner for October
            //TODO: Attempt file data validation

        }

    }
}
