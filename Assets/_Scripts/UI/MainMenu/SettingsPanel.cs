using Core.AudioSystem;
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
            SoundManager.Instance.Volume = slider_sound.value;
            //ApplyValue();
        });

        slider_music.onValueChanged.AddListener((v) =>
       {
           txt_musicAmount.text = Mathf.RoundToInt(v * 100).ToString();
           MusicManager.Instance.Volume = slider_music.value;
           // ApplyValue();
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
        slider_sound.value = SoundManager.Instance.Volume;
        slider_music.value = MusicManager.Instance.Volume;
    }

    public void HideMe()
    {
        UIManager.Instance.ClosePanel<SettingsPanel>();
    }

}
