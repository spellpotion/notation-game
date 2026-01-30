using UnityEngine;
using UnityEngine.UIElements;

namespace spellpotion
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class 抽象Screen : MonoScript
    {
        //public abstract ScreenType Type { get; }

        protected static Length Percent(float value) => new(value, LengthUnit.Percent);
    }
}