using spellpotion.midiTutor.Data;
using spellpotion.midiTutor.Manager;
using spellpotion.Screen;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Time = UnityEngine.Time;

namespace spellpotion.midiTutor.Screen
{
    public class OneNote : 抽象Screen
    {
        private const float differenceX = 1450f;

        private const float durationPulseOn = .2f;
        private const float durationPulseOff = .4f;

        private readonly Color colorZone原 = Color.gray6;
        private readonly Color colorName原 = Color.lightSteelBlue;
        private readonly Color colorAnswerIncorrect = Color.softRed;
        private readonly Color colorAnswerCorrect = Color.springGreen;
        private readonly Color colorAnswerPartial = Color.sandyBrown;

        private VisualElement root;
        private VisualElement note;
        private VisualElement flat;
        private VisualElement ledger1;
        private VisualElement ledger2;
        private VisualElement sharp;
        private VisualElement greyZone;
        private VisualElement noteNameContainer;
        private Label labelNoteName;
        private VisualElement selectContainer;

        private Label score;
        private Button[] keys;
        private Button buttonPause;

        private bool IsPause => buttonPause.ClassListContains("dark-button-pressed");

        private Coroutine releaseNote務;
        private Coroutine showResult務;

        private string question;
        private Accidental accidental;

        private NotationRange Range => NotationGame.NotationRange;

        protected override void OnEnable()
        {
            NotationGame.OnQuestion.AddListener(OnQuestion);
            NotationGame.OnAnswer.AddListener(OnAnswer);
            NotationGame.OnResult.AddListener(OnResult);
            Score.OnUpdateScore.AddListener(OnUpdateScore);
        }

        protected void OnDisable()
        {
            Score.OnUpdateScore.RemoveListener(OnUpdateScore);
            NotationGame.OnResult.RemoveListener(OnResult);
            NotationGame.OnAnswer.RemoveListener(OnAnswer);
            NotationGame.OnQuestion.RemoveListener(OnQuestion);
        }

        private void OnQuestion((NoteName noteName, float duration) question)
        {
            // accidental

            this.question = Conversion.NoteNameToString(question.noteName);
            accidental = Conversion.NoteNameToAccidental(question.noteName);

            flat.style.display = accidental == Accidental.Flat ? DisplayStyle.Flex : DisplayStyle.None;
            sharp.style.display = accidental == Accidental.Sharp ? DisplayStyle.Flex : DisplayStyle.None;

            // ledger line

            var lineNote = NoteNameToLineNote();
            var (offset1, offset2) = LineNoteToLedgerOffset(lineNote);

            if (offset1.HasValue)
            {
                ledger1.style.display = DisplayStyle.Flex;
                ledger1.style.translate = new Translate(0, offset1.Value);
            }
            else ledger1.style.display = DisplayStyle.None;

            if (offset2.HasValue)
            {
                ledger2.style.display = DisplayStyle.Flex;
                ledger2.style.translate = new Translate(0, offset2.Value);
            }
            else ledger2.style.display = DisplayStyle.None;

            // release note

            SetNull(ref releaseNote務);
            releaseNote務 = StartCoroutine(ReleaseNote務(lineNote, question.duration));
            

            LineNote NoteNameToLineNote() => NotationGame.NotationRange switch
            {
                NotationRange.Bass => NoteNameToLineNoteBass(question.noteName),
                NotationRange.Treble => NoteNameToLineNoteTreble(question.noteName),
                NotationRange.Alto => NoteNameToLineNoteAlto(question.noteName),
                NotationRange.Tenor => NoteNameToLineNoteTenor(question.noteName),
                _ => LineNote.Unknown
            };
        }

        private void OnAnswer(KeyName keyName)
        {
            var noteName = accidental == Accidental.Flat ?
                Conversion.KeyNameToNoteNameFlat(keyName) :
                Conversion.KeyNameToNoteNameSharp(keyName);
            var noteString = Conversion.NoteNameToString(noteName);

            if (!string.IsNullOrEmpty(noteString))
            {
                labelNoteName.text = noteString[..^1];
            }

            RevertKeysColor();

            var index = (int)keyName - GetKeyOffset();

            if (index >= 0 && index < keys.Length)
            {
                keys[index].AddToClassList("pressed");
            }
        }

