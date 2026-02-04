using System;
using UnityEngine;

namespace spellpotion.midiTutor.Manager
{
    public class Score : 抽象Manager<Score, Config.Score>
    {
        #region Events


        public static ActionEvent<int> OnUpdateScore = new(out onUpdateScore);
        private static Action<int> onUpdateScore;


        #endregion Events
        #region PublicStatic


        public static void Add(int value)
            => InstanceRun(x => x.Add_Instance(value));


        #endregion PublicStatic
        #region Generic


        private int score = 0;

        private void Add_Instance(int value)
        {
            score += value;

            onUpdateScore?.Invoke(score);
        }


        #endregion Generic
    }
}