using UnityEditor;
using UnityEngine;

namespace Unity.FilmTV.Toolbox
{
    [CustomEditor(typeof(AlembicMaterialMapper))]
    public class AlembicMaterialRemapperInspector : Editor
    {
        public Object sourceObject;

        public override void OnInspectorGUI()
        {
            GUILayout.Label("This component will remap materials from a source FBX to an Alembic, assuming the mesh & material structure are the same.", EditorStyles.helpBox);
            GUILayout.Space(10f);
           // DrawDefaultInspector();

            var mapper = (AlembicMaterialMapper) target;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Source Object: ");
                sourceObject = EditorGUILayout.ObjectField(sourceObject, typeof(Object), true);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);

            if( sourceObject != null)
            {
                GUI.backgroundColor = new Color(0f, 64f, 0f);

                if (GUILayout.Button("Sync Materials", GUILayout.Height(35f)))
                {
                    Undo.RegisterCompleteObjectUndo(target, "AlembicMaterialMapper");
                    mapper.fbxLookDev = sourceObject as GameObject;
                    mapper.SyncMaterials();
                }
            }
            else
            {
                GUILayout.Label("Point the Source Object to an asset that contains the mesh / material structure that you wish to remap", EditorStyles.helpBox);
            }
            

        }
    }
}