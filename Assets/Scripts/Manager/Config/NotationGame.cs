using spellpotion.Manager;
using UnityEngine;

namespace spellpotion.midiTutor.Manager.Config
{
    [CreateAssetMenu(fileName = "GameNotation", menuName = "Scriptable Objects/Manager Config/GameNotation")]
    public class NotationGame : 抽象Config<Manager.NotationGame>
    {
        public NotationRange NotationRange;
        public GameMode GameMode;

        private void OnValidate()
        {
            Debug.Assert(NotationRange != NotationRange.None);
            Debug.Assert(GameMode != GameMode.None);
        }
    }
}