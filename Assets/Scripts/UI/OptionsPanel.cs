// OptionsPanel.cs (��ü/����)

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionsPanel : MonoBehaviour
    {
        [SerializeField] private Slider masterVol;
        [SerializeField] private TMP_Dropdown quality;
        [SerializeField] private Toggle vSyncToggle;          // ����: ��� �ϳ� �߰�
        [SerializeField] private Button btnClose;

        const string KEY_VOL = "opt_master_vol";
        const string KEY_QUAL = "opt_quality";
        const string KEY_VSYNC = "opt_vsync";

        void Start()
        {
            // �ʱ� UI �¾�
            quality.ClearOptions();
            var customNames = new System.Collections.Generic.List<string> { "High", "Middle", "Low" };
            quality.AddOptions(customNames);

            // �ε�
            float vol = PlayerPrefs.GetFloat(KEY_VOL, 1f);
            int q = PlayerPrefs.GetInt(KEY_QUAL, QualitySettings.GetQualityLevel());
            int vs = PlayerPrefs.GetInt(KEY_VSYNC, 1);

            masterVol.SetValueWithoutNotify(vol);
            quality.SetValueWithoutNotify(Mathf.Clamp(q, 0, QualitySettings.names.Length - 1));
            if (vSyncToggle) vSyncToggle.SetIsOnWithoutNotify(vs == 1);

            // ��� ����
            ApplyVolume(vol);
            ApplyQuality(q);
            ApplyVSync(vs == 1);

            // ������
            masterVol.onValueChanged.AddListener(v => { ApplyVolume(v); PlayerPrefs.SetFloat(KEY_VOL, v); });
            quality.onValueChanged.AddListener(i => { ApplyQuality(i); PlayerPrefs.SetInt(KEY_QUAL, i); });
            if (vSyncToggle) vSyncToggle.onValueChanged.AddListener(on => { ApplyVSync(on); PlayerPrefs.SetInt(KEY_VSYNC, on ? 1 : 0); });

            btnClose.onClick.AddListener(() => gameObject.SetActive(false));
        }

        void ApplyVolume(float v) => AudioListener.volume = Mathf.Clamp01(v);
        void ApplyQuality(int i) { i = Mathf.Clamp(i, 0, QualitySettings.names.Length - 1); QualitySettings.SetQualityLevel(i, true); }
        void ApplyVSync(bool on) { QualitySettings.vSyncCount = on ? 1 : 0; }
    }
}
