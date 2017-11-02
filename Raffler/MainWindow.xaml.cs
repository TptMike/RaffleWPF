using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using MojangSharp.Endpoints;
using MojangSharp.Responses;

namespace Raffler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Player> players = new List<Player>();  //Container with player objects
        string fileName;                            //Input file passed from GUI file dialog button
        string file = "results.txt";                //Output file stored in current working directory (where this exe is)
        StringBuilder sb = new StringBuilder();     //we're parsing our json file with this

        public MainWindow()
        {
            //start the GUI window
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            //Open dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "json files (*.json)|*.json"; //dont want to read any other format
            openFileDialog.ShowDialog();

            //Set our file text field to show the path
            fileName = openFileDialog.FileName;
            txtFle.Text = fileName;
        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            //Super hacky way of showing loading process
            //The proper way would be to thread this parsing function but we're not doing that here
            progressBar.IsIndeterminate = true; // start animation
            //if it exists, delete the file for a fresh copy
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            File.Create(file).Close();
            
            //Clear the output field. its going to be filled with parsing output.
            taOutput.Clear();
            try
            {
                //Do it again if for whatever reason windows is weird and doesn't run clear the first time
                if (taOutput.Text != null)
                {
                    taOutput.Clear();
                }

                //Serialize the JSON file into a key value pair inside a Dictionary container
                var playerKVP = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(txtFle.Text)).ToList();
                //convert KVP Dictionary to a Player List
                foreach (var player in playerKVP)
                {
                    //holy magic balls - this is a way to create an anonymous object and add it to our list
                    players.Add(new Player { uuid = player.Key, voteCount = player.Value });
                    //print whats happening to the large text box
                    taOutput.Text += player.Key + " | " + player.Value + "\n";
                }

                //loop through list until its empty, then output the string to file
                while (players.Count > 0)
                {
                    //Append the uuid to our string, then decrement the vote counter
                    for(int i = 0; i < players.Count; i++)
                    {
                        if (players[i].voteCount > 0)
                        {
                            //keep appending the string and decrementing
                            sb.Append(players[i].uuid + "\n");
                            --players[i].voteCount;
                        }
                        else
                        {
                            //vote counter is zero, remove this player from the list
                            players.Remove(players[i]);
                        }
                    }
                }
                //Now that the list has been parsed, lets write our (long) string to the file
                File.WriteAllText(file, sb.ToString());
            }
            catch (JsonException)
            {
                //The file contents weren't in { "key":"value" } format, go check that
                taOutput.Text = "ERROR: Could not parse JSON file. Is the JSON in the write format?";
            }
            catch (ArgumentException)
            {
                //No file was selected with the OpenDialog
                taOutput.Text = "ERROR: No file selected! Please enter a file.";
            }
            catch (Exception)
            {
                //Who the hell knows? throw this so we don't crash
                taOutput.Text = "ERROR: Unknown.";
            }
            progressBar.IsIndeterminate = false; //hack progess bar things
            progressBar.Value = 100; // we're done (?)
        }

        private async void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            string result;
            try
            {
                var lines = File.ReadLines(file).Count();   //count how many lines are in our output file
                Random rnd = new Random();
                var num = rnd.Next(1, lines);               //get an int between 1 and the amount of lines in output file

                //The using block is a way to run a procedural code block and then dispose/destroy of any objects created within it
                //Read through the file and get the line we're looking for
                using (var sr = new StreamReader(file))
                {
                    for (int i = 1; i <= num; i++)
                    {
                        sr.ReadLine();
                    }
                     result = sr.ReadLine();
                }
                
                // remove hyphens for API call to Mojang
                string formattedId = result.Replace("-", "");
                
                //Send Mojang the API call
                NameHistoryResponse response = await new NameHistory(formattedId).PerformRequest();

                //Set the display of our winner text box
                txtWinner.Text = response.NameHistory[response.NameHistory.Count - 1].Name;
             
            }
            catch(FileNotFoundException)
            {
                //Our file is missing, did it get deleted before clicking select? Try parsing again
                taOutput.Text += "ERROR: Could not locate file";
            }
        }
    }
}
