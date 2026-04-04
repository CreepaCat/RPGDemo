using Unity.VisualScripting;
using UnityEngine;

namespace MyBehaviourTree
{


    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Success,
            Failure

        }
        [TextArea] public string description;
        [HideInInspector] public State state = State.Running;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public bool started = false;
        [System.NonSerialized] protected BehaviourTreeContext context;

        public State Update()
        {
            if (started == false)
            {
                OnStart();
                started = true;
            }
            state = OnUpdate();

            if (state != State.Running)
            {
                OnStop();
                started = false;
            }

            return state;

        }

        /// <summary>
        /// 中断运行状态
        /// </summary>
        public virtual void Abort()
        {
            if (!started)
            {
                return;
            }

            OnStop();
            started = false;
            state = State.Failure;
        }

        /// <summary>
        /// 克隆方法，用于创建运行时文件，确保在运行时的修改不会影响到源文件
        /// </summary>
        /// <returns></returns>
        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        public virtual void Bind(BehaviourTreeContext context)
        {
            this.context = context;
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();




    }
}
