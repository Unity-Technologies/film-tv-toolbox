using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace Unity.FilmTV.Toolbox.MultiScene
{
    /// <summary>
    /// Custom inspector for the MultiScene Scriptable Object
    /// </summary>
    [CustomEditor(typeof(MultiSceneLoader))]
	public class MultiSceneLoaderEditor : Editor
	{
        private MultiSceneLoader sceneConfig;
        public Dictionary<SceneConfig, bool> foldoutState = new Dictionary<SceneConfig, bool>();

        public enum ListSort
        {
            MovetoTop,
            MoveToBottom,
            MoveUp,
            MoveDown,
        }

        public override void OnInspectorGUI()
		{
			sceneConfig = (MultiSceneLoader)target;

            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Multi-Scene Config", EditorStyles.boldLabel);

                GUILayout.Label("Allows you to define sets of scenes that can be loaded either as one 'set' or individually as desired. Useful for defining subsets of a project that different team members can work on independently.", EditorStyles.helpBox);

                GUILayout.Space(15f);

                GUILayout.Label("Config List", EditorStyles.boldLabel);
                for( var j = 0; j < sceneConfig.config.Count; j++)
                {
                    var entry = sceneConfig.config[j];
                    if ( foldoutState.ContainsKey( entry))
                    {
                        foldoutState[entry] = EditorGUILayout.Foldout(foldoutState[entry], entry.name);
                    }
                    else
                    {
                        foldoutState.Add(entry, false);
                        foldoutState[entry] = EditorGUILayout.Foldout(foldoutState[entry], entry.name);
                    }

                    if( foldoutState[entry])
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(10f);
                            GUILayout.BeginVertical();
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("Config name: ", GUILayout.Width(120f));
                                    entry.name = GUILayout.TextField(entry.name);
                                }
                                GUILayout.EndHorizontal();
                                // scene list
                                for (var i = 0; i < entry.sceneList.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        entry.sceneList[i] = EditorGUILayout.ObjectField("Scene:", entry.sceneList[i], typeof(Object), false);
                                        if (GUILayout.Button("-", GUILayout.Width(35f)))
                                        {
                                            entry.sceneList.Remove(entry.sceneList[i]);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }

                                if (GUILayout.Button("Add New Scene"))
                                {
                                    entry.sceneList.Add(new Object());
                                }
                                GUILayout.Space(15f);
                                if (sceneConfig.config.Count > 1)
                                {
                                    GUILayout.Label("Move Config", EditorStyles.helpBox);
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button("Top"))
                                        {
                                            ReorderListEntry(entry, ListSort.MovetoTop);
                                        }
                                        if( GetConfigIndex(entry) != 0)
                                        {
                                            if (GUILayout.Button("Up"))
                                            {
                                                ReorderListEntry(entry, ListSort.MoveUp);
                                            }
                                        }
                                        if (GetConfigIndex(entry) != sceneConfig.config.Count -1)
                                        {
                                            if (GUILayout.Button("Down"))
                                            {
                                                ReorderListEntry(entry, ListSort.MoveDown);
                                            }
                                        }
                                        if (GUILayout.Button("Last"))
                                        {
                                            ReorderListEntry(entry, ListSort.MoveToBottom);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                if (GUILayout.Button("Remove Config"))
                                {
                                    sceneConfig.config.Remove(entry);
                                    foldoutState.Remove(entry);
                                }
                                GUILayout.Space(15f);

                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                if( GUILayout.Button("Add new Config"))
                {
                    var newConfig = new SceneConfig()
                    {
                        name = "New Config"
                    };
                    sceneConfig.config.Add(newConfig);
                }

                //DrawDefaultInspector(); // TODO: make a proper inspector for the configs so I don't need to use the default inspector

                EditorGUILayout.Space();
                GUILayout.Label("Load All Scenes", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                if (GUILayout.Button("Load All Scenes", GUILayout.MinHeight(100), GUILayout.Height(50)))
                    sceneConfig.LoadAllScenes();
                GUILayout.Label("Loads all Main Scenes and then all Set Scenes", EditorStyles.helpBox);

                EditorGUILayout.Space();
                GUILayout.Label("Load Sub Config Scenes", EditorStyles.boldLabel);
                GUILayout.Label("Load scenes defined in a specific config, from above.", EditorStyles.helpBox);

                foreach (var entry in sceneConfig.config)
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Load " + entry.name + " Scenes", GUILayout.MinHeight(100), GUILayout.Height(50)))
                        sceneConfig.LoadSceneConfig(entry, true);
                    GUILayout.Label("Loads ONLY the scenes defined in " + entry.name + ".", EditorStyles.helpBox);
                }
            }
            GUILayout.EndVertical();
		}

        public void ReorderListEntry( SceneConfig thisEntry, ListSort sort)
        {
            if (thisEntry == null)
                return;

            // get our current index
            var index = GetConfigIndex(thisEntry);
            // remove the old entry
            sceneConfig.config.RemoveAt(index);
            switch (sort)
            {
                case ListSort.MovetoTop:
                    {
                        // insert at the top
                        sceneConfig.config.Insert(0, thisEntry);
                        break;
                    }
                case ListSort.MoveToBottom:
                    {
                        // add it to the end
                        sceneConfig.config.Add(thisEntry);
                        break;
                    }
                case ListSort.MoveUp:
                    {
                        var newIndex = (index - 1);
                        sceneConfig.config.Insert(newIndex, thisEntry);
                        break;
                    }
                case ListSort.MoveDown:
                    {
                        var newIndex = (index + 1);
                        sceneConfig.config.Insert(newIndex, thisEntry);
                        break;
                    }
            }
        }

        public int GetConfigIndex( SceneConfig thisEntry)
        {
            var index = -1;
            if (sceneConfig == null)
                return index;

            if (thisEntry == null)
                return index;

            if (!sceneConfig.config.Contains(thisEntry))
                return index;
            
            for( var i = 0; i < sceneConfig.config.Count; i++)
            {
                if (sceneConfig.config[i] == thisEntry)
                {
                    index = i;
                }
            }

            return index;

        }
	}
}