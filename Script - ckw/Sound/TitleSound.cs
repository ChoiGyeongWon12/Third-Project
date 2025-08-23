using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleSound : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] AudioSource audiosource;

    void Start()
    {
        audiosource.loop = true;

        // 저장된 BGM 볼륨 값 로드
        float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        slider.SetValueWithoutNotify(savedVolume);
        audiosource.volume = savedVolume;

        // 슬라이더 값 변경 시 저장
        slider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        audiosource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }
}
