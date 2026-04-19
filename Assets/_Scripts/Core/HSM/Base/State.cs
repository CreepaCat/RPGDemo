using System.Collections.Generic;


namespace HSM
{
    public abstract class State
    {
        public readonly StateMachine StateMachine;
        public readonly State Parent;
        public State ActiveChild;

        public State(StateMachine stateMachine, State parent = null)
        {
            StateMachine = stateMachine;
            Parent = parent;
        }
        /// <summary>
        /// 获取初始化子状态（进入当前状态时要同步进入的状态），如果为null说明当前状态是叶子状态
        /// </summary>
        /// <returns></returns>
        protected virtual State GetInitialState() => null;


        /// <summary>
        /// 切换到目标状态，返回null说明不切换，停留在当前状态,并执行当前激活的子节点
        /// 如果由父节点控制子节点间的状态切换，则不要返回null,否则会导致无法退出某个子状态
        /// </summary>
        /// <returns></returns>
        protected virtual State GetTransition() => null;

        //生命循环函数
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate(float deltaTime) { }

        //节点树内部进入和退出函数
        internal void InnerEnter()
        {

            //进入时将父状态的激活子状态设为自己
            if (Parent != null)
            {
                Parent.ActiveChild = this;
            }
            OnEnter(); //执行外部逻辑进入

            State init = GetInitialState();
            if (init != null) init.InnerEnter(); //递归进入初始化的子节点
        }
        internal void InnerExit()
        {   //递归退出激活的子节点
            if (ActiveChild != null) ActiveChild.InnerExit();
            ActiveChild = null;

            // 从父节点解绑，避免在外层退出链中被父节点重复递归退出。
            if (Parent != null && ReferenceEquals(Parent.ActiveChild, this))
            {
                Parent.ActiveChild = null;
            }

            OnExit(); //执行外部逻辑退出
        }

        internal void InnerUpdate(float deltaTime)
        {
            State t = GetTransition();

            //*防止重复进入同一个子节点
            if (t != null && ActiveChild != t)
            {

                //*从叶子节点开始退出，保证状态退出顺序正确
                var from = Leaf();
                if (from != t)
                {
                    StateMachine.Sequencer.RequestTransition(from, t);
                    return;
                }

            }

            //递归调用InnerUpdate逻辑，可直接到达leaf节点
            ActiveChild?.InnerUpdate(deltaTime);

            //执行leaf节点的外部update逻辑
            OnUpdate(deltaTime);
        }


        /// <summary>
        /// 获取当前节点的叶子
        /// </summary>
        /// <returns></returns>
        public State Leaf()
        {
            State s = this;
            //深度遍历
            while (s.ActiveChild != null) s = s.ActiveChild;
            return s;
        }

        public bool IsInLeafPath(State s)
        {
            for (State current = s; current != null; current = current.Parent)
            {
                if (current == this) return true;
            }
            return false;
        }


        /// <summary>
        /// 向上遍历父节点，直到根节点，逐步返回所有遍历到的节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<State> PathToRoot()
        {

            for (State s = this; s != null; s = s.Parent)
            {
                yield return s;
            }
        }
    }
}
