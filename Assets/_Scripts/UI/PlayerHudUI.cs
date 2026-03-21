using System;
using UnityEngine;
using UnityEngine.UI;
using RPGDemo.Attributes;

namespace RPGDemo.UI
{


    public class PlayerHudUI : MonoBehaviour
    {
        [SerializeField] Image healthBarFill;
        [SerializeField] Image manaBarFill;

        Health playerHealth;
        Mana playerMana;

        private void Awake()
        {
            var player = Player.GetInstance();
            playerHealth = player.GetComponent<Health>();
            playerMana = player.GetComponent<Mana>();
        }

        private void OnEnable()
        {
            playerHealth.OnHealthChanged += UpdateBarsUI;
            playerMana.OnManaChanged += UpdateBarsUI;
        }

        private void OnDisable()
        {
            playerHealth.OnHealthChanged -= UpdateBarsUI;
            playerMana.OnManaChanged -= UpdateBarsUI;
        }

        private void Start()
        {
            UpdateBarsUI();
        }

        [ContextMenu("TestTakeDamage")]
        public void TestTakeDamage()
        {
            playerHealth.TakeDamage(10);
        }

        [ContextMenu("TestCostMana")]
        public void TestCostMana()
        {
            playerMana.TryUseMana(10);
        }

        private void UpdateBarsUI(float healthChangedValue = 0)
        {
            healthBarFill.fillAmount = playerHealth.GetHealthRatio();
            manaBarFill.fillAmount = playerMana.GetManaRatio();

        }
    }
}
