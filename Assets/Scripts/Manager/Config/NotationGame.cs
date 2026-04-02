using spellpotion.Manager.Config;
using UnityEngine;

namespace spellpotion.midiTutor.Manager.Config
{
    [CreateAssetMenu(fileName = "GameNotation", menuName = "Scriptable Objects/Manager Config/GameNotation")]
    public class NotationGame : 抽象Config<Manager.NotationGame>
    {
        public GameMode GameMode = GameMode.None;
        public bool AutoAnswer = false;
        [Range(0f, 1f)]
        public float AutoAnswerSuccessRate = 1f;
    }
}