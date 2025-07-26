using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    private void Update()
    {
        float fps = 1f / Time.unscaledDeltaTime;
        fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";
    }
}
