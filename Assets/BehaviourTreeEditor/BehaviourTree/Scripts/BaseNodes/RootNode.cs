namespace MyBehaviourTree
{

    public class RootNode : Node
    {
        public Node child;

        public override void Abort()
        {
            child?.Abort();
            base.Abort();
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            return child.Update();
        }

        public override Node Clone()
        {
            RootNode rootNode = Instantiate(this);
            rootNode.child = child?.Clone();
            return rootNode;
        }
    }
}
