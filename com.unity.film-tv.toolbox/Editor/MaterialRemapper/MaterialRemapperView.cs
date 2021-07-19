using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.FilmTV.Toolbox
{
    [Serializable]
    public class MaterialRemapperView : EditorWindow
    {
        [SerializeField]
        Transform[] selection;                                    // user scene selection
        
        // UI States
        bool showGlobalUI;
        bool showMeshRemapList;
        [SerializeField]
        MaterialRemapperModel model;

        static MaterialRemapperView instance;
        Vector2 scrollPos;

        
        /// <summary>
        ///  initialization
        /// </summary>
        [MenuItem(MaterialRemapperLocalization.menuItemLabel)]
        public static void ShowWindow()
        {
            var thisWindow = GetWindow<MaterialRemapperView>(false, MaterialRemapperLocalization.windowLabel, true);
            thisWindow.ResetData();
        }

        /// <summary>
        /// reset ourself
        /// </summary>
        void ResetData()
        {
            model.ResetData();
        }

        
        void ResetSceneSelection()
        {
            model.ResetData();
        }
        
        void OnEnable()
        {
            instance = this;
            if (model == null)
                instance.model = CreateInstance<MaterialRemapperModel>();
            
            instance.selection = Selection.GetTransforms(
                SelectionMode.TopLevel | SelectionMode.Editable);

            Undo.RegisterCompleteObjectUndo(instance.selection, "Replace Materials in Selection");

            instance.model.objectsToRemap = instance.selection.ToList();
        }
        
        /// <summary>
        /// Draw the UI
        /// </summary>
        void OnGUI()
        {
            var currentHeight = position.height;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(currentHeight));
            {
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(MaterialRemapperLocalization.updateSceneSelection, GUILayout.Width(300), GUILayout.Height(35)))
                        {
                            selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable);
                            Debug.Log("Selection updated, found : " + selection.Length + " entries");
                            // reset the scene selection (doesn't reset the material list)
                            ResetSceneSelection();
                            model.FindMeshesAndMaterials(selection.ToList());        // build a list of MeshRenderers from our selection
                        }
                        if (GUILayout.Button(MaterialRemapperLocalization.resetRemapper, GUILayout.Width(300), GUILayout.Height(35)))
                        {
                            // reset everything
                            ResetData();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    showGlobalUI = model.objectsToRemap.Count > 0;    
                    GUILayout.Space(20);
                    GUILayout.Label(MaterialRemapperLocalization.meshSelectLabel, EditorStyles.boldLabel, GUILayout.Width(500));

                    // hide the UI unless we have a selection
                    if (showGlobalUI)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            // list of objects that we want to remap
                            EditorGUILayout.BeginVertical(GUILayout.Width(250));
                            {
                                GUILayout.Space(10);
                                // list of objects
                                GUILayout.Label(MaterialRemapperLocalization.objectsToRemap, EditorStyles.boldLabel,
                                    GUILayout.MinWidth(400));
                                // list the objects
                                foreach (var entry in model.objectsToRemap)
                                {
                                    EditorGUILayout.ObjectField(entry.name, entry, typeof(Object), true,
                                        GUILayout.MinWidth(400));
                                }

                                // temp debug values
                                GUILayout.Label(MaterialRemapperLocalization.selectionDetails, EditorStyles.boldLabel,
                                    GUILayout.Width(250));
                                var meshCount = model.GetMeshCount();
                                GUILayout.Label(MaterialRemapperLocalization.meshCountInSelection + meshCount,
                                    GUILayout.Width(250));
                                var uniqueMesh = model.GetUniqueMeshList().Count;
                                GUILayout.Label(MaterialRemapperLocalization.uniqueMeshCount + uniqueMesh,
                                    GUILayout.Width(250));
                            }
                            EditorGUILayout.EndVertical();
                        }
                        {
                            // list of materials that we want to use for remapping
                            EditorGUILayout.BeginVertical(GUILayout.Width(250));
                            {
                                // list of materials
                                GUILayout.Label(MaterialRemapperLocalization.materialsLibrary, EditorStyles.boldLabel,
                                    GUILayout.MinWidth(400));

                                EditorGUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button(MaterialRemapperLocalization.addNewMaterialEntry,
                                        GUILayout.Width(400), GUILayout.Height(35)))
                                    {
                                        var newMat = MaterialRemapperModel.GetDefaultMaterial();
                                        // add to our temp list of materials
                                        model.materialList.Add(newMat);
                                        newMat.name = "MaterialRemap";
                                    }
                                    
                                }
                                EditorGUILayout.EndHorizontal();

                                for (var i = 0; i < model.materialList.Count; i++)
                                {
                                    model.materialList[i] = (Material) EditorGUILayout.ObjectField(model.materialList[i].name,
                                        model.materialList[i], typeof(Material), true, GUILayout.MinWidth(400));
                                }
                                
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                        {
                            GUILayout.Space(20);


                            // if we have any, show the remapper
                            showMeshRemapList = model.materialList.Count > 0;

                            if (showMeshRemapList)
                            {
                                GUILayout.Label(MaterialRemapperLocalization.meshRemapLabel, EditorStyles.boldLabel, GUILayout.Width(500));

                                // remapping UI
                                EditorGUILayout.BeginHorizontal(GUILayout.Width(500));
                                {
                                    // mesh list to map to materials
                                    EditorGUILayout.BeginVertical();
                                    {
                                        GUILayout.Label(MaterialRemapperLocalization.remapMesh, EditorStyles.boldLabel, GUILayout.Width(500));

                                        // list the objects
                                        foreach (var remapItem in model.remapList)
                                        {
                                            EditorGUILayout.BeginHorizontal();

                                            // mesh that we want to remap
                                            GUILayout.Label(remapItem.mesh.name, GUILayout.Width(200));

                                            // show all of the materials in the mesh
                                            for (var j = 0; j < remapItem.mesh.sharedMaterials.Length; j++)
                                            {
                                                // do we have any materials to remap
                                                remapItem.selectedIndex[j] = EditorGUILayout.Popup(remapItem.selectedIndex[j], model.materialList.Select(x=>x.name).ToArray(), GUILayout.Width(200));
                                            }

                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            else
                            {
                                GUILayout.Label(MaterialRemapperLocalization.noMaterialToRemap, EditorStyles.boldLabel, GUILayout.Width(500));
                            }
                        }
                        if (showGlobalUI)
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Apply Material Changes", GUILayout.Width(300), GUILayout.Height(35)))
                            {
                                model.ApplyMaterialChanges();
                            }
                        }
                        GUILayout.Space(20);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        
        
        
        /// <summary>
        /// Localization table for the MaterialRemapperView class
        /// </summary>
        static class MaterialRemapperLocalization
        {
            // UI Labels
            public const string menuItemLabel = "Window/Film-TV toolbox/Material Remapper";
            public const string windowLabel = "Material Remapper";
            public const string meshSelectLabel = "Select Scene objects to remap";
            public const string meshRemapLabel = "Remap materials to object meshes";
            public const string updateSceneSelection = "Update Scene Selection";
            public const string resetRemapper = "Reset Remapper";
            public const string objectsToRemap = "Objects to Remap";
            public const string selectionDetails = "Selection Details:";
            public const string meshCountInSelection = "   Mesh Count in selection: ";
            public const string uniqueMeshCount = "   Unique Mesh Count: ";
            public const string materialsLibrary = "Materials Library";
            public const string addNewMaterialEntry = "Add new Material Entry";
            public const string remapMesh = "Mesh to Remap";
            public const string noMaterialToRemap = "Add some materials to remap";
        }
    }
}

