using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor;

public static class CsvImportExport
{
    const int k_FPS = 24;
    const string k_importFileName = "shot.csv";
    const string k_exportFileName = "export_from_unity.csv";
    const string k_timelineAssetName = "shotgun_imported_timeline.asset";
    static string k_pathToFile = Path.Combine("Assets", "CsvImportExport", "Editor");
    const string k_timelineObjectName = "Sample Shotgun CSV timeline";

    [UnityEditor.MenuItem("Window/General/Film & TV toolbox/Shotgun/Import CSV")]
    public static void ImportCsv ()
    {
        TimelineAsset timeline;

        timeline = ScriptableObject.CreateInstance("TimelineAsset") as TimelineAsset;
        timeline.name = k_timelineObjectName;
        timeline.editorSettings.fps = k_FPS;
        
        foreach (var row in ReadCsvFile(Path.Combine(k_pathToFile, k_importFileName)))
        {
            ActivationTrack track = timeline.CreateTrack<ActivationTrack>(null, "");
            TimelineClip clip = track.CreateDefaultClip();
            track.name = row["Shot Code"];
            clip.start =  (Convert.ToDouble(row["Cut In"]) / k_FPS);
            clip.duration = ((Convert.ToDouble(row["Cut Out"]) - Convert.ToDouble(row["Cut In"])) / k_FPS);
            AssetDatabase.CreateAsset(track, Path.Combine(k_pathToFile, String.Format("{0}.track.asset", track.name)));
        }
        AssetDatabase.CreateAsset(timeline, Path.Combine(k_pathToFile, k_timelineAssetName ));
    }

    [UnityEditor.MenuItem("Window/General/Film & TV toolbox/Shotgun/Export CSV")]
    public static void ExportCsv()
    {

        // Get the actual timeline object
        var timelineAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(Path.Combine(k_pathToFile, k_timelineAssetName));

        // Each dictionary in the list represents a row in the CSV file
        // Each value in the row is addressable by its column name
        var toWrite = new List<Dictionary<string, string>>();

        foreach (var track in timelineAsset.GetOutputTracks())
        {
            
            var start = Convert.ToInt64(track.start * k_FPS);
            var end = Convert.ToInt64((track.duration + track.start) * k_FPS);
            var shotCode = track.name;

            var dict = new Dictionary<string, string>() 
            {
                {"Shot Code", shotCode },
                {"Cut In", start.ToString() },
                {"Cut Out", end.ToString() },
            };
            toWrite.Add(dict);
        }

        var pathToOutputFile = Path.Combine(k_pathToFile, k_exportFileName);
        Debug.Log(pathToOutputFile);
        WriteCsv(toWrite, pathToOutputFile);
    }

    private static List<Dictionary<string, string>> ReadCsvFile(string filePath)
    {
        var output = new List<Dictionary<string, string>>();

        using (var reader = new StreamReader(filePath))
        {
            var fieldNamesList = reader.ReadLine().Split(',');
            // Trim the double quotes
            fieldNamesList = fieldNamesList.Select(x => x.Replace("\"", "")).ToArray();
            
            string line = reader.ReadLine();

            while (null != line)
            {
                var dict = new Dictionary<string, string>();

                var values = line.Split(',');
                // Trim the double quotes
                values = values.Select(x => x.Replace("\"", "")).ToArray();

                for (Int64 idx = 0; idx < fieldNamesList.Length; idx++)
                {
                    dict.Add(fieldNamesList[idx], values[idx]);
                }

                output.Add(dict);

                line = reader.ReadLine();
            }
        }
        return output;
    }

    private static void WriteCsv (List<Dictionary<string, string>> input, string filePath)
    {
        //Keys should be uniform
        var keys = input[0].Keys;
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine(String.Join("\t", keys));
            foreach (var item in input)
            {
                writer.WriteLine(String.Join("\t", item.Values));
            }
        }
    }
}
