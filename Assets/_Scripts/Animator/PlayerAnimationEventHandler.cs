using UnityEngine;

public  class PlayerAnimationEventHandler : MonoBehaviour
{
    private Player _player;
    
    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }
    
    #region 动画事件

    //此种动画事件的写法值得借鉴
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
           _player.TpController.OnFootstep();
        }
    }
    
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            // Debug.Log("播放落地声");
            _player.TpController.OnLand();

        }
    }

    private void OnAttackStart(AnimationEvent animationEvent)
    {
     
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            Debug.Log("OnAttackStart");
            _player.Fighter.OpenCombatTargetDetector();
        }
    }
    
    private void OnAttackOver(AnimationEvent animationEvent)
    {
      
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
           Debug.Log("OnAttackOver");
            _player.TpController.OnAttackOver();
           // _player.Animator.SetBool(PlayerAnimatorParamConfig.animIDAttack,false);
            
            //关闭武器伤害检测
            _player.Fighter.CloseCombatTargetDetector();
        }
            
       
    }

    private void OnRollOver(AnimationEvent animationEvent)
    {
       // Debug.Log("OnRollOver");
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
           // _player.Input.RollInput(false);
           // _player.Weapon.UnsheathSword();
        }
    }

    private void OnFreeFallOver(AnimationEvent animationEvent)
    {
       // Debug.Log("OnFreeFallOver");
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            // _player.Input.RollInput(false);
            // _player.Input.JumpInput(false);
            // _player.Input.SprintInput(false);
           
        }
    }
    
    private void Hit(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
           // Debug.Log("Hit");
            //打开武器伤害范围检测，对所有符合条件的对象施加伤害
            _player.Fighter.Hit();
        }
       
    }

    private void OnDisableInput(AnimationEvent animationEvent)
    {
        Debug.Log("OnDisableInput");
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            _player.DisableInput();
        }
    }

    private void OnEnableInput(AnimationEvent animationEvent)
    {
        Debug.Log("OnEnableInput");
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            _player.EnableInput();
        }
    }

    private void Unsheath(AnimationEvent animationEvent)
    {
        
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            Debug.Log("Unsheath");
           
            _player.Weapon.OnUnsheath();
        }
    }

    private void Sheath(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            Debug.Log("Sheath");
          
            _player.Weapon.OnSheath();
        }
    }
    #endregion
}
