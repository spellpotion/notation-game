using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace spellpotion.midiTutor.Manager
{
    public class GameNotation : 抽象Manager<GameNotation, Config.GameNotation>
    {
        #region Events


        public static ActionEvent<(NoteName noteName, float duration)> OnQuery = new(out onQuery);
        private static Action<(NoteName, float)> onQuery;

        public static ActionEvent<bool> OnAnswer = new(out onAnswer);
        private static Action<bool> onAnswer;


        #endregion Events
        #region PublicStatic


        public static NotationType NotationType => InstanceRun(x => x.Config.NotationType);

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

            var duration = 6f;

            while (true)
            {
                query現 = noteNames[Random.Range(1, noteNames.Length)];

                onQuery?.Invoke((query現.Value, duration));

                yield return Utils.WaitForSecondsOrWhile(duration, () => query現.HasValue);
                
                if (query現.HasValue) onAnswer?.Invoke(false);
            }
        }

        private void Answer_Instance(KeyName keyName)
        {
            if (!query現.HasValue) return;

            onAnswer?.Invoke(NoteNameToKeyName(query現.Value) == keyName);

            query現 = null;
        }

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
            _ => KeyName.Unknown
        };
    }
}