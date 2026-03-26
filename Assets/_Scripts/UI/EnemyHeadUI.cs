using RPGDemo.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHeadUI : MonoBehaviour
{

    [SerializeField] Image image_healthBarFill = null;
    [SerializeField] TextMeshProUGUI tmp_healthNumber = null;
    Enemy enemy;
    Health health;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        health = enemy.GetComponent<Health>();
    }
    private void Start()
    {
        UpdateUI(0);
    }

    private void OnEnable()
    {
        health.OnHealthChanged += UpdateUI;
        health.OnDeath += HideMe;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= UpdateUI;
        health.OnDeath -= HideMe;
    }


    private void UpdateUI(float v)
    {

        image_healthBarFill.fillAmount = health.GetHealthRatio();
        tmp_healthNumber.text = $"{health.GetCurrentHealth()}/{health.GetMaxHealth()}";

    }

    private void ShowMe()
    {
        //todo:用CanvasGroup控制显示隐藏
    }
    private void HideMe()
    {
        gameObject.SetActive(false);
    }
}
