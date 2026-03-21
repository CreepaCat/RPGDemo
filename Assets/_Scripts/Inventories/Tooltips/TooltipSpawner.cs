using System;
using RPGDemo.Inventories.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGDemo.Inventories.Tooltips
{
    public class TooltipSpawner : MonoBehaviour/*,ISpawnTooltip*/, IPointerEnterHandler, IPointerExitHandler
    {
        //CONFIG
        [SerializeField] ItemTooltipUI tooltipPrefab;


        //CACHE
        private ItemTooltipUI _tooltip = null;

        private void OnDisable()
        {
            ClearTooltip();
        }
        private void OnDestroy()
        {
            ClearTooltip();
        }

        private void Update()
        {
            if (InventoryDragItem.IsDragging())
            {
                ClearTooltip();
            }
        }

        public bool CanCreateTooltip()
        {
            IItemGetter itemGetter = GetComponentInParent<IItemGetter>();
            if (itemGetter == null) return false;

            return itemGetter.GetItem() != null;
        }

        public void CreateTooltip()
        {
            if (InventoryDragItem.IsDragging())
            {
                ClearTooltip();
                return;
            }
            var parentCanvas = GetComponentInParent<Canvas>();
            //先判断当前是否有缓存的tooltipUI,然后结合是否可以显示tooltipUI
            if (_tooltip && !CanCreateTooltip()) //该销毁却还在显示
            {
                //清除显示并销毁
                ClearTooltip();
                return;
            }
            if (!_tooltip && CanCreateTooltip())//该显示却没有显示
            {
                print("Creating tooltip");
                _tooltip = Instantiate(tooltipPrefab, parentCanvas.transform);


            }

            if (_tooltip)
            {
                UpdateTooltip(_tooltip);
                PositionTooltip(_tooltip.gameObject);
            }
        }

        public void ClearTooltip()
        {
            if (_tooltip)
            {
                Destroy(_tooltip.gameObject);
            }
        }


        private void UpdateTooltip(ItemTooltipUI tooltip)
        {
            //  print("UpdateTooltip");
            if (tooltip == null) return;

            var item = GetComponentInParent<SlotUI>().GetItem();
            tooltip.Setup(item);
        }

        /// <summary>
        /// 将tooltip显示在正确的位置，跟随鼠标，且不超出屏幕
        /// </summary>
        /// <param name="tooltip"></param>
        private void PositionTooltip(GameObject tooltip)
        {
            //强制刷新一次canvas
            Canvas.ForceUpdateCanvases();

            //用内置方法获得rect四个角的坐标,存到tooltipCorners中,四角顺序从左下角顺时针排列
            var tooltipCorners = new Vector3[4];
            //结构体赋值，直接传入
            tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);

            //此脚本挂载在slotUI上，所以可以直接获取rt
            var slotConers = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotConers);

            //判断tip显示区域,若item在屏幕上半，则在下方显示；若在屏幕左边，则tip在右边显示
            //选择slot的角点
            bool tipBelow = transform.position.y > Screen.height / 2f;
            bool tipRight = transform.position.x < Screen.width / 2f;

            int slotCornerIndex = 0;
            int tipCornerIndex = 0;
            if (tipBelow && !tipRight) //tip在左下显示，则取右上角为标点，而slot取左下角为标点
            {
                tipCornerIndex = 2;
                slotCornerIndex = 0;
            }
            else if (!tipBelow && !tipRight) //tip在左上显示，则取右下角为标点，而slot取左上
            {
                tipCornerIndex = 3;
                slotCornerIndex = 1;
            }
            else if (!tipBelow && tipRight) //tip在右上显示
            {
                tipCornerIndex = 0;
                slotCornerIndex = 2;
            }
            else
            {
                tipCornerIndex = 1;
                slotCornerIndex = 3;
            }
            //平滑移动
            _tooltip.transform.position += slotConers[slotCornerIndex] - tooltipCorners[tipCornerIndex];
        }

        #region 鼠标事件接口




        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (InventoryDragItem.IsDragging())
            {
                ClearTooltip();
                return;
            }
            var parentCanvas = GetComponentInParent<Canvas>();
            //先判断当前是否有缓存的tooltipUI,然后结合是否可以显示tooltipUI
            if (_tooltip && !CanCreateTooltip()) //该销毁却还在显示
            {
                //清除显示并销毁
                ClearTooltip();
                return;
            }
            if (!_tooltip && CanCreateTooltip())//该显示却没有显示
            {
                // print("Creating tooltip");
                _tooltip = Instantiate(tooltipPrefab, parentCanvas.transform);


            }

            if (_tooltip)
            {
                UpdateTooltip(_tooltip);
                PositionTooltip(_tooltip.gameObject);
            }
        }



        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();
            ClearTooltip();

        }
        #endregion



    }
}