        private void OnResult(Result result)
        {
            RevertKeysColor();

            SetNull(ref showResult務);
            showResult務 = StartCoroutine(ShowResult務(result));

            NotationGame.WaitUntil(() => showResult務 == null);
        }

        private void OnUpdateScore(int value)
        {
            score.text = value.ToString();
        }

        protected void Awake()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            note = root.Q<VisualElement>("note");
            note.style.display = DisplayStyle.None;

            flat = root.Q<VisualElement>("flat");
            flat.style.display = DisplayStyle.None;

            sharp = root.Q<VisualElement>("sharp");
            sharp.style.display = DisplayStyle.None;

            ledger1 = root.Q<VisualElement>("ledger1");
            ledger1.style.display = DisplayStyle.None;

            ledger2 = root.Q<VisualElement>("ledger2");
            ledger2.style.display = DisplayStyle.None;

            greyZone = root.Q<VisualElement>("staff-occlusion-container");

            labelNoteName = root.Q<Label>("note-name-label");
            labelNoteName.text = string.Empty;

            noteNameContainer = root.Q<VisualElement>("note-name-container");

            score = root.Q<Label>("score");
            score.text = "0";

            buttonPause = root.Q<Button>("button-pause");
            buttonPause.clicked += () =>
            {
                buttonPause.ToggleInClassList("dark-button-pressed");

                spellpotion.Manager.Time.SetPause(IsPause);

                UpdateNoteDisplayStyle();
            };
            buttonPause.style.display = DisplayStyle.None;
        }

        protected void Start()
        {
            selectContainer = root.Q<VisualElement>("select-container");

            if (Range == NotationRange.None)
            {
                noteNameContainer.style.display = DisplayStyle.None;
                selectContainer.style.display = DisplayStyle.Flex;

                var selectBass = root.Q<Button>("select-bass");
                selectBass.clicked += () => 
                {
                    NotationGame.SetNotationRange(NotationRange.Bass);

                    InitializeKeyboard();
                };

                var selectTenor = root.Q<Button>("select-tenor");
                selectTenor.clicked += () =>
                {
                    NotationGame.SetNotationRange(NotationRange.Tenor);

                    InitializeKeyboard();
                };

                var selectAlto = root.Q<Button>("select-alto");
                selectAlto.clicked += () =>
                {
                    NotationGame.SetNotationRange(NotationRange.Alto);

                    InitializeKeyboard();
                };

                var selectTreble = root.Q<Button>("select-treble");
                selectTreble.clicked += () =>
                {
                    NotationGame.SetNotationRange(NotationRange.Treble);

                    InitializeKeyboard();
                };
            }
            else
            {
                InitializeKeyboard();
            }
        }

        private void InitializeKeyboard()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Oscillator.ResumeWebAudio();
#endif

            noteNameContainer.style.display = DisplayStyle.Flex;
            selectContainer.style.display = DisplayStyle.None;

            var keyboardBass = root.Q<VisualElement>("keyboard-bass");
            keyboardBass.style.display = Range == NotationRange.Bass ?
                DisplayStyle.Flex : DisplayStyle.None;

            var keyboardTreble = root.Q<VisualElement>("keyboard-treble");
            keyboardTreble.style.display = Range == NotationRange.Treble ?
                DisplayStyle.Flex : DisplayStyle.None;

            var keyboardAlto = root.Q<VisualElement>("keyboard-alto");
            keyboardAlto.style.display = Range == NotationRange.Alto ?
                DisplayStyle.Flex : DisplayStyle.None;

            var keyboardTenor = root.Q<VisualElement>("keyboard-tenor");
            keyboardTenor.style.display = Range == NotationRange.Tenor ?
                DisplayStyle.Flex : DisplayStyle.None;

            var clefF = root.Q<VisualElement>("f-clef");
            clefF.style.display = Range == NotationRange.Bass ?
                DisplayStyle.Flex : DisplayStyle.None;

            var clefG = root.Q<VisualElement>("g-clef");
            clefG.style.display = Range == NotationRange.Treble ?
                DisplayStyle.Flex : DisplayStyle.None;

