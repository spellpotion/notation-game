using System;
using UnityEngine;

namespace spellpotion.midiTutor.Manager
{
    public class Time : 抽象Manager<Time, Config.Time>
    {
        public static ActionEvent<bool> OnPauseSet = new(out onPauseSet);
        private static Action<bool> onPauseSet;

        public static void SetPause(bool pause)
            => InstanceRun(x => x.SetPause_Instance(pause));

        private void SetPause_Instance(bool pause)
        {
            UnityEngine.Time.timeScale = pause ? 0f : Config.TimeScale;
            AudioListener.pause = pause;

            Debug.Log($"{名} {(pause ? "⏸ PAUSE" : "⏯ RESUME")}");

            onPauseSet?.Invoke(pause);
        }
    }
}