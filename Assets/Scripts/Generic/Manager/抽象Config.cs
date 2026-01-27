using System;
using UnityEngine;

namespace spellpotion.Manager
{
    public abstract class 抽象Config<T> : 抽象Config
        where T : 抽象Manager<T>
    {
        public override Type ManagerType => typeof(T);
    }

    public abstract class 抽象Config : ScriptableObject
    {
        public abstract Type ManagerType { get; }
    }
}