# Raffler
This app was created for handling the monthly voting raffle at Everneth.com
# Usage
Select a JSON file for parsing. The accepted format is shown below.
```json
{
  "uuid" : "num"
}
```
After you have selected the file in the dialog box, click parse. This will iterate through your JSON file and create a `results.txt` file with uuid printed for every num of votes. After the parse is complete, the progress bar will be full and you can then click select. This will randomly choose a Winner from the parsed list, convert the UUID to a Name from the Mojang API and display the name in the bottom text box.

##### Created by @TptMike (c) 2017 - Provided AS IS
