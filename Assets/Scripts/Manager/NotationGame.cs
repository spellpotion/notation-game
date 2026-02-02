using spellpotion.midiTutor.Data;
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

        public static ActionEvent<bool> OnResult = new(out onResult);
        private static Action<bool> onResult;

        public static ActionEvent<KeyName> OnAnswer = new(out onAnswer);
        private static Action<KeyName> onAnswer;

        #endregion Events
        #region PublicStatic


        public static NotationRange NotationRange => InstanceRun(x => x.Config.NotationRange);

        public static void Answer(KeyName n) => InstanceRun(x => x.Answer_Instance(n));


        #endregion PublicStatic

        private KeyName? query現;
        private KeyName? answer;

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
                var query = noteNames[Random.Range(range.minInclusive, range.maxEclusive)];

                query現 = Conversion.NoteNameToKeyName(query);
                
                Debug.Log($"{名} Query {query} ({query現})");
                onQuery?.Invoke((query, duration));

                yield return new WaitForSeconds(duration);

                if (!answer.HasValue)
                {
                    Debug.Log($"{名} <b>NO</b> Answer");
                    onResult?.Invoke(false);
                }
                else
                {
                    var success = query現.Value == answer.Value;
                    Debug.Log($"{名} {(success ? "✔️" : "❌")} Answer");
                    onResult?.Invoke(success);
                }

                query現 = null;
                answer = null;
            }
        }

        private void Answer_Instance(KeyName keyName)
        {
            if (!query現.HasValue) return;

            Debug.Log($"{名} Answer {keyName}");
            answer = keyName;

            onAnswer?.Invoke(keyName);
        }

        private static (int minInclusive, int maxEclusive) RangeToIndices(NotationRange range) => range switch
        { 
            NotationRange.Bass => (1, 46), // B1b - F4#
            NotationRange.Treble => (29, 76), // G3b - D6#
            NotationRange.Alto => (15, 61), // A2b - E5
            NotationRange.Tenor => (10, 56), // F2 - C6#
            _ => (0, 0)
        };
    }
}