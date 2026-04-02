using UnityEngine;
using UnityEngine.UIElements;

namespace spellpotion
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class 抽象Screen : MonoScript
    {
        protected static Length Percent(float value) => new(value, LengthUnit.Percent);
    }
}