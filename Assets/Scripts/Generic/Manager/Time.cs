using System;
using Unity.VisualScripting;
using UnityEngine;

namespace spellpotion.Manager
{
    public class Time : 抽象Manager<Time, Config.Time>
    {
        public static ActionEvent<bool> OnPauseSet = new(out onPauseSet);
        private static Action<bool> onPauseSet;

        private float timeScale;

        protected override void OnEnable()
        {
            base.OnEnable();

            timeScale = UnityEngine.Time.timeScale;
            SetPause_Instance(true);
        }

        public static void SetPause(bool pause)
            => InstanceRun(x => x.SetPause_Instance(pause));

        private void SetPause_Instance(bool pause)
        {
            UnityEngine.Time.timeScale = pause ? 0f : timeScale;
            AudioListener.pause = pause;
            
            Debug.Log($"{名} <color=#{Color.gold.ToHexString()}>" +
                $"{(pause ? "⏸ PAUSE" : "⏯ RESUME")}</color>");

            onPauseSet?.Invoke(pause);
        }
    }
}