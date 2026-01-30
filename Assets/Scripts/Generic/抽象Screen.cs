using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace spellpotion
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class 抽象Screen : MonoBehaviour
    {
        //public abstract ScreenType Type { get; }

        protected static Length Percent(float value) => new(value, LengthUnit.Percent);
    }
}