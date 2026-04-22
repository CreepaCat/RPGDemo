using TMPro;
using UnityEngine;
using RPGDemo.Stats;

public class PlayerStatsInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_level;
    [SerializeField] TextMeshProUGUI tmp_health;
    [SerializeField] TextMeshProUGUI tmp_magic;
    [SerializeField] TextMeshProUGUI tmp_atk;
    [SerializeField] TextMeshProUGUI tmp_def;


    Player player;

    private void Awake()
    {
        player = Player.GetInstance();
    }

    void Start()
    {
        DrawUI();
    }

    void Update()
    {
        //todo:当角色数值发生变化时才draw
        DrawUI();
    }

    void DrawUI()
    {
        tmp_level.text = "Lv." + player.BaseStats.CurrentLevel.ToString();

        tmp_health.text = ((int)player.BaseStats.GetStats(StatsType.Health)).ToString();
        tmp_magic.text = ((int)player.BaseStats.GetStats(StatsType.Mana)).ToString();
        tmp_atk.text = ((int)player.BaseStats.GetStats(StatsType.Attack)).ToString();
        tmp_def.text = ((int)player.BaseStats.GetStats(StatsType.Defense)).ToString();

    }
}
