using System.Collections;
using UnityEngine;

namespace spellpotion
{
    public abstract class MonoScript : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            StartCoroutine(OnEnable務());
        }

        protected virtual void OnStart() { }

        private IEnumerator OnEnable務()
        {
            yield return null;

            OnStart();
        }

        protected void SetNull(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}