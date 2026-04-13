using System;
using UnityEngine;


namespace RPGDemo.InteractionSystem
{
    public class Outliner : MonoBehaviour
    {
        private Renderer[] highlightTarget = null;
        private RenderingLayerMask interactionOutlineMixLayer;


        private RenderingLayerMask originalRenderingLayerMask;
        // [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer; //角色渲染

        private void Awake()
        {
            //skinnedMeshRenderer ??=

            highlightTarget = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (highlightTarget == null || highlightTarget.Length < 1)
                highlightTarget = GetComponentsInChildren<MeshRenderer>();
            if (highlightTarget == null || highlightTarget.Length < 1) return;

            originalRenderingLayerMask = highlightTarget[0].renderingLayerMask;

            int DefaultLayerIndex = 0;
            //在编辑器设置的属于交互轮廓线的renderinglayer
            int interactionOutlineLayerIndex = 9;

            uint mixedMask = (1u << DefaultLayerIndex) | (1u << interactionOutlineLayerIndex);

            interactionOutlineMixLayer = mixedMask;
        }

        public void ShowOutline()
        {
            if (highlightTarget == null || highlightTarget.Length < 1) return;
            for (int i = 0; i < highlightTarget.Length; i++)
            {
                if (highlightTarget[i] == null) continue;
                highlightTarget[i].renderingLayerMask = interactionOutlineMixLayer;

            }
        }

        public void HideOutline()
        {
            if (highlightTarget == null || highlightTarget.Length < 1) return;
            for (int i = 0; i < highlightTarget.Length; i++)
            {
                if (highlightTarget[i] == null) continue;
                highlightTarget[i].renderingLayerMask = originalRenderingLayerMask;

            }
        }

    }
}
