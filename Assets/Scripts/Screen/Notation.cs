using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace spellpotion.midiTutor.Screen
{
    [RequireComponent(typeof(UIDocument))]
    public class Notation : 抽象Screen
    {
        private const float offsetXMax = 1200f;
        private const float duration = 4f;

        private VisualElement note;

        private VisualElement flat;
        private VisualElement ledger1;
        private VisualElement ledger2;
        private VisualElement sharp;

        private readonly Button[] keyButtons
            = new Button[((BassKey[])Enum.GetValues(typeof(BassKey))).Length];

        private Coroutine releaseNote務;
        private Coroutine demo務;

        protected void OnEnable()
        {
            Manager.Midi.OnNoteOn.AddListener(OnNoteOn);
        }

        protected void OnDisable()
        {
            Manager.Midi.OnNoteOn.RemoveListener(OnNoteOn);
        }

        private void OnNoteOn(int midiNote)
        {
            Debug.Log($"DBG Note {midiNote} received");

            //var lineNote = MidiNoteToLineNote(midiNote);

            //if (releaseNote務 != null)
            //{
            //    StopCoroutine(releaseNote務);
            //}

            //if (lineNote == LineNote.Unknown)
            //{
            //    note.style.display = DisplayStyle.None;
            //    releaseNote務 = null;

            //    return;
            //}

            //releaseNote務 = StartCoroutine(ReleaseNote務(lineNote));
        }

        private void OnNoteOn(BassKey keyName)
        {
            Debug.Log($"DBG NoteOn {keyName}");
        }

        protected void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

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

            var bassKeys = (BassKey[])Enum.GetValues(typeof(BassKey));

            for (var i = 0; i < bassKeys.Length; i++)
            {
                var buttonName = $"key-{Enum.GetName(typeof(BassKey), bassKeys[i]).ToLower()}";

                var button = root.Q<Button>(buttonName);
                button.clicked += () => { OnNoteOn(bassKeys[i]); };

                keyButtons[i] = button;
            }
        }

        protected void Start()
        {
            //StartCoroutine(Demo務());
        }

        private NoteName note現;

        private IEnumerator Demo務()
        {
            var noteNames = (NoteName[])Enum.GetValues(typeof(NoteName));

            while (true)
            {
                note現 = noteNames[Random.Range(1, noteNames.Length)];

                var lineNote = NoteNameToLineNote(note現);
                var accidental = NoteNameToAccidental(note現);

                flat.style.display = accidental == Accidental.Flat ? DisplayStyle.Flex : DisplayStyle.None;
                sharp.style.display = accidental == Accidental.Sharp ? DisplayStyle.Flex : DisplayStyle.None;

                UpdateLedger(lineNote);

                releaseNote務 = StartCoroutine(ReleaseNote務(lineNote));

                yield return new WaitUntil(() => releaseNote務 == null);
            }
        }

        private IEnumerator ReleaseNote務(LineNote lineNote)
        {
            var difference = offsetXMax;

            var time始 = Time.time;
            var time的 = time始 + duration;

            note.style.display = DisplayStyle.Flex;
            note.style.top = new Length(LineNoteToTopOffset(lineNote), LengthUnit.Percent);

            while (Time.time < time的)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / duration);

                var offsetX = new Length(offsetXMax - progress * difference, LengthUnit.Percent);
                var easeOut = Utils.EaseOut(0, 1f, progress * 4f);

                note.style.translate = new Translate(offsetX, 0f, 0f);
                note.style.opacity = easeOut;

                yield return null;
            }

            note.style.display = DisplayStyle.None;

            releaseNote務 = null;
        }

        private static readonly Dictionary<LineNote, (Length? offset1, Length? offset2)> LedgerOffset = new()
        {
            [LineNote.F4] = (Offset(300f), Offset(900f)),
            [LineNote.E4] = (Offset(0f), Offset(600f)),
            [LineNote.D4] = (Offset(300f), null),
            [LineNote.C4] = (Offset(0f), null),

            [LineNote.E2] = (Offset(0f), null),
            [LineNote.D2] = (Offset(-300f), null),
            [LineNote.C2] = (Offset(0f), Offset(-600f)),
            [LineNote.B1] = (Offset(-300f), Offset(-900f)),
        };

        private static Length Offset(float value) => new(value, LengthUnit.Percent);

        private void UpdateLedger(LineNote lineNote)
        {
            if ((lineNote > LineNote.E2) && (lineNote < LineNote.C4))
            {
                ledger1.style.display = DisplayStyle.None;
                ledger2.style.display = DisplayStyle.None;
                return;
            }

            var ledgerOffset = LedgerOffset[lineNote];

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

        private static LineNote NoteNameToLineNote(NoteName noteName) => noteName switch
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

        private static readonly List<string> ButtonNames = new()
        {
            "key-b1",
            "key-a1b1",
            "key-c2", "key-d2", "key-e2", "key-f2", "key-g2", "key-a2", "key-b2",
             "key-c2d2", "key-d2e2", "key-f2g2", "key-g2a2", "key-a2b2",
            "key-c3", "key-d3", "key-e3", "key-f3", "key-g3", "key-a3", "key-b3",
            "key-c3d3", "key-d3e3", "key-f3g3", "key-g3a3", "key-a3b3",
            "key-c4", "key-d4", "key-e4", "key-f4",
            "key-c4d4", "key-d4e4", "key-f4g4",
        };
    }
}