using UnityEngine;

namespace RPGDemo.Core
{
    public abstract class Item : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] protected string _itemID;
        [SerializeField] protected string _displayName;
        [SerializeField][TextArea] protected string _description;
        [SerializeField] protected Sprite _icon;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(_itemID))
            {
                _itemID = System.Guid.NewGuid().ToString();
            }
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            //NO OP
        }


    }
}
