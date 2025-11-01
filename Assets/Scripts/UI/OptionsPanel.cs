// OptionsPanel.cs (교체/보강)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private Slider masterVol;
    [SerializeField] private TMP_Dropdown quality;
    [SerializeField] private Toggle vSyncToggle;          // 선택: 토글 하나 추가
    [SerializeField] private Button btnClose;

    const string KEY_VOL = "opt_master_vol";
    const string KEY_QUAL = "opt_quality";
    const string KEY_VSYNC = "opt_vsync";

    void Start()
    {
        // 초기 UI 셋업
        quality.ClearOptions();
        quality.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));

        // 로드
        float vol = PlayerPrefs.GetFloat(KEY_VOL, 1f);
        int q = PlayerPrefs.GetInt(KEY_QUAL, QualitySettings.GetQualityLevel());
        int vs = PlayerPrefs.GetInt(KEY_VSYNC, 1);

        masterVol.SetValueWithoutNotify(vol);
        quality.SetValueWithoutNotify(Mathf.Clamp(q, 0, QualitySettings.names.Length - 1));
        if (vSyncToggle) vSyncToggle.SetIsOnWithoutNotify(vs == 1);

        // 즉시 적용
        ApplyVolume(vol);
        ApplyQuality(q);
        ApplyVSync(vs == 1);

        // 리스너
        masterVol.onValueChanged.AddListener(v => { ApplyVolume(v); PlayerPrefs.SetFloat(KEY_VOL, v); });
        quality.onValueChanged.AddListener(i => { ApplyQuality(i); PlayerPrefs.SetInt(KEY_QUAL, i); });
        if (vSyncToggle) vSyncToggle.onValueChanged.AddListener(on => { ApplyVSync(on); PlayerPrefs.SetInt(KEY_VSYNC, on ? 1 : 0); });

        btnClose.onClick.AddListener(() => gameObject.SetActive(false));
    }

    void ApplyVolume(float v) => AudioListener.volume = Mathf.Clamp01(v);
    void ApplyQuality(int i) { i = Mathf.Clamp(i, 0, QualitySettings.names.Length - 1); QualitySettings.SetQualityLevel(i, true); }
    void ApplyVSync(bool on) { QualitySettings.vSyncCount = on ? 1 : 0; }
}
