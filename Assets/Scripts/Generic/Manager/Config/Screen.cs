using UnityEngine;
using UnityEngine.UIElements;

namespace spellpotion.Manager.Config
{
    [CreateAssetMenu(fileName = "Screen", menuName = "Scriptable Objects/Manager Config/Screen")]
    public class Screen : 抽象Config<Manager.Screen>
    {
        public PanelSettings PanelSettings;
        public spellpotion.Screen.Config.抽象Config[] screenConfigs;
    }
}