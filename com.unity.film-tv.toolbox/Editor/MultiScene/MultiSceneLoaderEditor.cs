using UnityEngine;
using UnityEditor;

namespace Unity.FilmTV.Toolbox.MultiScene
{
    /// <summary>
    /// Custom inspector for the MultiScene Scriptable Object
    /// </summary>
    [CustomEditor(typeof(MultiSceneLoader))]
	public class MultiSceneLoaderEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var sceneConfig = (MultiSceneLoader)target;

            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Multi-Scene Config", EditorStyles.boldLabel);

                GUILayout.Label("Allows you to define sets of scenes that can be loaded either as one 'set' or individually as desired. Useful for defining subsets of a project that different team members can work on independently.", EditorStyles.wordWrappedLabel);

                DrawDefaultInspector(); // TODO: make a proper inspector for these so I don't need to use the default inspector

                EditorGUILayout.Space();
                GUILayout.Label("Load All Scenes", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                if (GUILayout.Button("Load All Scenes", GUILayout.MinHeight(100), GUILayout.Height(50)))
                    sceneConfig.LoadAllScenes();
                GUILayout.Label("Loads all Main Scenes and then all Set Scenes", EditorStyles.wordWrappedLabel);

                EditorGUILayout.Space();
                GUILayout.Label("Load Sub Config Scenes", EditorStyles.boldLabel);
                GUILayout.Label("Load scenes defined in a specific config, from above.", EditorStyles.wordWrappedLabel);

                foreach (var entry in sceneConfig.config)
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Load " + entry.name + " Scenes", GUILayout.MinHeight(100), GUILayout.Height(50)))
                        sceneConfig.LoadSceneConfig(entry);
                    GUILayout.Label("Loads ONLY the scenes defined in " + entry.name + ".", EditorStyles.wordWrappedLabel);
                }
            }
            GUILayout.EndVertical();
		}
	}
}