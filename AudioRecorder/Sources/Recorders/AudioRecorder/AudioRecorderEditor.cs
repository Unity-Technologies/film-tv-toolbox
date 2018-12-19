using UnityEngine;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(AudioRecorderSettings))]
    class AudioRecorderEditor : RecorderEditor
    {

        SerializedProperty m_OutputFormat;

        static class Styles
        {
            internal static readonly GUIContent FormatLabel = new GUIContent("Format");
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;
            
            m_OutputFormat = serializedObject.FindProperty("outputFormat");

        }

        protected override void FileTypeAndFormatGUI()
        {
            EditorGUILayout.PropertyField(m_OutputFormat, Styles.FormatLabel);
        }
    }
}