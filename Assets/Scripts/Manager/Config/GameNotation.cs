using spellpotion.Manager;
using UnityEngine;

namespace spellpotion.midiTutor.Manager.Config
{
    [CreateAssetMenu(fileName = "GameNotation", menuName = "Scriptable Objects/Manager Config/GameNotation")]
    public class GameNotation : 抽象Config<Manager.GameNotation>
    {
        public NotationType NotationType;

        private void OnValidate()
        {
            Debug.Assert(NotationType != NotationType.None);
        }
    }
}