using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class PlayerAvoidanceAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private CharacterController cc;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cc = GetComponent<CharacterController>();

        // 关键：禁用 Agent 自身移动，只用于避障计算
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.enabled = true;  // 启用以加入 Crowd

        // 高优先级：敌人让路
        agent.avoidancePriority = 0;
    }

    void LateUpdate()
    {
        // 同步玩家实际位置到 Agent（让敌人看到实时位置）
        agent.transform.position = transform.position;
        agent.transform.rotation = transform.rotation;  // 可选，如果需要旋转预测

        // 同步速度（让敌人预测玩家移动，避免突推）
        agent.velocity = cc.velocity;
    }
}
