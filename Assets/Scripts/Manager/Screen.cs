using spellpotion.Manager;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace spellpotion.midiTutor.Manager
{
    public class Screen : 抽象Manager<Screen, Config.Screen>
    {
        #region Events


        public static ActionEvent<ScreenType> OnClearScreen = new(out onClearScreen);
        private static Action<ScreenType> onClearScreen;

        public static ActionEvent<ScreenType> OnScreenCleared = new(out onScreenCleared);
        private static Action<ScreenType> onScreenCleared;

        public static ActionEvent<ScreenType> OnSetScreen = new(out onSetScreen);
        private static Action<ScreenType> onSetScreen;

        public static ActionEvent<ScreenType> OnScreenSet = new(out onScreenSet);
        private static Action<ScreenType> onScreenSet;


        #endregion Events
        #region PublicStatic


        //public static void RunChangeScreenInitial()
        //    => InstanceRun(x => x.StartCoroutine(x.ChangeScreen務(x.Config.InitialScreen)));

        //public static void RunChangeScreen(ScreenType screenType)
        //    => InstanceRun(x => x.StartCoroutine(x.ChangeScreen務(screenType)));


        #endregion PublicStatic
        #region Common


        private Transform screenParent;
        private 抽象Screen screen現;

        //private ScreenType ScreenType現 
        //    => screen現 == null ? ScreenType.None : screen現.Type;

        protected override void OnEnable()
        {
            base.OnEnable();

            screenParent = new GameObject("User Interface").transform;
        }

        protected override void OnDisable()
        {
            if (screenParent != null)
            {
                Destroy(screenParent.gameObject);
                screenParent = null;
            }

            base.OnDisable();
        }

        //private IEnumerator ChangeScreen務(ScreenType screenType)
        //{
        //    Debug.Log($"{名} 📜 ChangeScreen <i>{ScreenType現}</i> → <i>{screenType}</i>");

        //    // clear screen

        //    if (screen現 != null)
        //    {
        //        var type = screen現.Type;

        //        Debug.Log($"{名} 📢 clear screen <i>{type}</i> …");
        //        yield return SyncEvent務(onClearScreen, type);

        //        Destroy(screen現.gameObject);

        //        onScreenCleared?.Invoke(type);
        //        Debug.Log($"{名} ✔️ <i>{type}</i> cleared");
        //    }

        //    // set screen

        //    Debug.Log($"{名} 📢 set screen <i>{screenType}</i> …");
        //    yield return SyncEvent務(onSetScreen, screenType);

        //    if (screenType != ScreenType.None)
        //    {
        //        screen現 = LoadScreen(screenType);
        //    }

        //    onScreenSet?.Invoke(screenType);
        //    Debug.Log($"{名} ✔️ <i>{screenType}</i> set");

        //    抽象Screen LoadScreen(ScreenType screenType)
        //    {
        //        var screenPrefab = Config.screenPrefabs.FirstOrDefault(s => s.Type == screenType);
        //        if (screenPrefab == null)
        //        {
        //            Debug.LogWarning($"{名} Screen <i>{screenType}</i> not found");
        //            return null;
        //        }

        //        var screen = Instantiate(screenPrefab, screenParent);
        //        screen.name = screenType.ToString();

        //        return screen;
        //    }
        //}


        #endregion Common

        protected static new string 名 => "[<color=#b34d94>Screen長</color>]";
    }
}