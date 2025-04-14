using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Loading : UI_Base
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    private NGO_LoadingManager loadingManager;

    public override void Init()
    {
        loadingManager = FindObjectOfType<NGO_LoadingManager>();
    }

    public void UpdateLoadingProgress(int progress)
    {
        loadingBar.value = progress;
        loadingText.text = $"{progress}%";
    }
} 