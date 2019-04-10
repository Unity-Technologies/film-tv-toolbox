using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor;
using UnityEditor.FilmTV.Toolbox;

public static class CsvImportExport
{
    const int k_FPS = 24;
    const string k_ImportFileName = "shot.csv";
    const string k_ExportFileName = "export_from_unity.csv";
    const string k_TimelineAssetName = "shotgun_imported_timeline.asset";
    static string s_PathToFile = PackageUtils.GetCallerRelativeToProjectFolderPath();
    const string k_TimelineObjectName = "Sample Shotgun CSV timeline";

    [MenuItem("Window/Film-TV toolbox/Samples/Shotgun/Import CSV")]
    public static void ImportCsv ()
    {
        var timeline = ScriptableObject.CreateInstance("TimelineAsset") as TimelineAsset;
        timeline.name = k_TimelineObjectName;
        timeline.editorSettings.fps = k_FPS;
        var track = timeline.CreateTrack<ControlTrack>(null, "");

        foreach (var row in ReadCsvFile(Path.Combine(s_PathToFile, k_ImportFileName)))
        {
            var clip = track.CreateClip<ControlPlayableAsset>();
            var clipAsset  = clip.asset as ControlPlayableAsset;

            clipAsset.name = row["Shot Code"];
            clip.displayName = row["Shot Code"];
            clip.start =  (Convert.ToDouble(row["Cut In"]) / k_FPS);
            // In Unity, the last frame is exclusive. frames goes : [start; end[
            clip.duration = (Convert.ToDouble(row["Cut Out"]) - Convert.ToDouble(row["Cut In"]) + 1 ) / k_FPS;
        }

        Debug.Log($"Loaded {k_ImportFileName} from {s_PathToFile}.");
        AssetDatabase.CreateAsset(timeline, Path.Combine(s_PathToFile, k_TimelineAssetName ));
        Debug.Log($"Created Timeline Asset in {s_PathToFile}.");
    }

    [MenuItem("Window/Film-TV toolbox/Samples/Shotgun/Export CSV")]
    public static void ExportCsv()
    {
        // Get the actual timeline object
        var timelineAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(Path.Combine(s_PathToFile, k_TimelineAssetName));

        // Each dictionary in the list represents a row in the CSV file
        // Each value in the row is addressable by its column name
        var toWrite = new List<Dictionary<string, string>>();

        var track = timelineAsset.GetOutputTrack(0);
        foreach (var clip in track.GetClips())
        {
            var start = Convert.ToInt64(clip.start * k_FPS);
            // In Shotgun, the last frame is inclusive. frames goes : [start; end]
            // remove the "padding" we added
            var end = Convert.ToInt64((clip.duration + clip.start) * k_FPS) - 1;

            var clipAsset  = clip.asset as ControlPlayableAsset;
            var shotCode = clipAsset.name;

            var dict = new Dictionary<string, string>
            {
                {"Shot Code", shotCode },
                {"Cut In", start.ToString() },
                {"Cut Out", end.ToString() }
            };
            toWrite.Add(dict);
        }

        var pathToOutputFile = Path.Combine(s_PathToFile, k_ExportFileName);
        Debug.Log($"Exported clip updates in {pathToOutputFile}");
        WriteCsv(toWrite, pathToOutputFile);
    }

    static IEnumerable<Dictionary<string, string>> ReadCsvFile(string filePath)
    {
        var output = new List<Dictionary<string, string>>();

        using (var reader = new StreamReader(filePath))
        {
            var line = reader.ReadLine();
            if (line == null)
                return output;

            var fieldNamesList = line.Split(',');
            // Trim the double quotes
            fieldNamesList = fieldNamesList.Select(x => x.Replace("\"", "")).ToArray();

            line = reader.ReadLine();

            while (null != line)
            {
                var dict = new Dictionary<string, string>();

                var values = line.Split(',');
                // Trim the double quotes
                values = values.Select(x => x.Replace("\"", "")).ToArray();

                for (long idx = 0; idx < fieldNamesList.Length; idx++)
                {
                    dict.Add(fieldNamesList[idx], values[idx]);
                }

                output.Add(dict);

                line = reader.ReadLine();
            }
        }
        return output;
    }

    static void WriteCsv (IReadOnlyList<Dictionary<string, string>> input, string filePath)
    {
        //Keys should be uniform
        var keys = input[0].Keys;
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine(string.Join("\t", keys));
            foreach (var item in input)
            {
                writer.WriteLine(string.Join("\t", item.Values));
            }
        }
    }
}
