using spellpotion.Data;
using UnityEngine;

namespace spellpotion.Manager
{
    public class System : 抽象Manager<System, Config.System>
    {
        #region Bootstrap


        protected void Awake()
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 120;

            CreateManagers();
        }

        protected void CreateManagers()
        {
            foreach (var config in Config.Configs)
            {
                var manager = new GameObject(config.ManagerType.Name, config.ManagerType);
                manager.transform.SetParent(transform, true);

                var component = manager.GetComponent(config.ManagerType) as 抽象Manager;
                component.SetConfig(config);
            }
        }


        #endregion Bootstrap
        #region VersionNumber


        public static string GetVersionNumber()
        {
            var version = Resources.Load<VersionInfo>("Version Info");
            var buildNumber = Resources.Load<TextAsset>("build").text;

            return $"v{version.Major}.{version.Minor}.{version.Patch}.{int.Parse(buildNumber)}";
        }


        #endregion VersionNumber
    }
}