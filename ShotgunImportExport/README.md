Shotgun CSV Import Export
==========================

This is a simplified API example of how to create a timeline in Unity using a shot sequence from Shotgun.

How to use
--------
* Create a new project
* Import the [package](Bin/ShotgunImportExport.unitypackage) into your project
* Then through the menu `Window > Film-TV toolbox > Shotgun > Import CSV` import the example csv file
    * Alternatively, change the `k_pathToFile` and `k_importFileName` fields to the file you want to use
* Make edits to the timeline asset
* Then through the menu `Window > Film-TV toolbox > Shotgun > Import CSV` export to Shotgun-compatible tab-separated values. This will create a file name `export_from_unity.csv` in the same directory as the imported CSV file
* Use the import function of Shotgun to import the edits into your sequence

Limitations
-----------
* Having commas in fields (e.g.: `Opening sequence, part 2`) will confuse the parser
* Having duplicate column names will cause errors due to duplicate keys

