using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace spellpotion.midiTutor.Manager
{
    public class NotationGame : 抽象Manager<NotationGame, Config.NotationGame>
    {
        #region Events


        public static ActionEvent<(NoteName noteName, float duration)> OnQuery = new(out onQuery);
        private static Action<(NoteName, float)> onQuery;

        public static ActionEvent<bool> OnAnswer = new(out onAnswer);
        private static Action<bool> onAnswer;


        #endregion Events
        #region PublicStatic


        public static NotationRange NotationRange => InstanceRun(x => x.Config.NotationRange);

        public static void Answer(KeyName n) => InstanceRun(x => x.Answer_Instance(n));


        #endregion PublicStatic

        private NoteName? query現;

        protected void Start()
        {
            StartCoroutine(Demo務());
        }

        private IEnumerator Demo務()
        {
            var noteNames = (NoteName[])Enum.GetValues(typeof(NoteName));
            var range = RangeToIndices(Config.NotationRange);

            var duration = 6f;

            while (true)
            {
                query現 = noteNames[Random.Range(range.minInclusive, range.maxEclusive)];

                Debug.Log($"DBG Query {query現}");
                onQuery?.Invoke((query現.Value, duration));

                yield return Utils.WaitForSecondsOrWhile(duration, () => query現.HasValue);
                
                if (query現.HasValue) onAnswer?.Invoke(false);
            }
        }

        private void Answer_Instance(KeyName keyName)
        {
            if (!query現.HasValue) return;

            Debug.Log($"DBG Answer {keyName}");
            onAnswer?.Invoke(NoteNameToKeyName(query現.Value) == keyName);

            query現 = null;
        }

        private static (int minInclusive, int maxEclusive) RangeToIndices(NotationRange range) => range switch
        { 
            NotationRange.Bass => (1, 46), // B1b - F4#
            NotationRange.Treble => (29, 76), // G3b - D6#
            NotationRange.Alto => (15, 61), // A2b - E5
            NotationRange.Tenor => (10, 56), // F2 - C6#
            _ => (0, 0)
        };

        private static KeyName NoteNameToKeyName(NoteName noteName) => noteName switch
        {
            NoteName.BF1 => KeyName.A1B1,
            NoteName.B1 => KeyName.B1,
            NoteName.C2 => KeyName.C2,
            NoteName.CS2 => KeyName.C2D2,
            NoteName.DF2 => KeyName.C2D2,
            NoteName.D2 => KeyName.D2,
            NoteName.DS2 => KeyName.D2E2,
            NoteName.EF2 => KeyName.D2E2,
            NoteName.E2 => KeyName.E2,
            NoteName.F2 => KeyName.F2,
            NoteName.FS2 => KeyName.F2G2,
            NoteName.GF2 => KeyName.F2G2,
            NoteName.G2 => KeyName.G2,
            NoteName.GS2 => KeyName.G2A2,
            NoteName.AF2 => KeyName.G2A2,
            NoteName.A2 => KeyName.A2,
            NoteName.AS2 => KeyName.A2B2,
            NoteName.BF2 => KeyName.A2B2,
            NoteName.B2 => KeyName.B2,
            NoteName.C3 => KeyName.C3,
            NoteName.CS3 => KeyName.C3D3,
            NoteName.DF3 => KeyName.C3D3,
            NoteName.D3 => KeyName.D3,
            NoteName.DS3 => KeyName.D3E3,
            NoteName.EF3 => KeyName.D3E3,
            NoteName.E3 => KeyName.E3,
            NoteName.F3 => KeyName.F3,
            NoteName.FS3 => KeyName.F3G3,
            NoteName.GF3 => KeyName.F3G3,
            NoteName.G3 => KeyName.G3,
            NoteName.GS3 => KeyName.G3A3,
            NoteName.AF3 => KeyName.G3A3,
            NoteName.A3 => KeyName.A3,
            NoteName.AS3 => KeyName.A3B3,
            NoteName.BF3 => KeyName.A3B3,
            NoteName.B3 => KeyName.B3,
            NoteName.C4 => KeyName.C4,
            NoteName.CS4 => KeyName.C4D4,
            NoteName.DF4 => KeyName.C4D4,
            NoteName.D4 => KeyName.D4,
            NoteName.DS4 => KeyName.D4E4,
            NoteName.EF4 => KeyName.D4E4,
            NoteName.E4 => KeyName.E4,
            NoteName.F4 => KeyName.F4,
            NoteName.FS4 => KeyName.F4G4,
            NoteName.GF4 => KeyName.F4G4,
            NoteName.G4 => KeyName.G4,
            NoteName.GS4 => KeyName.G4A4,
            NoteName.AF4 => KeyName.G4A4,
            NoteName.A4 => KeyName.A4,
            NoteName.AS4 => KeyName.A4B4,
            NoteName.BF4 => KeyName.A4B4,
            NoteName.B4 => KeyName.B4,
            NoteName.C5 => KeyName.C5,
            NoteName.CS5 => KeyName.C5D5,
            NoteName.DF5 => KeyName.C5D5,
            NoteName.D5 => KeyName.D5,
            NoteName.DS5 => KeyName.D5E5,
            NoteName.EF5 => KeyName.D5E5,
            NoteName.E5 => KeyName.E5,
            NoteName.F5 => KeyName.F5,
            NoteName.FS5 => KeyName.F5G5,
            NoteName.GF5 => KeyName.F5G5,
            NoteName.G5 => KeyName.G5,
            NoteName.GS5 => KeyName.G5A5,
            NoteName.AF5 => KeyName.G5A5,
            NoteName.A5 => KeyName.A5,
            NoteName.AS5 => KeyName.A5B5,
            NoteName.BF5 => KeyName.A5B5,
            NoteName.B5 => KeyName.B5,
            NoteName.C6 => KeyName.C6,
            NoteName.CS6 => KeyName.C6D6,
            NoteName.DF6 => KeyName.C6D6,
            NoteName.D6 => KeyName.D6,
            NoteName.DS6 => KeyName.D6E6,
            _ => KeyName.Unknown
        };
    }
}