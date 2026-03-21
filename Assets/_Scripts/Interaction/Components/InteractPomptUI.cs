using TMPro;
using UnityEngine;

namespace RPGDemo.InteractionSystem
{


    public class InteractPomptUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);

        [SerializeField] private string keyHint = "[F]";

        Camera cam;
        Transform player;
        Canvas canvas;
        RectTransform canvasRect;
        RectTransform labelRect;

        IInteractable interactableTarget;
        

        private void Awake()
        {
            cam = Camera.main;
            canvas = label.GetComponentInParent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();
            labelRect = label.GetComponent<RectTransform>();

            player = GameObject.FindWithTag("Player").transform;
            Hide();
        }

        private void LateUpdate()
        {
            //if(target == null) return;
            if(interactableTarget == null) return;  
            if(!label.gameObject.activeSelf) label.gameObject.SetActive(true);
            
            Vector3 woeldPos = player.position + worldOffset;
            Vector3 screenPos = cam.WorldToScreenPoint(woeldPos);
            Camera uiCame = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCame,
                    out Vector2 localPos))
            {
                labelRect.anchoredPosition = localPos;
            }
        }

        public void Show(IInteractable interactable)
        {
            if(interactable == null) return;
            label.text = $"{keyHint} {interactable.DisplayName}";
            interactableTarget =  interactable;
            label.gameObject.SetActive(true);
        }

        public void Hide()
        {
            label.gameObject.SetActive(false);
            interactableTarget = null;
        }
    }
}
