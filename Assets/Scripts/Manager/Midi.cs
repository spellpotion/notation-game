using jp.kshoji.midisystem;
using jp.kshoji.unity.midi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace spellpotion.midiTutor.Manager
{
    public class Midi : 抽象Manager<Midi, Config.Midi>,
        IMidiAllEventsHandler, IMidiDeviceEventHandler
    {
        #region Events


        public static ActionEvent<int> OnNoteOn = new(out onNoteOn);
        private static Action<int> onNoteOn;


        #endregion Events

        private readonly List<string> receivedMidiMessages = new();

        private void Awake()
        {
            MidiManager.Instance.RegisterEventHandleObject(gameObject);
            MidiManager.Instance.InitializeMidi(() => { });

            MidiSystem.AddReceiver("MidiNoteReceiver", new MidiNoteReceiver());
        }

        private void Report()
        {
            Debug.Log($"[MIDI Debug] {receivedMidiMessages[^1]}");
        }

        public void OnMidiActiveSensing(string deviceId, int group)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiCableEvents(string deviceId, int group, int byte1, int byte2, int byte3)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiChannelAftertouch(string deviceId, int group, int channel, int pressure)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiContinue(string deviceId, int group)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiControlChange(string deviceId, int group, int channel, int function, int value)
        {
            throw new System.NotImplementedException();
        }

        // #1
        public void OnMidiInputDeviceAttached(string deviceId)
        {
            receivedMidiMessages.Add($"MIDI Input device attached. deviceId: {deviceId}, name: {MidiManager.Instance.GetDeviceName(deviceId)}, vendor: {MidiManager.Instance.GetVendorId(deviceId)}, product: {MidiManager.Instance.GetProductId(deviceId)}");

            Report();
        }

        public void OnMidiInputDeviceDetached(string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiMiscellaneousFunctionCodes(string deviceId, int group, int byte1, int byte2, int byte3)
        {
            throw new System.NotImplementedException();
        }

        // #4
        public void OnMidiNoteOff(string deviceId, int group, int channel, int note, int velocity)
        {
            receivedMidiMessages.Add($"OnMidiNoteOff from: {deviceId}, channel: {channel}, note: {note}, velocity: {velocity}");

            Report();
        }

        // #3
        public void OnMidiNoteOn(string deviceId, int group, int channel, int note, int velocity)
        {
            receivedMidiMessages.Add($"OnMidiNoteOn from: {deviceId}, channel: {channel}, note: {note}, velocity: {velocity}");

            onNoteOn?.Invoke(note);

            Report();
        }

        // #2
        public void OnMidiOutputDeviceAttached(string deviceId)
        {
            receivedMidiMessages.Add($"MIDI Output device attached. deviceId: {deviceId}, name: {MidiManager.Instance.GetDeviceName(deviceId)}, vendor: {MidiManager.Instance.GetVendorId(deviceId)}, product: {MidiManager.Instance.GetProductId(deviceId)}");

            Report();
        }

        public void OnMidiOutputDeviceDetached(string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiPitchWheel(string deviceId, int group, int channel, int amount)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiPolyphonicAftertouch(string deviceId, int group, int channel, int note, int pressure)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiProgramChange(string deviceId, int group, int channel, int program)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiReset(string deviceId, int group)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiSingleByte(string deviceId, int group, int byte1)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiSongPositionPointer(string deviceId, int group, int position)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiSongSelect(string deviceId, int group, int song)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiStart(string deviceId, int group)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiStop(string deviceId, int group)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiSystemCommonMessage(string deviceId, int group, byte[] message)
        {
            throw new System.NotImplementedException();
        }

        // #5
        public void OnMidiSystemExclusive(string deviceId, int group, byte[] systemExclusive)
        {
            receivedMidiMessages.Add($"OnMidiSystemExclusive from: {deviceId}, systemExclusive: {BitConverter.ToString(systemExclusive).Replace("-", " ")}");

            Report();
        }

        public void OnMidiTimeCodeQuarterFrame(string deviceId, int group, int timing)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiTimingClock(string deviceId, int group)
        {
            throw new System.NotImplementedException();
        }

        public void OnMidiTuneRequest(string deviceId, int group)
        {
            throw new System.NotImplementedException();
        }

        protected static new string 名 => "[<color=violet>MIDI長</color>]";
    }
}