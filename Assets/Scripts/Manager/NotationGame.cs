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


        public static ActionEvent<(NoteName noteName, float duration)> OnQuestion = new(out onQuestion);
        private static Action<(NoteName, float)> onQuestion;

        public static ActionEvent<KeyName> OnAnswer = new(out onAnswer);
        private static Action<KeyName> onAnswer;

        public static ActionEvent<Result> OnResult = new(out onResult);
        private static Action<Result> onResult;


        #endregion Events
        #region PublicStatic


        public static void Answer(KeyName n) => InstanceRun(x => x.Answer_Instance(n));
        public static NotationRange NotationRange => InstanceRun(x => x.Config.NotationRange);


        #endregion PublicStatic

        private NoteName noteName現;
        private KeyName answer;

        private bool lockAnswer;

        protected void Start()
        {
            StartCoroutine(Demo務());
        }

        private IEnumerator Demo務()
        {
            var noteNames = (NoteName[])Enum.GetValues(typeof(NoteName));
            var (minInclusive, maxEclusive) = RangeToIndices(Config.NotationRange);

            var duration = 6f;

            while (true)
            {
                noteName現 = noteNames[Random.Range(minInclusive, maxEclusive)];
                Debug.Log($"{名} Question <color=white><i>{noteName現}</i></color>");
                onQuestion?.Invoke((noteName現, duration));

                yield return new WaitForSeconds(duration);

                lockAnswer = true;

                var result = Evaluate();

                Debug.Log($"{名} Result <i>{result}</i>");
                yield return SyncEvent務(onResult, result);

                answer = KeyName.Unknown;
                lockAnswer = false;
            }

            Result Evaluate()
            {
                if (answer == KeyName.Unknown)
                    return Result.NoAnswer;

                var question = Conversion.NoteNameToKeyName(noteName現);

                if (question == answer)
                    return Result.Correct;

                if (IsSamePitchClass(question, answer))
                    return Result.Partial;

                return Result.Incorrect;
            }
        }

        private void Answer_Instance(KeyName keyName)
        {
            if (lockAnswer) return;

            answer = keyName;

            Debug.Log($"{名} Answer <i>{keyName}</i>");
            onAnswer?.Invoke(keyName);
        }

        private static bool IsSamePitchClass(KeyName key1, KeyName key2)
        {
            int index1 = ((int)key1 - (int)KeyName.A1B1) % 12;
            int index2 = ((int)key2 - (int)KeyName.A1B1) % 12;

            return index1 == index2;
        }

        private static (int minInclusive, int maxEclusive) RangeToIndices(NotationRange range) => range switch
        { 
            NotationRange.Bass => (2, 47), // B1b - F4#
            NotationRange.Treble => (30, 77), // G3b - D6#
            NotationRange.Alto => (16, 62), // A2b - E5
            NotationRange.Tenor => (11, 57), // F2 - C6#
            _ => (0, 0)
        };
    }
}