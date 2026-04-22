using System.Linq;
using RPGDemo.Inventories;
using RPGDemo.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RPGDemo.Shops
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ShopUI : BasePanel
    {
        [SerializeField] Button btnClose;
        [SerializeField] ShopSlotUI shopSlotUIPrefab;
        [SerializeField] Transform shopSlotUIRoot;
        [SerializeField] SelectorsRoot selectorsRoot;
        [SerializeField] Button btnSellMode;


        [Header("购买和出售面板")]
        [SerializeField] SellItemPanel sellItemPanel;
        [SerializeField] BuyItemPanel buyItemPanel;

        Shopper _playerShopper;
        ItemCategory currentItemCategory = ItemCategory.None;


        protected override void Awake()
        {
            base.Awake();

            _playerShopper = Player.GetInstance().GetComponent<Shopper>();

            btnClose.onClick.AddListener(CloseMe);
            btnSellMode.onClick.AddListener(ShowSellPanel);
        }
        private void OnEnable()
        {
            _playerShopper.OnShopChanged += HandleShopChanged;
        }
        private void OnDisable()
        {
            _playerShopper.OnShopChanged -= HandleShopChanged;
        }


        private void Start()
        {
            HideMe();
        }

        public void SetCurrentCategory(int categoryIndex)
        {
            ItemCategory newCategory = (ItemCategory)categoryIndex;
            if (currentItemCategory != newCategory)
            {
                currentItemCategory = newCategory;
                Redraw();
            }
        }

        public Shop GetShop()
        {
            return _playerShopper.GetCurrentShop();
        }

        public Shop.ShopItem GetCurrentSelectedItem()
        {
            return selectorsRoot.GetCurrentSelected().GetComponent<ShopSlotUI>().GetShopItem();
        }

        public void Sell(InventoryItem item, int amount)
        {
            _playerShopper.SellToShop(item, amount);
        }
        public bool Buy(Shop.ShopItem item, int amount)
        {
            return _playerShopper.BuyFromShop(item, amount);
        }

        public void ShowBuyPanel(Shop.ShopItem shopItem)
        {
            buyItemPanel.gameObject.SetActive(true);
            buyItemPanel.SetBuyItemQuantity(shopItem);

            sellItemPanel.SetSellItemQuantity(null);
            sellItemPanel.gameObject.SetActive(false);

        }
        public void ShowSellPanel()
        {
            buyItemPanel.SetBuyItemQuantity(null);
            buyItemPanel.gameObject.SetActive(false);

            sellItemPanel.gameObject.SetActive(true);
        }

        private void HandleShopChanged()
        {
            if (_playerShopper.GetCurrentShop() == null)
            {
                HideMe();

                return;
            }
            ShowMe();
        }

        private void Redraw()
        {
            //当不显示时不重绘UI
            if (canvasGroup.alpha < 0.1f)
            {
                return;
            }
            foreach (Transform child in shopSlotUIRoot)
            {
                Destroy(child.gameObject);
            }
            if (currentItemCategory == ItemCategory.None)
            {

                var shopItems = _playerShopper.GetCurrentShop().GetShopItems();
                for (int i = 0; i < shopItems.Count; i++)
                {
                    var slotUIObj = Instantiate(shopSlotUIPrefab, shopSlotUIRoot);
                    var slotUI = slotUIObj.GetComponent<ShopSlotUI>();
                    slotUI.Setup(shopItems[i], i, this);
                }
            }
            else
            {
                var filterItems = _playerShopper.GetCurrentShop().GetFilterItems(currentItemCategory).ToList();
                for (int i = 0; i < filterItems.Count; i++)
                {
                    var slotUIObj = Instantiate(shopSlotUIPrefab, shopSlotUIRoot);
                    var slotUI = slotUIObj.GetComponent<ShopSlotUI>();
                    slotUI.Setup(filterItems[i], i, this);
                }
            }


        }

        public void CloseMe()
        {
            CloseSelf();
            _playerShopper.InteractWithShop(null);
        }


        public void HideMe()
        {
            UIManager.Instance.ClosePanel<ShopUI>();
            _playerShopper.InteractWithShop(null);
        }

        public void ShowMe()
        {
            UIManager.Instance.OpenPanel<ShopUI>();
            Redraw();
        }


    }
}
