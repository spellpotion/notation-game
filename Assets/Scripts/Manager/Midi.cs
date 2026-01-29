using System;
using System.Collections.Generic;
using UnityEngine;

namespace spellpotion.midiTutor.Manager
{
    public class Midi : 抽象Manager<Midi, Config.Midi>
    {
        #region Events


        public static ActionEvent<int> OnNoteOn = new(out onNoteOn);
        private static Action<int> onNoteOn;


        #endregion Events

        protected static new string 名 => "[<color=violet>MIDI長</color>]";
    }
}