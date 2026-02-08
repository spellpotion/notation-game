using spellpotion.midiTutor.Data;
using System.Collections;
using UnityEngine;

namespace spellpotion.midiTutor
{
    [RequireComponent(typeof(AudioSource))]
    public class Oscillator : MonoBehaviour
    {
        private const float amplitudeMax = 1f;
        private const float amplitudeSmoothing = .001f;

        protected void OnEnable()
        {
            Manager.NotationGame.OnResult.AddListener(OnResult);
            Manager.NotationGame.OnQuestion.AddListener(OnQuestion);
        }

        protected void OnDisable()
        {
            Manager.NotationGame.OnQuestion.RemoveListener(OnQuestion);
            Manager.NotationGame.OnResult.RemoveListener(OnResult);
        }

        private void OnQuestion((NoteName noteName, float duration) question)
        {
            var keyName = Conversion.NoteNameToKeyName(question.noteName);
            var noteNumber = (int)keyName + Manager.Midi.Offset;
            frequency = NoteNumberToFrequency(noteNumber);
        }

        private void OnResult(Result result)
        {
            if (result != Result.Correct) return;

            StartCoroutine(PlayNote務());
        }

        private IEnumerator PlayNote務()
        {
            this.amplitude的 = amplitudeMax;

            yield return new WaitForSeconds(.5f);

            this.amplitude的 = 0f;
        }

        private int sampleRate;

        protected void Awake()
        {
            sampleRate = AudioSettings.outputSampleRate;
        }

        private float frequency = 440f;
        private float amplitude的 = 0f;
        private float amplitude現 = 0f;
        private double phase;

        protected void OnAudioFilterRead(float[] data, int channels)
        {
            var phaseIncrement = 2.0 * Mathf.PI * frequency / sampleRate;

            for (var i = 0; i < data.Length; i += channels)
            {
                amplitude現 += (amplitude的 - amplitude現) * amplitudeSmoothing;

                var sample = (float)(amplitude現 * System.Math.Sin(phase));

                for (var c = 0; c < channels; c++)
                {
                    data[i + c] = sample;
                }

                phase = (phase + phaseIncrement) % (2.0 * Mathf.PI);
            }
        }

        private static float NoteNumberToFrequency(int noteNumber)
            => 440f * Mathf.Pow(2f, (noteNumber - 69) / 12f);
    }
}