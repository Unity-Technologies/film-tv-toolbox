using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEditor.Media;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    class AudioRecorder : GenericRecorder<AudioRecorderSettings>
    {

        private WavEncoder m_Encoder;
        
        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session))
                return false;
            
            try
            {
                m_Settings.fileNameGenerator.CreateDirectory(session);
            }
            catch (Exception)
            {
                Debug.LogError(string.Format( "Audio recorder output directory \"{0}\" could not be created.", m_Settings.fileNameGenerator.BuildAbsolutePath(session)));
                return false;
            }
            
            var audioInput = (AudioInput)m_Inputs[0];
            var audioAttrsList = new List<AudioTrackAttributes>();
            
            if (audioInput.audioSettings.preserveAudio)
            {
                var audioAttrs = new AudioTrackAttributes
                {
                    sampleRate = new MediaRational
                    {
                        numerator = audioInput.sampleRate,
                        denominator = 1
                    },
                    channelCount = audioInput.channelCount,
                    language = ""
                };
                
                audioAttrsList.Add(audioAttrs);
                
                if (Options.verboseMode)
                    Debug.Log(string.Format("Audio starting to write audio {0}ch @ {1}Hz", audioAttrs.channelCount, audioAttrs.sampleRate.numerator));
            }

            try
            {
                var path =  m_Settings.fileNameGenerator.BuildAbsolutePath(session);
                m_Encoder = new WavEncoder(path);
                
                return true;
            }
            catch
            {
                if (Options.verboseMode)
                    Debug.LogError("AudioRecorder unable to create MovieEncoder.");
            }
            
            return false;

        }


        public override void RecordFrame(RecordingSession session)
        {
            var audioInput = (AudioInput)m_Inputs[0];
            
            if (!audioInput.audioSettings.preserveAudio)
                return;

            m_Encoder.AddSamples(audioInput.mainBuffer);
        }
        
        
        public override void EndRecording(RecordingSession session)
        {
            base.EndRecording(session);
            
            if (m_Encoder != null)
            {
                m_Encoder.Dispose();
                m_Encoder = null;
            }
            
            // When adding a file to Unity's assets directory, trigger a refresh so it is detected.
            if (settings.fileNameGenerator.root == OutputPath.Root.AssetsFolder || settings.fileNameGenerator.root == OutputPath.Root.StreamingAssets)
                AssetDatabase.Refresh();
            
            
        }
    }

    internal class WavEncoder
    {
        
        BinaryWriter _binwriter;

        // Use this for initialization
        public WavEncoder (string filename)
        {
            var stream = new FileStream (filename, FileMode.Create);
            _binwriter = new BinaryWriter (stream);
            for(int n = 0; n < 44; n++)
                _binwriter.Write ((byte)0);
        }

        public void Stop()
        {
            var closewriter = _binwriter;
            _binwriter = null;
            int subformat = 3; // float
            int numchannels = AudioSettings.speakerMode == AudioSpeakerMode.Mono ? 1 : 2;
            int numbits = 32;
            int samplerate = AudioSettings.outputSampleRate;
            
            if (Options.verboseMode)
                Debug.Log ("Closing file");
            
            long pos = closewriter.BaseStream.Length;
            closewriter.Seek (0, SeekOrigin.Begin);
            closewriter.Write ((byte)'R'); closewriter.Write ((byte)'I'); closewriter.Write ((byte)'F'); closewriter.Write ((byte)'F');
            closewriter.Write ((uint)(pos - 8));
            closewriter.Write ((byte)'W'); closewriter.Write ((byte)'A'); closewriter.Write ((byte)'V'); closewriter.Write ((byte)'E');
            closewriter.Write ((byte)'f'); closewriter.Write ((byte)'m'); closewriter.Write ((byte)'t'); closewriter.Write ((byte)' ');
            closewriter.Write ((uint)16);
            closewriter.Write ((ushort)subformat);
            closewriter.Write ((ushort)numchannels);
            closewriter.Write ((uint)samplerate);
            closewriter.Write ((uint)((samplerate * numchannels * numbits) / 8));
            closewriter.Write ((ushort)((numchannels * numbits) / 8));
            closewriter.Write ((ushort)numbits);
            closewriter.Write ((byte)'d'); closewriter.Write ((byte)'a'); closewriter.Write ((byte)'t'); closewriter.Write ((byte)'a');
            closewriter.Write ((uint)(pos - 36));
            closewriter.Seek ((int)pos, SeekOrigin.Begin);
            closewriter.Flush ();
        }

        public void AddSamples(NativeArray<float> data)
        {
            
            if (Options.verboseMode)
                Debug.Log ("Writing wav chunk " + data.Length);

            if (_binwriter == null)
                return;

            for(int n = 0; n < data.Length; n++)
                _binwriter.Write (data[n]);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}