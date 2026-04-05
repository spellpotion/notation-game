using spellpotion.Data;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace spellpotion.Manager
{
    [CustomEditor(typeof(System))]
    public class SystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(15f);

            var pathResources = "Assets/Resources";
            var pathVersionInfo = Path.Combine(pathResources, "Version Info.asset");
            var pathBuildTxt = Path.Combine(pathResources, "build.txt");

            var existsResources = Directory.Exists(pathResources);
            var existsVersionInfo = File.Exists(pathVersionInfo);
            var existsBuildTxt = File.Exists(pathBuildTxt);

            EditorGUI.BeginDisabledGroup(existsResources && existsVersionInfo && existsBuildTxt);

            if (GUILayout.Button("Generate Resources"))
            {
                if (!existsResources)
                {
                    Directory.CreateDirectory(pathResources);
                    AssetDatabase.Refresh();

                    Debug.Log($"created directory {pathResources}");
                }

                if (!existsVersionInfo)
                {
                    var versionInfo = CreateInstance<VersionInfo>();

                    AssetDatabase.CreateAsset(versionInfo, pathVersionInfo);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Debug.Log($"created file {pathVersionInfo}");
                }

                if (!existsBuildTxt)
                {
                    File.WriteAllText(pathBuildTxt, "0");

                    AssetDatabase.ImportAsset(pathBuildTxt);
                    AssetDatabase.Refresh();

                    Debug.Log($"created file {pathBuildTxt}");
                }
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
