using UnityEngine;

namespace MyNodeEditor.Extension.Dialogue
{
    [CreateAssetMenu(fileName = "New DialogueNodeEnterEvent", menuName = "MyDialogue/DialogueEvent/New Event")]
    public class DialogueNodeEventSO:ScriptableObject
    {
        public string description;
    }
}