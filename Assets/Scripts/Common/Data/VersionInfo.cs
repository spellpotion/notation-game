using UnityEngine;

namespace spellpotion.Data
{
    [CreateAssetMenu(fileName = "Version Info", menuName = "Scriptable Objects/Version Info")]
    public class VersionInfo : ScriptableObject
    {
        public int Major = 0;
        public int Minor = 1;
        public int Patch = 0;
    }
}