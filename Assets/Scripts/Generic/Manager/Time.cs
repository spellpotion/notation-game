using System;
using Unity.VisualScripting;
using UnityEngine;

namespace spellpotion.Manager
{
    public class Time : 抽象Manager<Time, Config.Time>
    {
        #region Events


        public static ActionEvent<bool> OnPauseSet = new(out onPauseSet);
        private static Action<bool> onPauseSet;


        #endregion Events
        #region PublicStatic


        public static void SetPause(bool pause)
            => InstanceRun(x => x.SetPause_Instance(pause));


        #endregion PublicStatic
        #region Common


        private float timeScale;

        protected override void OnEnable()
        {
            base.OnEnable();

            timeScale = UnityEngine.Time.timeScale;
        }


        #endregion Common
        #region PrivateInstance


        private void SetPause_Instance(bool pause)
        {
            UnityEngine.Time.timeScale = pause ? 0f : timeScale;
            AudioListener.pause = pause;
            
            Debug.Log($"{名} <color=#{Color.gold.ToHexString()}>" +
                $"{(pause ? "⏸ PAUSE" : "⏯ RESUME")}</color>");

            onPauseSet?.Invoke(pause);
        }


        #endregion PrivateInstance
    }
}