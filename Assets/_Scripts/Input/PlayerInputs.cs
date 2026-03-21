using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace RPGDemo.Inputs
{
	public class PlayerInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		//public bool attack;
		public bool roll;
		public bool interact;
		
		public bool chargeAttack;
		public bool lightAttack;
		public bool hasReachedChargeAttackPoint = false;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		
		//CACHE
		private InputActionMap playerActions;

		private void Awake()
		{
		
			playerActions = GetComponent<PlayerInput>().actions.FindActionMap("Player");
			
		}
		
		private void OnEnable()
		{
			var attackAction = playerActions.FindAction("Attack");
			attackAction?.Enable();
			attackAction.started += OnAttackStarted;
			attackAction.performed += OnAttackPerformed;
			attackAction.canceled += OnAttackCanceled;
		}

		private void OnDisable()
		{
			var attackAction = playerActions.FindAction("Attack");
			attackAction?.Disable();

			//取消订阅，避免内存泄漏（尤其重要）
			attackAction.started   -= OnAttackStarted;
			attackAction.performed -= OnAttackPerformed;
			attackAction.canceled  -= OnAttackCanceled;
		}

		public void EnablePlayerInput()
		{
			Debug.Log("Enable Player Input");
			playerActions.Enable();
			SetCursorState(true);
			
		}

		public void DisablePlayerInput()
		{
			Debug.Log("Disable Player Input");
			playerActions.Disable();
			SetCursorState(false);
		}

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
		
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		
		
		public void OnRoll(InputValue value)
		{
			//Debug.Log("Pressed Roll" + value.isPressed);
			RollInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}


		// ────────────── 新增：长按相关的三个阶段 ──────────────
		private void OnAttackStarted(InputAction.CallbackContext ctx)
		{
			hasReachedChargeAttackPoint = false;
			//按下时立即触发一次轻攻击
			lightAttack = true;
		}

		private void OnAttackPerformed(InputAction.CallbackContext ctx)
		{
			// 如果没加 Interaction，默认是按下就触发 performed
			// 如果加了 Hold Interaction，这里才会是「长按成功」
			//chargeAttack = true;
			//lightAttack = false;
			lightAttack = true;
			hasReachedChargeAttackPoint =  true;
		}

		private void OnAttackCanceled(InputAction.CallbackContext ctx)
		{
			
			//lightAttack =  true;
			// if (hasReachedChargeAttackPoint)
			// {
			// 	lightAttack = false;
			// }

			lightAttack = false;
			//chargeAttack = false;
			
		}
		
		
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void HeavelAttackInput(bool newAttackState)
		{
			chargeAttack = newAttackState;
		}

		public void RollInput(bool newRollState)
		{
			roll = newRollState;
		}
		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}


		// private void OnApplicationFocus(bool hasFocus)
		// {
		// 	SetCursorState(cursorLocked);
		// }

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}