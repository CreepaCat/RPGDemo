using TMPro;
using Unity.VisualScripting;
using UnityEngine;


namespace RPGDemo.Inventories
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txt_money;

        Purse _playerPurse;

        private void Awake()
        {
            _playerPurse = Purse.GetPlayerPurse();
        }
        private void OnEnable()
        {
            _playerPurse.OnBalanceUpdated += UpdateMoneyText;
        }
        private void OnDisable()
        {
            _playerPurse.OnBalanceUpdated -= UpdateMoneyText;
        }
        private void Start()
        {
            UpdateMoneyText();
        }

        void UpdateMoneyText()
        {
            txt_money.text = Mathf.RoundToInt(_playerPurse.GetBalance()).ToString();
        }

    }
}
