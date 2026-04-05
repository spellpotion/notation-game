using UnityEngine;

namespace spellpotion.Manager.Config
{
    [CreateAssetMenu(fileName = "System", menuName = "Scriptable Objects/Manager Config/System")]
    public class System : 抽象Config<Manager.System>
    {
        public 抽象Config[] Configs;
    }
}