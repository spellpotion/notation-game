using spellpotion.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace spellpotion
{
    public abstract class 抽象Manager<T, U> : 抽象Manager<T>
        where T : 抽象Manager<T, U>
        where U : 抽象Config<T>
    {
        [SerializeField] protected U Config = null;

        public override void SetConfig(抽象Config config)
        {
            Config = config as U;
        }
    }

    public abstract class 抽象Manager<T> : 抽象Manager
        where T : 抽象Manager<T>
    {
        #region Singleton

        protected static T Instance { get; private set; } = null;

        protected override void OnEnable()
        {
            if (Instance != null)
            {
                Debug.LogWarning($"{名} Singleton rule violation");
                return;
            }

            Instance = (T)this;
            Debug.Log($"{名} <b>Enabled</b>");

            base.OnEnable();
        }

        protected virtual void OnDisable()
        {
            if (Instance != this)
            {
                Debug.LogWarning($"{名} Singleton rule violation");
                return;
            }

            Instance = null;
            Debug.Log($"{名} <b>Disabled</b>");
        }

        protected static void InstanceRun(Action<T> action)
        {
            if (Instance == null)
            {
                Debug.LogWarning($"{名} Instance not available");
                return;
            }

            action(Instance);
        }

        protected static TResult InstanceRun<TResult>(Func<T, TResult> func)
        {
            if (Instance == null)
            {
                Debug.LogWarning($"{名} Instance not available");
                return default;
            }

            return func(Instance);
        }

        #endregion Singleton
        #region SyncEvent


        public static void WaitUntil(Func<bool> func)
            => InstanceRun(x => x.waitUntil.Add(func));
        private readonly List<Func<bool>> waitUntil = new();

        protected IEnumerator SyncEvent務(Action action, Action callback = null)
        {
            waitUntil.Clear();

            action?.Invoke();

            yield return new WaitUntil(() => waitUntil.All(func => func()));

            callback?.Invoke();
        }

        protected IEnumerator SyncEvent務<E>(Action<E> action, E value, Action<E> callback = null)
        {
            waitUntil.Clear();

            action?.Invoke(value);

            yield return new WaitUntil(() => waitUntil.All(func => func()));

            callback?.Invoke(value);
        }


        #endregion SyncEvent

        protected static string 名 => $"[{typeof(T).Name}長]";
    }

    public abstract class 抽象Manager : MonoScript
    {
        public abstract void SetConfig(Manager.抽象Config config);
    }
}