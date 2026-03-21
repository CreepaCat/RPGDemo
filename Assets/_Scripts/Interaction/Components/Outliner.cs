using System;
using UnityEngine;


namespace RPGDemo.InteractionSystem
{
    public class Outliner : MonoBehaviour
    {
        private Renderer highlightTarget;
        private RenderingLayerMask interactionOutlineMixLayer;


        private RenderingLayerMask originalRenderingLayerMask;
       // [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer; //角色渲染

        private void Awake()
        {
            //skinnedMeshRenderer ??= 
         
            highlightTarget ??= GetComponentInChildren<SkinnedMeshRenderer>();
            highlightTarget ??= GetComponentInChildren<MeshRenderer>();
            if (highlightTarget==null) return;
            
            originalRenderingLayerMask = highlightTarget.renderingLayerMask;

            int DefaultLayerIndex = 0;
            //在编辑器设置的属于交互轮廓线的renderinglayer
            int interactionOutlineLayerIndex = 8;
            
            uint mixedMask = (1u << DefaultLayerIndex) | (1u << interactionOutlineLayerIndex);

            interactionOutlineMixLayer = mixedMask;
        }

        public void ShowOutline()
        {
            if (highlightTarget==null) return;
            highlightTarget.renderingLayerMask = interactionOutlineMixLayer;
        }

        public void HideOutline()
        {
            if (highlightTarget==null) return;
            highlightTarget.renderingLayerMask = originalRenderingLayerMask;
        }

    }
}
