using System;

namespace spellpotion
{
    public class ActionEvent<T>
    {
        private Action<T> listener;
        private readonly Action<T> invoker;

        public ActionEvent(out Action<T> invoker)
        {
            this.invoker = x => listener?.Invoke(x);
            invoker = this.invoker;
        }

        public void AddListener(Action<T> listener)
        {
            this.listener += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            this.listener -= listener;
        }
    }

    public class ActionEvent
    {
        private Action listener;
        private readonly Action invoker;

        public ActionEvent(out Action invoker)
        {
            this.invoker = () => listener?.Invoke();
            invoker = this.invoker;
        }

        public void AddListener(Action listener)
        {
            this.listener += listener;
        }

        public void RemoveListener(Action listener)
        {
            this.listener -= listener;
        }
    }

}