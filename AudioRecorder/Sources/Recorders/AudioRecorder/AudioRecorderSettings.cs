using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    
    public enum AudioRecorderOutputFormat
    {
       // MP3,
        WAV
    }
    
    [RecorderSettings(typeof(AudioRecorder), "Audio")]
    class AudioRecorderSettings : RecorderSettings
    {
        public AudioRecorderOutputFormat  outputFormat = AudioRecorderOutputFormat.WAV;
        
        [SerializeField] AudioInputSettings m_AudioInputSettings = new AudioInputSettings();

        public override string extension
        {
            get { return outputFormat.ToString().ToLower(); }
        }

        public AudioInputSettings audioInputSettings
        {
            get { return m_AudioInputSettings; }
        }
        
        public override IEnumerable<RecorderInputSettings> inputsSettings
        {
            get { yield return m_AudioInputSettings; }
        }


        public AudioRecorderSettings()
        {
            fileNameGenerator.fileName = "mixdown";
        }
    }
}