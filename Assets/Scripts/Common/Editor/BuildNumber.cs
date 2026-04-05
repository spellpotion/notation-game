using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace spellpotion
{
    public class BuildNumber : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            const string path = "Assets/Resources/build.txt";
            int buildNumber = 0;

            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogError($"[BuildNumber] <i>{path}</i> does not exist");
                return;
            }

            if (!int.TryParse(File.ReadAllText(path), out buildNumber))
            {
                UnityEngine.Debug.LogError($"[BuildNumber] unable to parse {path}");
                return;
            }

            buildNumber++;

            File.WriteAllText(path, buildNumber.ToString());
            AssetDatabase.Refresh();
        }
    }
}