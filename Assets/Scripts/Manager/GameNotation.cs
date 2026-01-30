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

        public static void Answer(NoteName n) => InstanceRun(x => x.Answer_Instance(n));


        #endregion PublicStatic

        private NoteName? query現;

        protected void Start()
        {
            StartCoroutine(Demo務());
        }

        private IEnumerator Demo務()
        {
            var noteNames = (NoteName[])Enum.GetValues(typeof(NoteName));

            var duration = 4f;

            while (true)
            {
                query現 = noteNames[Random.Range(1, noteNames.Length)];

                onQuery?.Invoke((query現.Value, duration));

                yield return Utils.WaitForSecondsOrWhile(duration, () => query現.HasValue);
                
                if (query現.HasValue) onAnswer?.Invoke(false);
            }
        }

        private void Answer_Instance(NoteName noteName)
        {
            onAnswer?.Invoke(query現.Value == noteName);

            query現 = null;
        }
    }
}