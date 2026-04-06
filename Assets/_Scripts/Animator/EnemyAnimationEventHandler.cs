using UnityEngine;

public class EnemyAnimationEventHandler : MonoBehaviour
{
    Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }
    private void OnAttackStart(AnimationEvent animationEvent)
    {

        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            Debug.Log("OnAttackStart" + animationEvent.intParameter);
            _enemy.Fighter.OnAttackStart(animationEvent.intParameter);
        }
    }

    private void OnAttackOver(AnimationEvent animationEvent)
    {

        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {




            //关闭武器伤害检测
            _enemy.Fighter.OnAttackOver();
        }


    }
}
