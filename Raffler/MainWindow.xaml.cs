using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using MojangSharp.Api;
using MojangSharp;
using MojangSharp.Endpoints;
using MojangSharp.Responses;

namespace Raffler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List<KeyValuePair<string, int>> playerKVP = new List<KeyValuePair<string, int>>();
        List<Player> players = new List<Player>();
        string fileName;
        string file = "results.txt";
        StringBuilder sb = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            //Open dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "json files (*.json)|*.json"; //dont want to read any other format
            openFileDialog.ShowDialog();

            fileName = openFileDialog.FileName;
            txtFle.Text = fileName;

        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true; // start animation
            //if it exists, delete the file for a fresh copy
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            File.Create(file).Close();
            
            taOutput.Clear();
            try
            {
                if (taOutput.Text != null)
                {
                    taOutput.Clear();
                }
                var playerKVP = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(txtFle.Text)).ToList();
                //convert KVP to List
                foreach (var player in playerKVP)
                {
                    players.Add(new Player { uuid = player.Key, voteCount = player.Value });
                    taOutput.Text += player.Key + " | " + player.Value + "\n";
                }

                //loop through list until its empty, then output the string to file
                while (players.Count > 0)
                {
                    for(int i = 0; i < players.Count; i++)
                    {
                        if (players[i].voteCount > 0)
                        {
                            sb.Append(players[i].uuid + "\n");
                            --players[i].voteCount;
                        }
                        else
                        {
                            players.Remove(players[i]);
                        }
                    }
                }
                File.WriteAllText(file, sb.ToString());
            }
            catch (JsonException)
            {
                taOutput.Text = "ERROR: Could not parse JSON file. Is the JSON in the write format?";
            }
            catch (ArgumentException)
            {
                taOutput.Text = "ERROR: No file selected! Please enter a file.";
            }
            catch (Exception)
            {
                taOutput.Text = "ERROR: Unknown.";
            }
            progressBar.IsIndeterminate = false;
            progressBar.Value = 100; // start animation
        }

        private async void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            string result;
            try
            {
                var lines = File.ReadLines(file).Count();
                Random rnd = new Random();
                var num = rnd.Next(1, lines);
                using (var sr = new StreamReader(file))
                {
                    for (int i = 1; i <= num; i++)
                    {
                        sr.ReadLine();
                    }
                     result = sr.ReadLine();
                }
                

                // remove hyphens for API call
                string formattedId = result.Replace("-", "");
                
                //Send Mojang the API call
                NameHistoryResponse response = await new NameHistory(formattedId).PerformRequest();

                txtWinner.Text = response.NameHistory[response.NameHistory.Count - 1].Name;
             
            }
            catch(FileNotFoundException)
            {
                taOutput.Text += "ERROR: Could not locate file";
            }
        }
    }
}
