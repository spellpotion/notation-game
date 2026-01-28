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
    }
}
