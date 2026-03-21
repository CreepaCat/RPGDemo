using UnityEngine;
namespace MyNodeEditor.Extension.Dialogue{
    /// <summary>
    ///定义一个谈话对象
    /// </summary>
    [CreateAssetMenu(menuName = "MyDialogue/DialogueRole", fileName = "New DialogueRole")]
    public class DialogueRole : ScriptableObject
    {
        public string speakerName;
        public Sprite avatar;

    }
}
