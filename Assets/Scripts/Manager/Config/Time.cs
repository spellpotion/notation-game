using spellpotion.Manager;
using UnityEngine;

namespace spellpotion.midiTutor.Manager.Config
{
    [CreateAssetMenu(fileName = "Time", menuName = "Scriptable Objects/Manager Config/Time")]
    public class Time : 抽象Config<Manager.Time>
    {
        [Min(0f)]
        public float TimeScale = 1f;
    }
}