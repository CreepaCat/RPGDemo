using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGDemo.Core.DraggingFrame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    where T : class
    {
        //CACHE
        private IDragSource<T> _dragSource;
        private Vector3 _originPos;
        private Transform _originParent;
        private Canvas _parentCanvas;


        private static DragItem<T> currentDragging = null;

        private void Awake()
        {
            _originParent = transform.parent;
            _parentCanvas = GetComponentInParent<Canvas>();
        }

        public static bool IsDragging() => currentDragging != null;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            currentDragging = this;
            _dragSource = GetComponentInParent<IDragSource<T>>();
            _originPos = transform.position;

            //关闭射线阻挡，否则不能放下item
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            //将item放在UI层级的最上方显示
            transform.SetParent(_parentCanvas.transform);

        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            currentDragging = null;
            //结束拖动的逻辑判断，先恢复原先的位置（以防目标地点不合格）
            transform.position = _originPos;
            transform.SetParent(_originParent, true);
            GetComponent<CanvasGroup>().blocksRaycasts = true; //恢复射线阻挡，可触发pointerEnter

            IDragDestination<T> container;
            //1、不在UI上(本Canvas所属的UI)
            if (!EventSystem.current.IsPointerOverGameObject())
            {

                // Debug.Log("鼠标不在UI上");
                //丢弃处理，将parentCanvas作为垃圾桶
                container = _parentCanvas.GetComponent<IDragDestination<T>>();

            }
            else //2、从鼠标pointerEnter的gameObject处获取容器
            {
                //  Debug.Log("鼠标在UI上");
                container = GetContainer(eventData);
            }

            if (container != null)
            {
                DropItemIntoContainer(container);
            }


        }

        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
                if (container != null)
                {
                    // Debug.Log("鼠标在IDragDestination上");
                }
                return container;
            }
            return null;

        }

        /// <summary>
        /// 将item放进目标容器
        /// </summary>
        /// <param name="container"></param>
        private void DropItemIntoContainer(IDragDestination<T> destination)
        {
            //情况1：目标容器和原容器相同
            if (ReferenceEquals(_dragSource, destination))
            {
                return;
            }
            //里氏替换，将原格和目标格都替换为合格容器，若为空说明地点不合格
            var destinationContainer = destination as IDragContainer<T>;
            var sourceContainer = _dragSource as IDragContainer<T>;

            //情况2：不发生交换，只是物品从原格（或背包外）移动到目标格（或背包外）
            //目标点为空，或者两格的物品类型相同
            if (destinationContainer == null || sourceContainer == null || destinationContainer.GetItem() == null
                || ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {
                AttemptSimpleTransfer(destination);
                return;
            }

            //情况3：物品交换

            AttempSwap(sourceContainer, destinationContainer);

        }

        private void AttempSwap(IDragContainer<T> sourceContainer, IDragContainer<T> destinationContainer)
        {
            Debug.Log("AttempSwap");
            var removedSourceItem = sourceContainer.GetItem();
            var removedSourceItemAmount = sourceContainer.GetAmount();
            var removedDestinationItem = destinationContainer.GetItem();
            var removedDestinationItemAmount = destinationContainer.GetAmount();

            //1、先从两个格子各自移除物品
            sourceContainer.RemoveItems(removedSourceItemAmount);
            destinationContainer.RemoveItems(removedDestinationItemAmount);

            //2、计算多余的数量，先拿回原格(原格 和 目标格都需要计算)
            var sourceTackBack = CaculateTackBack(sourceContainer, destinationContainer, removedSourceItemAmount, removedSourceItem);
            var destinationTackBack = CaculateTackBack(destinationContainer, sourceContainer, removedDestinationItemAmount, removedDestinationItem);

            //调整交换数量,将多余的放回原容器
            if (sourceTackBack > 0)
            {
                sourceContainer.AddItems(removedSourceItem, sourceTackBack);
                removedSourceItemAmount -= sourceTackBack;
            }

            if (destinationTackBack > 0)
            {
                destinationContainer.AddItems(removedDestinationItem, destinationTackBack);
                removedDestinationItemAmount -= destinationTackBack;
            }

            //3、若调整交换数量后仍不满足条件，则此次交换失败，放回各自的原容器
            if (sourceContainer.GetMaxAcceptable(removedDestinationItem) < removedDestinationItemAmount
                || destinationContainer.GetMaxAcceptable(removedSourceItem) < removedSourceItemAmount)
            {
                sourceContainer.AddItems(removedSourceItem, removedSourceItemAmount);
                destinationContainer.AddItems(removedDestinationItem, destinationTackBack);
                return;
            }


            //4、若都符合条件则执行完美交换
            // print("背包物品完美交换");
            if (removedSourceItemAmount > 0)
            {
                destinationContainer.AddItems(removedSourceItem, removedSourceItemAmount);
            }

            if (removedDestinationItemAmount > 0)
            {
                sourceContainer.AddItems(removedDestinationItem, removedDestinationItemAmount);
            }

        }

        private int CaculateTackBack(IDragContainer<T> sourceContainer, IDragContainer<T> destinationContainer,
            int removerSourceItemAmount, T removedSourceItem)
        {
            int tackBackAmount = 0;
            if (removerSourceItemAmount > destinationContainer.GetMaxAcceptable(removedSourceItem))
            {
                tackBackAmount = removerSourceItemAmount - destinationContainer.GetMaxAcceptable(removedSourceItem);

                //如果退回的物品数量大于removeSource能承受的数量，返回失败信息(什么情况下会发生？ 如：从世界拾取物品添加到背包时)
                int sourceTackBackAcceptable = sourceContainer.GetMaxAcceptable(removedSourceItem);
                if (tackBackAmount > sourceTackBackAcceptable)
                {
                    //tackBackAmount = 0 说明此次tackback失败
                    tackBackAmount = 0;
                }

            }

            return tackBackAmount;
        }

        private bool AttemptSimpleTransfer(IDragDestination<T> destination)
        {
            //  Debug.Log("AttemptSimpleTransfer");
            //简单item移动
            var draggingItem = _dragSource.GetItem();
            var draggingAmount = _dragSource.GetAmount();

            //计算接收数
            var acceptable = destination.GetMaxAcceptable(draggingItem);
            var toTransferAmount = Mathf.Min(acceptable, draggingAmount);



            if (toTransferAmount > 0)
            {
                Debug.Log("移动物体数量" + toTransferAmount);
                _dragSource.RemoveItems(toTransferAmount);
                destination.AddItems(draggingItem, toTransferAmount);
                return false;

            }

            return true;
        }
    }
}
