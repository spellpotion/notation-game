using spellpotion.midiTutor.Manager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace spellpotion.midiTutor.Screen
{
    [RequireComponent(typeof(UIDocument))]
    public class Notation : 抽象Screen
    {
        private const float translateXMax = 1450f;
        private const float durationPulse = .6f;
        private const float durationPulseHalf = .3f;

        private const int keyOffsetBass = 1;
        private const int keyOffsetTreble = 0; // TODO

        private VisualElement root;
        private VisualElement note;
        private VisualElement flat;
        private VisualElement ledger1;
        private VisualElement ledger2;
        private VisualElement sharp;
        private VisualElement greyZone;

        private readonly Button[] keys = new Button[33];

        private Coroutine releaseNote務;
        private Coroutine pulse務;

        protected override void OnEnable()
        {
            GameNotation.OnQuery.AddListener(OnQuery);
            GameNotation.OnAnswer.AddListener(OnAnswer);
        }

        protected void OnDisable()
        {
            GameNotation.OnAnswer.RemoveListener(OnAnswer);
            GameNotation.OnQuery.RemoveListener(OnQuery);
        }

        private void OnQuery((NoteName noteName, float duration) query)
        {
            var lineNote = GetLineNote();
            var accidental = NoteNameToAccidental(query.noteName);
            var ledgerOffset = LineNoteToLedgerOffset(lineNote);

            flat.style.display = accidental == Accidental.Flat ? DisplayStyle.Flex : DisplayStyle.None;
            sharp.style.display = accidental == Accidental.Sharp ? DisplayStyle.Flex : DisplayStyle.None;

            if (ledgerOffset.offset1.HasValue)
            {
                ledger1.style.display = DisplayStyle.Flex;
                ledger1.style.translate = new Translate(0, ledgerOffset.offset1.Value);
            }
            else ledger1.style.display = DisplayStyle.None;

            if (ledgerOffset.offset2.HasValue)
            {
                ledger2.style.display = DisplayStyle.Flex;
                ledger2.style.translate = new Translate(0, ledgerOffset.offset2.Value);
            }
            else ledger2.style.display = DisplayStyle.None;

            SetNull(ref releaseNote務);
            releaseNote務 = StartCoroutine(ReleaseNote務(lineNote, query.duration));

            LineNote GetLineNote() => GameNotation.NotationType switch
            {
                NotationType.Bass => NoteNameToLineNoteBass(query.noteName),
                NotationType.Treble => NoteNameToLineNoteTreble(query.noteName),
                _ => LineNote.Unknown
            };
        }

        private void OnAnswer(bool value)
        {
            SetNull(ref pulse務);

            pulse務 = StartCoroutine(Pulse務(value));
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
        }

        protected void Start()
        {
            var keyOffset = GameNotation.NotationType == NotationType.Treble ?
                keyOffsetTreble : keyOffsetBass;
            var keyNames = (KeyName[])Enum.GetValues(typeof(KeyName));

            for (var i = 0; i < keys.Length; i++)
            {
                var buttonName = $"key-{i}";
                var index = keyOffset + i;

                var button = root.Q<Button>(buttonName);
                button.clicked += () =>
                {
                    GameNotation.Answer(keyNames[index]);
                };

                keys[i] = button;
            }
        }

        private IEnumerator ReleaseNote務(LineNote lineNote, float duration)
        {
            var difference = translateXMax;

            var time始 = Time.time;
            var time的 = time始 + duration;

            note.style.display = DisplayStyle.Flex;
            note.style.top = new Length(LineNoteToTopOffset(lineNote), LengthUnit.Percent);

            while (Time.time < time的)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / duration);

                var offsetX = new Length(translateXMax - progress * difference, LengthUnit.Percent);

                note.style.translate = new Translate(offsetX, 0f, 0f);

                yield return null;
            }

            note.style.display = DisplayStyle.None;

            releaseNote務 = null;
        }

        private IEnumerator Pulse務(bool value)
        {
            var time始 = Time.time;
            var time中 = time始 + durationPulseHalf;
            var time的 = time始 + durationPulse;

            var color原 = new Color(.6f, .6f, .6f, 1f); // not possible to get
            var color的 = value ? Color.green : Color.red;

            while (Time.time < time中)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / durationPulseHalf);

                greyZone.style.backgroundColor = Utils.Lerp(color原, color的, progress);

                yield return null;
            }

            greyZone.style.backgroundColor = color的;

            while (Time.time < time的)
            {
                var time = Time.time - time中;
                var progress = Mathf.Clamp01(time / durationPulseHalf);

                greyZone.style.backgroundColor = Utils.Lerp(color的, color原, progress);

                yield return null;
            }

            greyZone.style.backgroundColor = color原;

            pulse務 = null;
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
            _ => LineNote.Unknown
        };

        private static Accidental NoteNameToAccidental(NoteName noteName) => noteName switch
        {
            NoteName.BF1 => Accidental.Flat,
            NoteName.CS2 => Accidental.Sharp,
            NoteName.DF2 => Accidental.Flat,
            NoteName.DS2 => Accidental.Sharp,
            NoteName.EF2 => Accidental.Flat,
            NoteName.FS2 => Accidental.Sharp,
            NoteName.GF2 => Accidental.Flat,
            NoteName.GS2 => Accidental.Sharp,
            NoteName.AF2 => Accidental.Flat,
            NoteName.AS2 => Accidental.Sharp,
            NoteName.BF2 => Accidental.Flat,
            NoteName.CS3 => Accidental.Sharp,
            NoteName.DF3 => Accidental.Flat,
            NoteName.DS3 => Accidental.Sharp,
            NoteName.EF3 => Accidental.Flat,
            NoteName.FS3 => Accidental.Sharp,
            NoteName.GF3 => Accidental.Flat,
            NoteName.GS3 => Accidental.Sharp,
            NoteName.AF3 => Accidental.Flat,
            NoteName.AS3 => Accidental.Sharp,
            NoteName.BF3 => Accidental.Flat,
            NoteName.CS4 => Accidental.Sharp,
            NoteName.DF4 => Accidental.Flat,
            NoteName.DS4 => Accidental.Sharp,
            NoteName.EF4 => Accidental.Flat,
            NoteName.FS4 => Accidental.Sharp,
            _ => Accidental.None
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
    }
}