using spellpotion.Manager.Config;
using UnityEngine;

namespace spellpotion.midiTutor.Manager.Config
{
    [CreateAssetMenu(fileName = "Screen", menuName = "Scriptable Objects/Manager Config/Screen")]
    public class Screen : 抽象Config<Manager.Screen>
    {
        public ScreenType InitialScreen;

        public 抽象Screen[] screenPrefabs;
    }
}