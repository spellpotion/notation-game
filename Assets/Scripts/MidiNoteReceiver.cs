using jp.kshoji.midisystem;
using UnityEngine;

namespace spellpotion.midiTutor
{
    public class MidiNoteReceiver : IReceiver
    {
        public void Close() { }

        public void Send(MidiMessage message, long timeStamp)
        {
            Debug.Log($"[MidiNoteReceiver] Send {message} timestamp {timeStamp}");
        }
    }
}