using UnityEngine;
using UnityEngine.UIElements;

namespace spellpotion.midiTutor.Screen
{
    [RequireComponent(typeof(UIDocument))]
    public class Notation : 抽象Screen
    {
        private VisualElement note;

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

            var lineNote = MidiNoteToLineNote(midiNote);

            if (lineNote == LineNote.Unknown)
            {
                note.style.display = DisplayStyle.None;
                return;
            }

            var offset = LineNoteToBassOffset(lineNote);
            
            note.style.display = DisplayStyle.Flex;
            note.style.translate = new Translate(Length.Percent(0f), Length.Percent(offset), 0f);
        }

        protected void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            note = root.Q<VisualElement>("note");
            note.style.display = DisplayStyle.None;
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