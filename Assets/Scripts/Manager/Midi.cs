#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#else
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using spellpotion.Manager;

#endif
using System;
using UnityEngine;

namespace spellpotion.midiTutor.Manager
{
    public class Midi : 抽象Manager<Midi, Config.Midi>
    {
        #region Events


        public static ActionEvent<(int noteNumber, int velocity)> OnNoteOn = new(out onNoteOn);
        private static Action<(int noteNumber, int velocity)> onNoteOn;


        #endregion Events
        #region PublicStatic


        public static readonly int Offset = 45;

        public static void NoteOn((int noteNumber, int velocity) noteOn)
            => InstanceRun(x => x.NoteOn_Instance(noteOn));


        #endregion PublicStatic
        #region Common

#if !UNITY_WEBGL || UNITY_EDITOR
        private InputDevice inputDevice;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (inputDevice != null)
            {
                inputDevice.EventReceived += OnEventReceived;
                inputDevice.StartEventsListening();
            }

        }

        protected override void OnDisable()
        {
            if (inputDevice != null)
            {
                inputDevice.StopEventsListening();
                inputDevice.EventReceived -= OnEventReceived;
            }

            base.OnDisable();
        }
#endif

        protected void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebMIDI_Init(gameObject.name, nameof(OnMIDIMessage), nameof(OnMIDIReady), nameof(OnMIDIError));
#else
            if (InputDevice.GetDevicesCount() > 0)
            {
                inputDevice = InputDevice.GetByIndex(0);
            }
#endif
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        protected void Update()
        {
            if (noteOn.HasValue)
            {
                onNoteOn?.Invoke(noteOn.Value);

                noteOn = null;
            }
        }

        protected void OnApplicationQuit()
        {
            if (inputDevice != null)
            {
                inputDevice.Dispose();
            }
        }
#endif

        private (int noteNumber, int velocity)? noteOn;

#if !UNITY_WEBGL || UNITY_EDITOR
        protected void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            if (e.Event.EventType == MidiEventType.NoteOn)
            {
                var noteOnEvent = (NoteOnEvent)e.Event;
                noteOn = (noteOnEvent.NoteNumber, noteOnEvent.Velocity);
            }
        }
#endif

        protected void NoteOn_Instance((int noteNumber, int velocity) noteOn)
        {
            onNoteOn?.Invoke((noteOn.noteNumber, noteOn.velocity));
        }


        #endregion Common
        #region WebMIDI


#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void WebMIDI_Init(string goName, string onMsg, string onReady, string onError);
        [DllImport("__Internal")] private static extern void WebMIDI_Send(string outputId, int b0, int b1, int b2);

        public void OnMIDIReady(string msg) => Debug.Log($"{名} {msg}");
        public void OnMIDIError(string err) => Debug.LogError($"{名} {err}");
        public void OnMIDIMessage(string json)
        {
            var message = JsonUtility.FromJson<MidiMessageJson>(json);

            if (TryParseNoteOn(message.data, out var data))
            {
                onNoteOn?.Invoke((data.Note, data.Velocity));
            }
        }

        [Serializable]
        private class MidiMessageJson
        {
            public string deviceId;
            public string name;
            public double ts;
            public byte[] data;
        }

        private struct MidiNoteOn
        {
            public int Channel;
            public int Note;
            public int Velocity;
        }

        private static bool TryParseNoteOn(byte[] data, out MidiNoteOn noteOn)
        {
            noteOn = default;

            if (data == null || data.Length < 3) return false;

            byte status = data[0];
            byte messageType = (byte)(status & 0xF0);

            int channel = status & 0x0F;

            if (messageType != 0x90) return false;

            int note = data[1];
            int velocity = data[2];

            if (velocity == 0) return false;

            noteOn = new MidiNoteOn
            {
                Channel = channel,
                Note = note,
                Velocity = velocity
            };

            return true;
        }
#endif

        #endregion WebMIDI
    }
}