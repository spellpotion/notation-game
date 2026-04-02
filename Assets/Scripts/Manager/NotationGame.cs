using spellpotion.Manager;
using spellpotion.midiTutor.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace spellpotion.midiTutor.Manager
{
    public class NotationGame : 抽象Manager<NotationGame, Config.NotationGame>
    {
        private const float durationMax = 6f;
        private const float durationMin = 1.2f;
        private const float durationRange = durationMax - durationMin;
        private const int baseScore = 100;
        
        // endless mode
        private const float streakHalfLife = 2f;
        private const int unlockCount = 10;

        #region Events


        public static ActionEvent<(NoteName noteName, float duration)> OnQuestion = new(out onQuestion);
        private static Action<(NoteName, float)> onQuestion;

        public static ActionEvent<KeyName> OnAnswer = new(out onAnswer);
        private static Action<KeyName> onAnswer;

        public static ActionEvent<Result> OnResult = new(out onResult);
        private static Action<Result> onResult;


        #endregion Events
        #region PublicStatic


        public static void SetNotationRange(NotationRange range)
            => InstanceRun(x => x.SetNotationRange_Instance(range));

        public static NotationRange NotationRange
            => InstanceRun(x => x.notationRange);


        #endregion PublicStatic
        #region Listeners


        protected override void OnEnable()
        {
            base.OnEnable();

            if (Config.AutoAnswer) OnQuestion.AddListener(AutoAnswer);
            Midi.OnNoteOn.AddListener(OnNoteOn);
        }

        protected override void OnDisable()
        {
            StopAllCoroutines();

            Midi.OnNoteOn.RemoveListener(OnNoteOn);
            if (Config.AutoAnswer) OnQuestion.RemoveListener(AutoAnswer);

            base.OnDisable();
        }

        private void OnNoteOn((int noteNumber, int velocity) noteOn)
        {
            if (lockAnswer) return;

            var keyName = noteOn.noteNumber - Midi.Offset;

            if (keyName < 1 || keyName > 54) return;

            answer = (KeyName)keyName;

            Debug.Log($"{名} received <i>{answer}</i>");
            onAnswer?.Invoke(answer);
        }

        private IEnumerator OnQuestion務((NoteName noteName, float duration) question)
        {
            var hesitate = Random.Range(0f, question.duration);

            yield return new WaitForSeconds(hesitate);

            var velocity = Random.Range(0, 127);

            if (Random.value <= Config.AutoAnswerSuccessRate)
            {
                var keyName = Conversion.NoteNameToKeyName(question.noteName);
                var noteNumber = (int)keyName + Midi.Offset;

                Midi.NoteOn((noteNumber, velocity));
            }
            else
            {
                var noteNumberRandom = (Random.Range(21, 109));

                Midi.NoteOn((noteNumberRandom, velocity));
            }

            yield break;
        }


        #endregion Listeners
        #region Common

        private NotationRange notationRange = NotationRange.None;

        private NoteName noteName現;
        private KeyName answer;

        private bool lockAnswer;

        private void SetNotationRange_Instance(NotationRange range)
        {
            this.notationRange = range;

            if (Config.GameMode == GameMode.Demo)
            {
                StartCoroutine(Demo務());
            }
            else if (Config.GameMode == GameMode.Endless)
            {
                StartCoroutine(Endless務());
            }

            if (Config.AutoAnswer)
            {
                Debug.Log($"{名} <color=#52cc33><b>autoANSWER</b></color>");
            }
        }

        private void AutoAnswer((NoteName noteName, float duration) question)
            => StartCoroutine(OnQuestion務(question));

        private Result EvaluateAnswer()
        {
            if (answer == KeyName.Unknown)
            {
                return Result.NoAnswer;
            }

            var question = Conversion.NoteNameToKeyName(noteName現);

            if (question == answer)
            {
                return Result.Correct;
            }

            if (IsSamePitchClass(question, answer))
            {
                return Result.PartiallyCorrect;
            }

            return Result.Incorrect;
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

        private static string ResultColor(Result result) => result switch
        {
            Result.NoAnswer => Color.magenta.ToHexString(),
            Result.Correct => Color.springGreen.ToHexString(),
            Result.PartiallyCorrect => Color.orange.ToHexString(),
            Result.Incorrect => Color.softRed.ToHexString(),
            _ => string.Empty,
        };


        #endregion Common
        #region Demo


        private IEnumerator Demo務()
        {
            var noteNames = (NoteName[])Enum.GetValues(typeof(NoteName));
            var (minInclusive, maxEclusive) = RangeToIndices(notationRange);

            while (true)
            {
                noteName現 = noteNames[Random.Range(minInclusive, maxEclusive)];
                var duration = Random.Range(durationMin, durationMax);

                Debug.Log($"{名} Question <color=white><b>{noteName現}</b></color>");
                onQuestion?.Invoke((noteName現, duration));

                yield return new WaitForSeconds(duration);

                lockAnswer = true;

                var result = EvaluateAnswer();

                Debug.Log($"{名} Answer <i>{answer}</i> is <color=" +
                    $"#{ResultColor(result)}><i>{result}</i></color>");
                yield return SyncEvent務(onResult, result);

                if (result == Result.Correct)
                {
                    Score.Add((int)(baseScore * (durationMax / duration)));
                }

                answer = KeyName.Unknown;
                lockAnswer = false;
            }
        }


        #endregion Demo
        #region EndlessMode


        private readonly Queue<NoteName> unlockOrder = new();
        private readonly Dictionary<NoteName, (int countTotal, int countSuccess)> mastery = new();

        private int streakCount = 0;

        private IEnumerator Endless務()
        {
            var all = (NoteName[])Enum.GetValues(typeof(NoteName));
            var (minInclusive, maxEclusive) = RangeToIndices(notationRange);
            var range = all.Skip(minInclusive).Take(maxEclusive - minInclusive).ToArray();

            BuildUnlockOrder(range);

            for (var i = 0; i < 3; i++)
            {
                mastery.Add(Unlock(), (0, 0));
            }

            while (true)
            {
                var ordered = mastery.OrderBy(x => x.Value.countSuccess).ToList();
                var index = Utils.RandomMin(ordered.Count, 2);
                var elementAt = ordered.ElementAt(index);
                noteName現 = elementAt.Key;
                var (countTotal, countSuccess) = elementAt.Value;

                var duration率 = streakHalfLife / (streakCount + streakHalfLife);
                var duration = durationMin + durationRange * duration率;

                Debug.Log($"{名} Question <color=white>{noteName現}</color>");
                onQuestion?.Invoke((noteName現, duration));

                yield return new WaitForSeconds(duration);

                lockAnswer = true;

                var result = EvaluateAnswer();

                Debug.Log($"{名} Answer <i>{answer}</i> is <color=" +
                    $"#{ResultColor(result)}><i>{result}</i></color>");
                yield return SyncEvent務(onResult, result);

                if (result == Result.Correct)
                {
                    Score.Add((int)(baseScore * (durationMax / duration)));

                    countSuccess++;
                    streakCount++;

                    if (streakCount % unlockCount == 0 && streakCount > 0 && unlockOrder.Count > 0)
                    {
                        mastery.Add(Unlock(), (0, 0));
                    }
                }
                else if (result == Result.Incorrect || result == Result.NoAnswer)
                {
                    streakCount = 0;
                }

                mastery[noteName現] = (++countTotal, countSuccess);

                answer = KeyName.Unknown;
                lockAnswer = false;
            }


            NoteName Unlock()
            {
                var unlock = unlockOrder.Dequeue();
                Debug.Log($"{名} Unlocked {unlock} 🔓");

                return unlock;
            }
        }

        private void BuildUnlockOrder(NoteName[] range)
        {
            var center = range.Length / 2;

            var unlockGroups = range
                .GroupBy(noteName => Conversion.NoteNameToAccidental(noteName))
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (Accidental accidental in Enum.GetValues(typeof(Accidental)))
            {
                var unlockGroup = unlockGroups[accidental];

                while (unlockGroup.Count > 0)
                {
                    var picked = PickWeightedIndex(unlockGroup, center);

                    unlockOrder.Enqueue(unlockGroup[picked]);
                    unlockGroup.RemoveAt(picked);
                }
            }
        }

        private static int PickWeightedIndex(List<NoteName> candidates, int center)
        {
            int distanceMax = int.MinValue;
            for (var i = 0; i < candidates.Count; i++)
            {
                var distance = Math.Abs((int)candidates[i] - center);
                if (distance > distanceMax) distanceMax = distance;
            }

            int weightTotal = 0;
            var cumulative = new int[candidates.Count];

            for (var i = 0; i < candidates.Count; i++)
            {
                var distance = Math.Abs((int)candidates[i] - center);
                var weight = (distanceMax - distance) + 1;

                weightTotal += weight;
                cumulative[i] = weightTotal;
            }

            var weightRandom = Random.Range(1, weightTotal + 1);
            for (var i = 0; i < cumulative.Length; i++)
            {
                if (weightRandom <= cumulative[i])
                {
                    return i;
                }
            }

            return candidates.Count() - 1;
        }


        #endregion EndlessMode
    }
}