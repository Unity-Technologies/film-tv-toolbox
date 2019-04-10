Shotgun CSV Import Export
--------------------------

This sample shows how to create a timeline in Unity using a shot sequence from a Shotgun CSV export and export back modifications.

### How to install

* Import the sample via Package Manager UI
* This will import in your Assets folder :
    * CSVImportExport.cs : a sample script demonstrating how to simply import / export a Shotgun CSV file
    * shot.csv : a shotgun csv export sample, used in the import.

### How to test 

* Go to `Window > Film-TV toolbox > Samples > Shotgun` and select `Import CSV`
    * This creates a Timeline Asset (see shotgun_imported_timeline in the sample folder) from the `shot.csv` sample file

* Make edits to the Timeline asset

* Go to `Window > Film-TV toolbox > Samples > Shotgun` and select `Export CSV`:
    * See export_from_unity.csv in the sample folder

### Limitations

* Having commas in fields (e.g.: `Opening sequence, part 2`) will confuse the parser
* Having duplicate column names will cause errors due to duplicate keys
