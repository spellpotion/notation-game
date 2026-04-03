using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace spellpotion.Screen.Config
{
    public abstract class 抽象Config<T> : 抽象Config
        where T : 抽象Screen
    {
        public override Type ScreenType => typeof(T);
    }

    public abstract class 抽象Config : ScriptableObject
    {
        public VisualTreeAsset SourceAsset;

        public abstract Type ScreenType { get; }
    }
}
