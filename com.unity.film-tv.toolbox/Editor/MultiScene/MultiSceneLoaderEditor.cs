using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Unity.FilmTV.Toolbox.MultiScene
{
    /// <summary>
    /// Custom inspector for the MultiScene Scriptable Object
    /// </summary>
    [CustomEditor(typeof(MultiSceneLoader))]
	public class MultiSceneLoaderEditor : Editor
	{
        /// <summary>
        /// all of the localization for the UI
        /// </summary>
        private static class Loc
        {
            public const string WindowTitle = "Multi-Scene Config";
            public const string TopDesc = "Allows you to define sets of scenes that can be loaded either as one 'set' or individually as desired. Useful for defining subsets of a project that different team members can work on independently.";
            public const string ConfigList = "Config List";
            public const string ConfigName = "Config name: ";
            public const string SceneName = "Scene:";
            public const string AddNewScene = "Add New Scene";
            public const string MoveConfig = "Move Config";
            public const string MoveTop = "Top";
            public const string MoveUp = "Up";
            public const string MoveDown = "Down";
            public const string MoveBottom = "Last";
            public const string RemoveConfig = "Remove Config";
            public const string AddNewConfig = "Add new Config";
            public const string NewConfigName = "New Config";
            public const string LoadAllScenes = "Load All Scenes";
            public const string LoadAllScenesDesc = "Loads all configs in the order they are defined";
            public const string LoadSubScenes = "Load Sub Config Scenes";
            public const string LoadSubScenesDesc = "Load scenes defined in a specific config, from above.";
            public const string LoadOnlyScenes = "Loads ONLY the scenes defined in ";
            public const string LoadXScenes = "Load {0} Scenes";
            public const string SceneLoading = "Individual Config Loaders";
            public const string SceneLoadTip = "Load ALL scenes, or only specific scenes from a given config.";
        }

        /// <summary>
        /// The scriptable object that we are editing
        /// </summary>
        private static MultiSceneLoader sceneConfig;

        /// <summary>
        /// State for the foldout / dropdowns in the list UI
        /// </summary>
        private Dictionary<SceneConfig, bool> foldoutState = new Dictionary<SceneConfig, bool>();
        private static Vector2 topScroll = Vector2.zero;
        private static Vector2 botScroll = Vector2.zero;
        private static bool topToggle, botToggle = false;
        private static bool needToSave = false;

        /// <summary>
        /// used for sorting / moving the configs in the list
        /// </summary>
        private enum ListSort
        {
            MovetoTop,
            MoveToBottom,
            MoveUp,
            MoveDown,
        }


        /// <summary>
        /// Detect if we're deselected
        /// </summary>
        private void OnDisable()
        {
            if(needToSave)
            {
                SaveChanges(serializedObject);
            }
        }

        /// <summary>
        /// Draw our inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            sceneConfig = (MultiSceneLoader)target;

            EditorGUILayout.Space();
            GUILayout.Label(Loc.WindowTitle, EditorStyles.boldLabel);

            GUILayout.Label(Loc.TopDesc, EditorStyles.helpBox);

            GUILayout.Space(15f);

            topToggle = EditorGUILayout.Foldout(topToggle, Loc.ConfigList);
            if (topToggle)
            {
                topScroll = GUILayout.BeginScrollView(topScroll, false, true);
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(15f);

                            GUILayout.BeginVertical();
                            {
                                // Render our config editors
                                for (var j = 0; j < sceneConfig.config.Count; j++)
                                {
                                    var entry = sceneConfig.config[j];
                                    if (foldoutState.ContainsKey(entry))
                                    {
                                        foldoutState[entry] = EditorGUILayout.Foldout(foldoutState[entry], entry.name);
                                    }
                                    else
                                    {
                                        foldoutState.Add(entry, false);
                                        foldoutState[entry] = EditorGUILayout.Foldout(foldoutState[entry], entry.name);
                                    }

                                    if (foldoutState[entry])
                                    {
                                        GUILayout.BeginHorizontal();
                                        {
                                            GUILayout.Space(10f);
                                            GUILayout.BeginVertical();
                                            {
                                                GUILayout.BeginHorizontal();
                                                {
                                                    GUILayout.Label(Loc.ConfigName, GUILayout.Width(120f));
                                                    entry.name = GUILayout.TextField(entry.name);
                                                }
                                                GUILayout.EndHorizontal();
                                                // scene list
                                                for (var i = 0; i < entry.sceneList.Count; i++)
                                                {
                                                    GUILayout.BeginHorizontal();
                                                    {
                                                        entry.sceneList[i] = EditorGUILayout.ObjectField(Loc.SceneName, entry.sceneList[i], typeof(Object), false);
                                                        if (GUILayout.Button("-", GUILayout.Width(35f)))
                                                        {
                                                            entry.sceneList.Remove(entry.sceneList[i]);
                                                        }
                                                    }
                                                    GUILayout.EndHorizontal();
                                                }

                                                if (GUILayout.Button(Loc.AddNewScene))
                                                {
                                                    entry.sceneList.Add(new Object());
                                                }
                                                GUILayout.Space(15f);
                                                if (sceneConfig.config.Count > 1)
                                                {
                                                    GUILayout.Label(Loc.MoveConfig, EditorStyles.helpBox);
                                                    GUILayout.BeginHorizontal();
                                                    {
                                                        if (GUILayout.Button(Loc.MoveTop))
                                                        {
                                                            ReorderListEntry(entry, ListSort.MovetoTop);
                                                        }
                                                        if (GetConfigIndex(entry) != 0)
                                                        {
                                                            if (GUILayout.Button(Loc.MoveUp))
                                                            {
                                                                ReorderListEntry(entry, ListSort.MoveUp);
                                                            }
                                                        }
                                                        if (GetConfigIndex(entry) != sceneConfig.config.Count - 1)
                                                        {
                                                            if (GUILayout.Button(Loc.MoveDown))
                                                            {
                                                                ReorderListEntry(entry, ListSort.MoveDown);
                                                            }
                                                        }
                                                        if (GUILayout.Button(Loc.MoveBottom))
                                                        {
                                                            ReorderListEntry(entry, ListSort.MoveToBottom);
                                                        }
                                                    }
                                                    GUILayout.EndHorizontal();
                                                }
                                                if (GUILayout.Button(Loc.RemoveConfig))
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
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    if (GUILayout.Button(Loc.AddNewConfig))
                    {
                        var newConfig = new SceneConfig()
                        {
                            name = Loc.NewConfigName
                        };
                        sceneConfig.config.Add(newConfig);
                    }
                }
                GUILayout.Space(5f);
                GUILayout.EndScrollView();
            }

            EditorGUILayout.Space(10f);

            GUILayout.Label(Loc.LoadAllScenes, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            if (GUILayout.Button(Loc.LoadAllScenes, GUILayout.MinHeight(100), GUILayout.Height(35)))
                sceneConfig.LoadAllScenes();
            GUILayout.Label(Loc.LoadAllScenesDesc, EditorStyles.helpBox);

            EditorGUILayout.Space(5);

            botToggle = EditorGUILayout.Foldout(botToggle, Loc.SceneLoading);
            if (botToggle)
            {
                botScroll = GUILayout.BeginScrollView(botScroll, false, true);
                {
                    
                    EditorGUILayout.Space(5);
                    GUILayout.Label(Loc.LoadSubScenes, EditorStyles.boldLabel);
                    GUILayout.Label(Loc.LoadSubScenesDesc, EditorStyles.helpBox);

                    foreach (var entry in sceneConfig.config)
                    {
                        EditorGUILayout.Space(5);
                        var buttonText = string.Format(Loc.LoadXScenes, entry.name);
                        if (GUILayout.Button(buttonText, GUILayout.MinHeight(100), GUILayout.Height(35)))
                            sceneConfig.LoadSceneConfig(entry, true);
                        GUILayout.Label(Loc.LoadOnlyScenes + entry.name + ".", EditorStyles.helpBox);
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.Space(5f);
            }

            EditorGUILayout.Space();

            if( GUI.changed)
            {
                needToSave = true;
            }

            if( GUILayout.Button("Save Changes", GUILayout.Height(35f)))
            {
                if( needToSave)
                {
                    SaveChanges(serializedObject);
                }
            }
        }

        private static void SaveChanges(SerializedObject thisObject)
        {
            thisObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(sceneConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// List sorting functionality
        /// </summary>
        /// <param name="thisEntry">the entry you want to sort</param>
        /// <param name="sort">the sorting type</param>
        private void ReorderListEntry( SceneConfig thisEntry, ListSort sort)
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

        /// <summary>
        /// Retrieves the current index of the given config in our master list
        /// </summary>
        /// <param name="thisEntry">the entry that you are searching for</param>
        /// <returns>the current index in the master multiscene config list</returns>
        private int GetConfigIndex( SceneConfig thisEntry)
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