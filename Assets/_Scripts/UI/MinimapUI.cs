using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    public RawImage mapImage;
    public Image arrow;
    public TextMeshProUGUI mapName;

    //public Collider[] minimapBondingBoxs;
    [SerializeField]
    private Collider minimapBondingBox;
    //public Transform[] minimapBoxTrans;

    // int currMapId;

    Transform playerTransform;
    bool playerAdded;
    Quaternion arrrot;

    [Header("Minimap Boundary")]
    [SerializeField, Range(0f, 1f)]
    private float mapPivotMin = 0.2f;
    [SerializeField, Range(0f, 1f)]
    private float mapPivotMax = 0.8f;
    [SerializeField]
    private float arrowPadding = 8f;
    // Use this for initialization
    void Start()
    {
        MinimapManager.Instance.uimp = this;
        this.UpdateMap();

    }

    public void UpdateMap()
    {
        // if (Player.GetInstance.CurrentMapData != null)
        // {
        //     this.currMapId = User.Instance.CurrentMapData.ID;
        //     this.mapName.text = User.Instance.CurrentMapData.Name;

        //覆盖Img图片的方法，overrideSprite和sprite是同一种东西的不同用法
        //如果原先是overrideSprite则可覆盖
        // this.mapImage.overrideSprite = MinimapManager.Instance.LoadMinimap();


        //设置小地图默认数据
        this.mapImage.SetNativeSize();//使用原始大小
        mapImage.transform.localPosition = Vector3.zero;

        this.minimapBondingBox = MinimapManager.Instance.MinimapBondingBox;//包围盒由manager提供，每个模块只专注于自己的责任

        //唯一赋值模式
        if (this.playerTransform == null)
        {
            this.playerTransform = MinimapManager.Instance.PlayerTransform;
            // Debug.Log("当前游戏物体：" + User.Instance.CurrentCharacterObject.transform.name);
            return;
        }

        //}


    }


    void Update()
    {
        //MiniMap实时更新,那么在何时加载图片？
        if (this.minimapBondingBox != null && this.playerTransform != null)
        {
            //	Debug.Log("box:"+minimapBondingBox.transform.name);
            //可通过collider获取几何体的尺寸
            float realWidth = minimapBondingBox.bounds.size.x;

            float realHeight = minimapBondingBox.bounds.size.z;//因为是俯视的collider,纵向为z轴
                                                               //	Debug.LogFormat("realWidth:{0} realHeight:{1}", realWidth, realHeight);
                                                               //计算偏移
            float realX = playerTransform.position.x - minimapBondingBox.bounds.min.x;
            float realY = playerTransform.position.z - minimapBondingBox.bounds.min.z;//合起来为相对collider原点偏移

            //Debug.LogFormat("realX:{0} realY:{1}", realX, realY);

            //按比例换算成中心点位置
            float pivotX = realX / realWidth;
            float pivotY = realY / realHeight;
            //Debug.LogFormat("pivotX:{0} pivotY:{1}" ,pivotX, pivotY);

            float minPivot = Mathf.Min(mapPivotMin, mapPivotMax);
            float maxPivot = Mathf.Max(mapPivotMin, mapPivotMax);

            float clampedPivotX = Mathf.Clamp(pivotX, minPivot, maxPivot);
            float clampedPivotY = Mathf.Clamp(pivotY, minPivot, maxPivot);

            mapImage.rectTransform.pivot = new Vector2(clampedPivotX, clampedPivotY);
            mapImage.transform.localPosition = Vector3.zero;//本地坐标设0要放在设中心点之后

            // 超出边界后，地图不再移动，把超出量转换为箭头偏移
            float overflowX = pivotX - clampedPivotX;
            float overflowY = pivotY - clampedPivotY;

            Vector2 arrowOffset = new Vector2(
                overflowX * mapImage.rectTransform.rect.width - arrowPadding,
                overflowY * mapImage.rectTransform.rect.height - arrowPadding
            );

            RectTransform parentRect = arrow.rectTransform.parent as RectTransform;
            if (parentRect != null)
            {
                float halfW = parentRect.rect.width * 0.5f + arrowPadding;
                float halfH = parentRect.rect.height * 0.5f + arrowPadding;
                arrowOffset.x = Mathf.Clamp(arrowOffset.x, -halfW, halfW);
                arrowOffset.y = Mathf.Clamp(arrowOffset.y, -halfH, halfH);
            }

            arrow.rectTransform.localPosition = arrowOffset;

            //箭头旋转
            //  this.arrrot.eulerAngles = new Vector3(0, 0, -this.playerTransform.eulerAngles.y + 90f);
            this.arrrot.eulerAngles = new Vector3(0, 0, -this.playerTransform.eulerAngles.y);
            this.arrow.rectTransform.localRotation = arrrot;
        }

    }
}
