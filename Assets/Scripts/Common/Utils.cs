using UnityEngine;

namespace spellpotion
{
    public static class Utils
    {
        public static float EaseIn(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = t * t;
            return Mathf.Lerp(from, to, t);
        }

        public static float EaseOut(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = 1f - (1f - t) * (1f - t);
            return Mathf.Lerp(from, to, t);
        }

        public static Color Lerp(Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color(
                Mathf.Lerp(a.r, b.r, t),
                Mathf.Lerp(a.g, b.g, t),
                Mathf.Lerp(a.b, b.b, t),
                Mathf.Lerp(a.a, b.a, t)
            );
        }

        public static int RandomMin(int max, int iterations)
        {
            var min = int.MaxValue;

            for (var i = 0; i < iterations; i++)
            {
                min = Mathf.Min(min, Random.Range(0, max));
            }

            return min;
        }

        public static float RandomUp(float max, int iterations)
        {
            if (iterations <= 0)
            {
                return Random.Range(0f, max);
            }
            else
            {
                return Random.Range(RandomUp(max, iterations - 1), max);
            }
        }
    }
}
