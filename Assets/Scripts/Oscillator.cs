#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using spellpotion.midiTutor.Data;
using System.Collections;
using UnityEngine;

namespace spellpotion.midiTutor
{
    [RequireComponent(typeof(AudioSource))]
    public class Oscillator : MonoBehaviour
    {
        private const float amplitudeMax = .8f;
        private const float amplitudeSmoothing = .001f;

#if UNITY_WEBGL && !UNITY_EDITOR
        public static void ResumeWebAudio() => WebAudio_Resume();
#endif

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

        private int sampleRate;

        protected void Awake()
        {
            sampleRate = AudioSettings.outputSampleRate;
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
#if !UNITY_WEBGL || UNITY_EDITOR
            this.amplitude的 = amplitudeMax;
#else
            WebAudio_StartSine(frequency, amplitudeMax, amplitudeSmoothing);
#endif

            yield return new WaitForSeconds(.5f);

#if !UNITY_WEBGL || UNITY_EDITOR
            this.amplitude的 = 0f;
#else
            WebAudio_StopSine(amplitudeSmoothing);
#endif
        }

        private float frequency = 440f;
#if !UNITY_WEBGL || UNITY_EDITOR
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
#else
        [DllImport("__Internal")] private static extern void WebAudio_Resume();
        [DllImport("__Internal")] private static extern void WebAudio_StartSine(float frequency, float amplitude, float rampMs);
        [DllImport("__Internal")] private static extern void WebAudio_SetFrequency(float frequency, float rampMs);
        [DllImport("__Internal")] private static extern void WebAudio_SetAmplitude(float amplitude, float rampMs);
        [DllImport("__Internal")] private static extern void WebAudio_StopSine(float rampMs);
#endif

        private static float NoteNumberToFrequency(int noteNumber)
            => 440f * Mathf.Pow(2f, (noteNumber - 69) / 12f);
    }
}