            var clefA = root.Q<VisualElement>("c-clef-alto");
            clefA.style.display = Range == NotationRange.Alto ?
                DisplayStyle.Flex : DisplayStyle.None;

            var clefT = root.Q<VisualElement>("c-clef-tenor");
            clefT.style.display = Range == NotationRange.Tenor ?
                DisplayStyle.Flex : DisplayStyle.None;

            var keyOffset = GetKeyOffset();
            var keyNamesAll = (KeyName[])Enum.GetValues(typeof(KeyName));
            var rangeName = Enum.GetName(typeof(NotationRange), Range).ToLower();

            keys = new Button[NotationRangeToButtonCount(Range)];

            for (var i = 0; i < keys.Length; i++)
            {
                var buttonName = $"key-{rangeName}-{i}";
                var index = keyOffset + i;

                var button = root.Q<Button>(buttonName);

                button.clicked += () =>
                {
                    Midi.NoteOn((index + Midi.Offset, 0));
                };

                keys[i] = button;
            }

            buttonPause.style.display = DisplayStyle.Flex;
        }

        private bool noteReleased = false;

        private void UpdateNoteDisplayStyle() 
            => note.style.display = !IsPause && noteReleased ? DisplayStyle.Flex : DisplayStyle.None;

        private IEnumerator ReleaseNote務(LineNote lineNote, float duration)
        {
            var time始 = Time.time;
            var time的 = time始 + duration;

            noteReleased = true; UpdateNoteDisplayStyle();
            note.style.top = new Length(LineNoteToTopOffset(lineNote), LengthUnit.Percent);

            while (Time.time < time的)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / duration);

                var offsetX = Percent((1f - progress) * differenceX);
                note.style.translate = new Translate(offsetX, 0f, 0f);

                yield return null;
            }

            noteReleased = false; UpdateNoteDisplayStyle();

            releaseNote務 = null;
        }

        private IEnumerator ShowResult務(Result result)
        {
            var time始 = Time.time;
            var time的 = time始 + durationPulseOn;

            var color的 = result switch
            {
                Result.Correct => colorAnswerCorrect,
                Result.PartiallyCorrect => colorAnswerPartial,
                _ => colorAnswerIncorrect
            };

            while (Time.time < time的)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / durationPulseOn);

                greyZone.style.backgroundColor
                    = Utils.Lerp(colorZone原, color的, progress);
                noteNameContainer.style.backgroundColor
                    = Utils.Lerp(colorName原, color的, progress);

                yield return null;
            }

            greyZone.style.backgroundColor = color的;
            noteNameContainer.style.backgroundColor = color的;

            time始 = Time.time;
            time的 = time始 + durationPulseOff;

            while (Time.time < time的)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / durationPulseOff);

                greyZone.style.backgroundColor = Utils.Lerp(color的, colorZone原, progress);
                noteNameContainer.style.backgroundColor = Utils.Lerp(color的, colorName原, progress);
                labelNoteName.style.opacity = (1f - progress);

                yield return null;
            }

            greyZone.style.backgroundColor = colorZone原;
            noteNameContainer.style.backgroundColor = colorName原;
            labelNoteName.text = string.Empty;
            labelNoteName.style.opacity = 1f;

            if (result == Result.Correct)
            {
                showResult務 = null;

                yield break;
            }

            time始 = Time.time;
            time的 = time始 + durationPulseOn;

            labelNoteName.text = result == Result.PartiallyCorrect ? question : question[..^1];

            while (Time.time < time的)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / durationPulseOn);

                labelNoteName.style.opacity = progress;

                yield return null;
            }

            time始 = Time.time;
            time的 = time始 + durationPulseOff;

            while (Time.time < time的)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / durationPulseOff);

                labelNoteName.style.opacity = (1f - progress);

                yield return null;
            }

            labelNoteName.text = string.Empty;
            labelNoteName.style.opacity = 1f;

            showResult務 = null;
        }

        private void RevertKeysColor()
        {
            foreach (var key in keys)
            {
                key.RemoveFromClassList("pressed");
            }
        }

        private static LineNote MidiNoteToLineNote(int midiNote) => midiNote switch
        {
            47 => LineNote.B1,
            48 => LineNote.C2,
            50 => LineNote.D2,
            52 => LineNote.E2,
            53 => LineNote.F2,
            55 => LineNote.G2,
            57 => LineNote.A2,
            59 => LineNote.B2,
            60 => LineNote.C3,
            62 => LineNote.D3,
            64 => LineNote.E3,
            65 => LineNote.F3,
            67 => LineNote.G3,
            69 => LineNote.A3,
            71 => LineNote.B3,
            72 => LineNote.C4,
            74 => LineNote.D4,
            76 => LineNote.E4,
            77 => LineNote.F4,
            _ => LineNote.Unknown
        };

        private static LineNote NoteNameToLineNoteBass(NoteName noteName) => noteName switch
        {
            NoteName.BF1 => LineNote.B1,
            NoteName.B1 => LineNote.B1,
            NoteName.C2 => LineNote.C2,
            NoteName.CS2 => LineNote.C2,
            NoteName.DF2 => LineNote.D2,
            NoteName.D2 => LineNote.D2,
            NoteName.DS2 => LineNote.D2,
            NoteName.EF2 => LineNote.E2,
            NoteName.E2 => LineNote.E2,
            NoteName.F2 => LineNote.F2,
            NoteName.FS2 => LineNote.F2,
            NoteName.GF2 => LineNote.G2,
            NoteName.G2 => LineNote.G2,
            NoteName.GS2 => LineNote.G2,
            NoteName.AF2 => LineNote.A2,
            NoteName.A2 => LineNote.A2,
            NoteName.AS2 => LineNote.A2,
            NoteName.BF2 => LineNote.B2,
            NoteName.B2 => LineNote.B2,
            NoteName.C3 => LineNote.C3,
            NoteName.CS3 => LineNote.C3,
            NoteName.DF3 => LineNote.D3,
            NoteName.D3 => LineNote.D3,
            NoteName.DS3 => LineNote.D3,
            NoteName.EF3 => LineNote.E3,
            NoteName.E3 => LineNote.E3,
            NoteName.F3 => LineNote.F3,
            NoteName.FS3 => LineNote.F3,
            NoteName.GF3 => LineNote.G3,
            NoteName.G3 => LineNote.G3,
            NoteName.GS3 => LineNote.G3,
            NoteName.AF3 => LineNote.A3,
            NoteName.A3 => LineNote.A3,
            NoteName.AS3 => LineNote.A3,
            NoteName.BF3 => LineNote.B3,
            NoteName.B3 => LineNote.B3,
            NoteName.C4 => LineNote.C4,
            NoteName.CS4 => LineNote.C4,
            NoteName.DF4 => LineNote.D4,
            NoteName.D4 => LineNote.D4,
            NoteName.DS4 => LineNote.D4,
            NoteName.EF4 => LineNote.E4,
            NoteName.E4 => LineNote.E4,
            NoteName.F4 => LineNote.F4,
            NoteName.FS4 => LineNote.F4,
            _ => LineNote.Unknown
        };

        private static LineNote NoteNameToLineNoteTreble(NoteName noteName) => noteName switch
        {
            NoteName.GF3 => LineNote.B1,
            NoteName.G3 => LineNote.B1,
            NoteName.GS3 => LineNote.B1,
            NoteName.AF3 => LineNote.C2,
            NoteName.A3 => LineNote.C2,
            NoteName.AS3 => LineNote.C2,
            NoteName.BF3 => LineNote.D2,
            NoteName.B3 => LineNote.D2,
            NoteName.C4 => LineNote.E2,
            NoteName.CS4 => LineNote.E2,
            NoteName.DF4 => LineNote.F2,
            NoteName.D4 => LineNote.F2,
            NoteName.DS4 => LineNote.F2,
            NoteName.EF4 => LineNote.G2,
            NoteName.E4 => LineNote.G2,
            NoteName.F4 => LineNote.A2,
            NoteName.FS4 => LineNote.A2,
            NoteName.GF4 => LineNote.B2,
            NoteName.G4 => LineNote.B2,
            NoteName.GS4 => LineNote.B2,
            NoteName.AF4 => LineNote.C3,
            NoteName.A4 => LineNote.C3,
            NoteName.AS4 => LineNote.C3,
            NoteName.BF4 => LineNote.D3,
            NoteName.B4 => LineNote.D3,
            NoteName.C5 => LineNote.E3,
            NoteName.CS5 => LineNote.E3,
            NoteName.DF5 => LineNote.F3,
            NoteName.D5 => LineNote.F3,
            NoteName.DS5 => LineNote.F3,
            NoteName.EF5 => LineNote.G3,
            NoteName.E5 => LineNote.G3,
            NoteName.F5 => LineNote.A3,
            NoteName.FS5 => LineNote.A3,
            NoteName.GF5 => LineNote.B3,
            NoteName.G5 => LineNote.B3,
            NoteName.GS5 => LineNote.B3,
            NoteName.AF5 => LineNote.C4,
            NoteName.A5 => LineNote.C4,
            NoteName.AS5 => LineNote.C4,
            NoteName.BF5 => LineNote.D4,
            NoteName.B5 => LineNote.D4,
            NoteName.C6 => LineNote.E4,
            NoteName.CS6 => LineNote.E4,
            NoteName.DF6 => LineNote.F4,
            NoteName.D6 => LineNote.F4,
            NoteName.DS6 => LineNote.F4,

            _ => LineNote.Unknown
        };

        private static LineNote NoteNameToLineNoteAlto(NoteName noteName) => noteName switch
        {
            NoteName.AF2 => LineNote.B1,
            NoteName.A2 => LineNote.B1,
            NoteName.AS2 => LineNote.B1,
            NoteName.BF2 => LineNote.C2,
            NoteName.B2 => LineNote.C2,
            NoteName.C3 => LineNote.D2,
            NoteName.CS3 => LineNote.D2,
            NoteName.DF3 => LineNote.E2,
            NoteName.D3 => LineNote.E2,
            NoteName.DS3 => LineNote.E2,
            NoteName.EF3 => LineNote.F2,
            NoteName.E3 => LineNote.F2,
            NoteName.F3 => LineNote.G2,
            NoteName.FS3 => LineNote.G2,
            NoteName.GF3 => LineNote.A2,
            NoteName.G3 => LineNote.A2,
            NoteName.GS3 => LineNote.A2,
            NoteName.AF3 => LineNote.B2,
            NoteName.A3 => LineNote.B2,
            NoteName.AS3 => LineNote.B2,
            NoteName.BF3 => LineNote.C3,
            NoteName.B3 => LineNote.C3,
            NoteName.C4 => LineNote.D3,
            NoteName.CS4 => LineNote.D3,
            NoteName.DF4 => LineNote.E3,
            NoteName.D4 => LineNote.E3,
            NoteName.DS4 => LineNote.E3,
            NoteName.EF4 => LineNote.F3,
            NoteName.E4 => LineNote.F3,
            NoteName.F4 => LineNote.G3,
            NoteName.FS4 => LineNote.G3,
            NoteName.GF4 => LineNote.A3,
            NoteName.G4 => LineNote.A3,
            NoteName.GS4 => LineNote.A3,
            NoteName.AF4 => LineNote.B3,
            NoteName.A4 => LineNote.B3,
            NoteName.AS4 => LineNote.B3,
            NoteName.BF4 => LineNote.C4,
            NoteName.B4 => LineNote.C4,
            NoteName.C5 => LineNote.D4,
            NoteName.CS5 => LineNote.D4,
            NoteName.DF5 => LineNote.E4,
            NoteName.D5 => LineNote.E4,
            NoteName.DS5 => LineNote.E4,
            NoteName.EF5 => LineNote.F4,
            NoteName.E5 => LineNote.F4,
            NoteName.F5 => LineNote.F4,
            NoteName.FS5 => LineNote.F4,
            _ => LineNote.Unknown
        };

        private static LineNote NoteNameToLineNoteTenor(NoteName noteName) => noteName switch
        {
            NoteName.F2 => LineNote.B1,
            NoteName.FS2 => LineNote.B1,
            NoteName.GF2 => LineNote.C2,
            NoteName.G2 => LineNote.C2,
            NoteName.GS2 => LineNote.C2,
            NoteName.AF2 => LineNote.D2,
            NoteName.A2 => LineNote.D2,
            NoteName.AS2 => LineNote.D2,
            NoteName.BF2 => LineNote.E2,
            NoteName.B2 => LineNote.E2,
            NoteName.C3 => LineNote.F2,
            NoteName.CS3 => LineNote.F2,
            NoteName.DF3 => LineNote.G2,
            NoteName.D3 => LineNote.G2,
            NoteName.DS3 => LineNote.G2,
            NoteName.EF3 => LineNote.A2,
            NoteName.E3 => LineNote.A2,
            NoteName.F3 => LineNote.B2,
            NoteName.FS3 => LineNote.B2,
            NoteName.GF3 => LineNote.C3,
            NoteName.G3 => LineNote.C3,
            NoteName.GS3 => LineNote.C3,
            NoteName.AF3 => LineNote.D3,
            NoteName.A3 => LineNote.D3,
            NoteName.AS3 => LineNote.D3,
            NoteName.BF3 => LineNote.E3,
            NoteName.B3 => LineNote.E3,
            NoteName.C4 => LineNote.F3,
            NoteName.CS4 => LineNote.F3,
            NoteName.DF4 => LineNote.G3,
            NoteName.D4 => LineNote.G3,
            NoteName.DS4 => LineNote.G3,
            NoteName.EF4 => LineNote.A3,
            NoteName.E4 => LineNote.A3,
            NoteName.F4 => LineNote.B3,
            NoteName.FS4 => LineNote.B3,
            NoteName.GF4 => LineNote.C4,
            NoteName.G4 => LineNote.C4,
            NoteName.GS4 => LineNote.C4,
            NoteName.AF4 => LineNote.D4,
            NoteName.A4 => LineNote.D4,
            NoteName.AS4 => LineNote.D4,
            NoteName.BF4 => LineNote.E4,
            NoteName.B4 => LineNote.E4,
            NoteName.C5 => LineNote.F4,
            NoteName.CS5 => LineNote.F4,
            _ => LineNote.Unknown
        };

        private static (Length? offset1, Length? offset2) LineNoteToLedgerOffset(LineNote lineNote) =>  lineNote switch
        {
            LineNote.F4 => (Percent(300f), Percent(900f)),
            LineNote.E4 => (Percent(0f), Percent(600f)),
            LineNote.D4 => (Percent(300f), null),
            LineNote.C4 => (Percent(0f), null),

            LineNote.E2 => (Percent(0f), null),
            LineNote.D2 => (Percent(-300f), null),
            LineNote.C2 => (Percent(0f), Percent(-600f)),
            LineNote.B1 => (Percent(-300f), Percent(-900f)),
            _ => (null, null)
        };

        private static int NotationRangeToButtonCount(NotationRange range) => range switch
        {
            NotationRange.Bass => 33,
            NotationRange.Treble => 34,
            NotationRange.Alto => 33,
            NotationRange.Tenor => 33,
            _ => 0,
        };

        private static float LineNoteToTopOffset(LineNote lineNote) => lineNote switch
        {
            LineNote.F4 => 0,
            LineNote.E4 => 5,
            LineNote.D4 => 10,
            LineNote.C4 => 15,
            LineNote.B3 => 20,
            LineNote.A3 => 25,
            LineNote.G3 => 30,
            LineNote.F3 => 35,
            LineNote.E3 => 40,
            LineNote.D3 => 45,
            LineNote.C3 => 50,
            LineNote.B2 => 55,
            LineNote.A2 => 60,
            LineNote.G2 => 65,
            LineNote.F2 => 70,
            LineNote.E2 => 75,
            LineNote.D2 => 80,
            LineNote.C2 => 85,
            LineNote.B1 => 90,
            _ => -1
        };

        static int GetKeyOffset() => NotationGame.NotationRange switch
        {
            NotationRange.Bass => 1,
            NotationRange.Treble => 21,
            NotationRange.Alto => 11,
            NotationRange.Tenor => 8,
            _ => 0
        };
    }
}