using System.Collections.Generic;
using Core.AudioSystem;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;
using RPGDemo.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : BasePanel
{
    [SerializeField] Slider slider_sound;
    [SerializeField] Slider slider_music;
    [SerializeField] TextMeshProUGUI txt_soundAmount;
    [SerializeField] TextMeshProUGUI txt_musicAmount;
    [SerializeField] Button btn_close;

    private void Start()
    {

        slider_sound.onValueChanged.AddListener((v) =>
        {
            txt_soundAmount.text = Mathf.RoundToInt(v * 100).ToString();
            ApplyValue();
        });

        slider_music.onValueChanged.AddListener((v) =>
       {
           txt_musicAmount.text = Mathf.RoundToInt(v * 100).ToString();
           ApplyValue();
       });

        slider_sound.value = SoundManager.Instance.Volume;
        slider_music.value = MusicManager.Instance.Volume;

        btn_close.onClick.AddListener(CloseMe);

        HideMe();
    }

    public void CloseMe()
    {
        CloseSelf();
    }

    public void ShowMe()
    {
        UIManager.Instance.OpenPanel<SettingsPanel>();
    }

    public void HideMe()
    {
        UIManager.Instance.ClosePanel<SettingsPanel>();
    }

    private void ApplyValue()
    {
        SoundManager.Instance.Volume = slider_sound.value;
        MusicManager.Instance.Volume = slider_music.value;
    }

    //todo:声音面板设置用注册表来存档，避免与游戏存档冲突
}
