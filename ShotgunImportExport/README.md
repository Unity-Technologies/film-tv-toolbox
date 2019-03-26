Shotgun CSV Import Export
==========================

This is a simplified example of how to create a timeline in Unity using a shot sequence from Shotgun.

How to use
--------
* Create a new project
* Import the [package](Bin/ShotgunImportExport.unitypackage) into your project
* Then through the menu `Shotgun CSV > Import` import the example csv file
    * Alternatively, change the `pathToFile` and `filePath` fields to the file you want to use
* Make edits to the timeline
* Then through the menu `Shotgun CSV > Export` export to Shotgun-compatible tab-separated values. This will create a file name `export_from_unity.csv` in the same directory as the imported CSV file
* Use the import function of Shotgun to import the edits into your sequence

##Limitations
* Having commas in fields (e.g.: `Opening sequence, part 2`) will confuse the parser
* Having duplicate column names will cause errors due to duplicate keys
* There is currently no other way than through the UI to change the framerate of the timeline. Unity's default is 60

