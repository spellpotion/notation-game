using spellpotion.Screen;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace spellpotion.Manager
{
    public class Screen : 抽象Manager<Screen, Config.Screen>
    {
        #region Events


        public static ActionEvent<string> OnClearScreen = new(out onClearScreen);
        private static Action<string> onClearScreen;

        public static ActionEvent<string> OnScreenCleared = new(out onScreenCleared);
        private static Action<string> onScreenCleared;

        public static ActionEvent<string> OnSetScreen = new(out onSetScreen);
        private static Action<string> onSetScreen;

        public static ActionEvent<string> OnScreenSet = new(out onScreenSet);
        private static Action<string> onScreenSet;


        #endregion Events
        #region PublicStatic


        public static void ChangeScreen(string screenName)
            => InstanceRun(x => x.ChangeScreen_Instance(screenName));


        #endregion PublicStatic
        #region Common


        private Transform screenParent;
        private UIDocument document;
        private 抽象Screen screen;

        protected override void OnEnable()
        {
            base.OnEnable();

            screenParent = new GameObject("User Interface").transform;
        }

        protected override void OnDisable()
        {
            SetNull(ref changeScreen務);

            if (screenParent != null)
            {
                Destroy(screenParent.gameObject);

                screenParent = null;
                document = null;
            }

            base.OnDisable();
        }

        protected override void OnStart()
        {
            base.OnStart();

            document = screenParent.AddComponent<UIDocument>();
            document.panelSettings = Config.PanelSettings;

            changeScreen務 = StartCoroutine(ChangeScreen務(Config.screenConfigs[0]));
        }


        #endregion Common
        #region PrivateInstance


        private void ChangeScreen_Instance(string screenName)
        {
            if (changeScreen務 != null)
            {
                Debug.LogWarning($"{名} Screen change interrupted");
                SetNull(ref changeScreen務);
            }

            var screenConfig = Config.screenConfigs.FirstOrDefault(x => x.ScreenType.Name == screenName);

            if (screenConfig == null)
            {
                Debug.LogError($"{名} Screen config for <i>{screenName}</i> not found");
                return;
            }

            changeScreen務 = StartCoroutine(ChangeScreen務(screenConfig));
        }


        #endregion PrivateInstance
        #region Coroutines


        private Coroutine changeScreen務;

        private IEnumerator ChangeScreen務(spellpotion.Screen.Config.抽象Config screenConfig)
        {

            Debug.Log($"{名} 📜 change screen → <i>{screenConfig.ScreenType.Name}</i>");

            if (screen != null)
            {
                var type先 = screen.GetType().Name;

                Debug.Log($"{名} 📢 clear screen <i>{type先}</i> …");
                yield return SyncEvent務(onClearScreen, type先);

                Destroy(screen);
                yield return null;

                onScreenCleared?.Invoke(type先);
                Debug.Log($"{名} ✔️ <i>{type先}</i> screen cleared");
            }

            var type = screenConfig.ScreenType.Name;

            Debug.Log($"{名} 📢 set screen <i>{type}</i> …");
            yield return SyncEvent務(onSetScreen, type);

            document.visualTreeAsset = screenConfig.SourceAsset;
            screen = (抽象Screen)document.gameObject.AddComponent(screenConfig.ScreenType);

            onScreenSet?.Invoke(type);
            Debug.Log($"{名} ✔️ <i>{type}</i> screen set");

            changeScreen務 = null;
        }


        #endregion Coroutines
    }
}