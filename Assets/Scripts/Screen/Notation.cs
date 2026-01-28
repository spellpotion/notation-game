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
        private const float offsetXMax = 4700f;
        private const float offsetXMin = 400f;
        private const float duration = 4f;

        private VisualElement note;
        private VisualElement ledger;
        private VisualElement ledger上;
        private VisualElement ledger下;

        private Image sharp;
        private Image flat;

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

        protected void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            note = root.Q<VisualElement>("note");
            note.style.display = DisplayStyle.None;

            flat = root.Q<Image>("flat-image");
            flat.style.display = DisplayStyle.None;

            sharp = root.Q<Image>("sharp-image");
            sharp.style.display = DisplayStyle.None;

            ledger = root.Q<VisualElement>("ledger-container");
            ledger.style.display = DisplayStyle.None;

            ledger上 = root.Q<VisualElement>("ledger-line-top");
            ledger下 = root.Q<VisualElement>("ledger-line-bot");
        }

        protected void Start()
        {
            StartCoroutine(Demo務());
        }

        private IEnumerator Demo務()
        {
            var noteNames = (NoteName[])Enum.GetValues(typeof(NoteName));

            while (true)
            {
                var randomNote = noteNames[Random.Range(1, noteNames.Length)];

                var lineNote = NoteNameToLineNote(randomNote);
                var accidental = NoteNameToAccidental(randomNote);

                flat.style.display = accidental == Accidental.Flat ? DisplayStyle.Flex : DisplayStyle.None;
                sharp.style.display = accidental == Accidental.Sharp ? DisplayStyle.Flex : DisplayStyle.None;
                ShowLedger(lineNote);

                releaseNote務 = StartCoroutine(ReleaseNote務(lineNote));

                yield return new WaitUntil(() => releaseNote務 == null);
            }
        }

        private IEnumerator ReleaseNote務(LineNote lineNote)
        {
            var offsetY = new Length(LineNoteToBassOffset(lineNote), LengthUnit.Percent);
            var difference = offsetXMax - offsetXMin;

            var time始 = Time.time;
            var time的 = time始 + duration;

            note.style.display = DisplayStyle.Flex;

            while (Time.time < time的)
            {
                var time = Time.time - time始;
                var progress = Mathf.Clamp01(time / duration);

                var offsetX = new Length(offsetXMax - progress * difference, LengthUnit.Percent);

                note.style.translate = new Translate(offsetX, offsetY, 0f);

                yield return null;
            }

            note.style.display = DisplayStyle.None;

            releaseNote務 = null;
        }

        private static readonly Dictionary<LineNote, (Length y, float top, float bottom)> LedgerMap = new()
        {
            [LineNote.F4] = (Offset(0f), 100f, 100f),
            [LineNote.E4] = (Offset(-16.66f), 100f, 100f),
            [LineNote.D4] = (Offset(-33.33f), 0f, 100f),
            [LineNote.C4] = (Offset(-50f), 0f, 100f),

            [LineNote.E2] = (Offset(-16.66f), 100f, 0f),
            [LineNote.D2] = (Offset(-33.33f), 100f, 0f),
            [LineNote.C2] = (Offset(-50f), 100f, 100f),
            [LineNote.B2] = (Offset(-66.66f), 100f, 100f),
        };

        private static readonly Length LedgerX = new(-8f, LengthUnit.Percent);

        private static Length Offset(float value) => new(value, LengthUnit.Percent);

        private void ShowLedger(LineNote lineNote)
        {
            if ((lineNote > LineNote.E2) && (lineNote < LineNote.C4))
            {
                ledger.style.display = DisplayStyle.None;
                return;
            }

            ledger.style.display = DisplayStyle.Flex;

            if (!LedgerMap.TryGetValue(lineNote, out var cfg))
            {
                cfg = (Offset(0f), 0f, 0f);
            }

            ledger上.style.opacity = cfg.top;
            ledger下.style.opacity = cfg.bottom;
            ledger.style.translate = new Translate(LedgerX, cfg.y, 0f);
        }

        private LineNote MidiNoteToLineNote(int midiNote) => midiNote switch
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

        private LineNote NoteNameToLineNote(NoteName noteName) => noteName switch
        {
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
            _ => LineNote.Unknown
        };

        private Accidental NoteNameToAccidental(NoteName noteName) => noteName switch
        {
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
            _ => Accidental.None
        };

        private float LineNoteToBassOffset(LineNote lineNote) => lineNote switch
        {
            LineNote.F4 => 0,
            LineNote.E4 => 50,
            LineNote.D4 => 100,
            LineNote.C4 => 150,
            LineNote.B3 => 200,
            LineNote.A3 => 250,
            LineNote.G3 => 300,
            LineNote.F3 => 350,
            LineNote.E3 => 400,
            LineNote.D3 => 450,
            LineNote.C3 => 500,
            LineNote.B2 => 550,
            LineNote.A2 => 600,
            LineNote.G2 => 650,
            LineNote.F2 => 700,
            LineNote.E2 => 750,
            LineNote.D2 => 800,
            LineNote.C2 => 850,
            LineNote.B1 => 900,
            _ => -1
        };
    }
}