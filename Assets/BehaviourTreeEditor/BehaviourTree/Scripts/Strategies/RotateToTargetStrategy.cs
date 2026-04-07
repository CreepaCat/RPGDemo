using UnityEngine;

namespace MyBehaviourTree
{
    [System.Serializable]
    public class RotateToTargetStrategy : ActionStrategy
    {
        [SerializeField] bool rotateToPlayer;

        [SerializeField] private bool useBlackboardTarget = true;
        [SerializeField] private string targetKey = "Target";
        [SerializeField] private Transform target;
        [SerializeField] private float rotateSpeed = 360f;
        [SerializeField] private float maxRotateDuration = 1f;

        Quaternion desiredRotation;
        float startTime;

        public override void OnStart(BehaviourTreeContext context)
        {
            base.OnStart(context);
            var forward = (context.Blackboard.GetPlayer().position - context.Transform.position).normalized;
            desiredRotation = Quaternion.LookRotation(forward, Vector3.up);
            if (rotateSpeed < 0)
            {
                rotateSpeed = 360f;
            }

            if (maxRotateDuration < 0f)
            {
                maxRotateDuration = 1f;
            }
            startTime = Time.time;
        }

        public override Node.State Process(BehaviourTreeContext context)
        {
            if (!context.AnimationHandler.CanMoveOrRotate()) return Node.State.Failure;

            var forward = (context.Blackboard.GetPlayer().position - context.Transform.position).normalized;
            desiredRotation = Quaternion.LookRotation(forward, Vector3.up);
            context.Transform.rotation = Quaternion.Slerp(context.Transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime / 180f);
            // context.Transform.rotation

            if (Vector3.Angle(context.Transform.forward, forward) < 5f ||
            Time.time > startTime + maxRotateDuration)
            {

                return Node.State.Success;
            }
            return Node.State.Running;
        }
    }
}
