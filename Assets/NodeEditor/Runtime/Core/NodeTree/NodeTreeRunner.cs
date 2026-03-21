using UnityEngine;

/// <summary>
/// 控制对话树的进入和退出
/// </summary>
public class NodeTreeRunner : MonoBehaviour
{

    public NodeTree nodeTree;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nodeTree.OnTreeEnd();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            nodeTree.OnTreeStart();
        }

        //只有当树不为空时才执行其Update方法
        if (nodeTree != null)
        {
            nodeTree.Update();
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            nodeTree.OnTreeEnd();
        }
    }
}
