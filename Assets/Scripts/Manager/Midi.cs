using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
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
        #region Generic


        private InputDevice inputDevice;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputDevice.EventReceived += OnEventReceived;

            inputDevice.StartEventsListening();
        }

        protected override void OnDisable()
        {
            inputDevice.StopEventsListening();

            inputDevice.EventReceived -= OnEventReceived;

            base.OnDisable();
        }

        protected void OnApplicationQuit()
        {
            inputDevice.Dispose();
        }

        protected void Awake()
        {
            inputDevice = InputDevice.GetByIndex(0);

            Debug.Log($"DBG {inputDevice.Name}");
        }

        protected void Update()
        {
            if (noteOn.HasValue)
            {
                onNoteOn?.Invoke(noteOn.Value);

                noteOn = null;
            }
        }

        private (int noteNumber, int velocity)? noteOn;

        protected void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            if (e.Event.EventType == MidiEventType.NoteOn)
            {
                var noteOnEvent = (NoteOnEvent)e.Event;

                noteOn = (noteOnEvent.NoteNumber, noteOnEvent.Velocity);
            }
        }

        protected void NoteOn_Instance((int noteNumber, int velocity) noteOn)
        {
            onNoteOn?.Invoke((noteOn.noteNumber, noteOn.velocity));
        }


        #endregion Generic

        protected static new string 名 => "[<color=violet>MIDI長</color>]";
    }
}