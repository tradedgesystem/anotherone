using UnityEngine;
using UnityEngine.UI;

public class RaceHUD : MonoBehaviour
{
    private Text _lapText;
    private Text _positionText;
    private Text _lapTimeText;
    private Text _countdownText;
    private Text _instructionsText;
    private Text _finishText;

    private Canvas _canvas;

    private void Awake()
    {
        BuildCanvas();
    }

    private void BuildCanvas()
    {
        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        _lapText = CreateLabel("LapText", new Vector2(10, -10), TextAnchor.UpperLeft, 18);
        _positionText = CreateLabel("PositionText", new Vector2(-10, -10), TextAnchor.UpperRight, 18);
        _countdownText = CreateLabel("CountdownText", Vector2.zero, TextAnchor.UpperCenter, 32);
        _lapTimeText = CreateLabel("LapTimeText", new Vector2(10, 40), TextAnchor.LowerLeft, 16);
        _instructionsText = CreateLabel("Instructions", new Vector2(0, 30), TextAnchor.LowerCenter, 14);
        _finishText = CreateLabel("FinishText", Vector2.zero, TextAnchor.MiddleCenter, 26);

        _instructionsText.text = "WASD / Arrows: steer & throttle | Space: brake | Shift: boost | R: restart | Esc: quit";
        _finishText.text = string.Empty;
    }

    private Text CreateLabel(string name, Vector2 offset, TextAnchor anchor, int size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(_canvas.transform);
        var text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = anchor;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = AnchorFromAnchor(anchor);
        rect.anchoredPosition = offset;
        rect.sizeDelta = new Vector2(800, 60);
        rect.pivot = new Vector2(0.5f, 0.5f);
        return text;
    }

    private Vector2 AnchorFromAnchor(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.UpperLeft: return new Vector2(0f, 1f);
            case TextAnchor.UpperRight: return new Vector2(1f, 1f);
            case TextAnchor.UpperCenter: return new Vector2(0.5f, 1f);
            case TextAnchor.LowerLeft: return new Vector2(0f, 0f);
            case TextAnchor.LowerCenter: return new Vector2(0.5f, 0f);
            case TextAnchor.MiddleCenter: return new Vector2(0.5f, 0.5f);
            default: return new Vector2(0.5f, 0.5f);
        }
    }

    public void SetLapInfo(int current, int total)
    {
        if (_lapText != null)
        {
            _lapText.text = $"Lap {Mathf.Clamp(current, 1, total)} / {total}";
        }
    }

    public void SetPosition(int current, int totalRacers)
    {
        if (_positionText != null)
        {
            _positionText.text = $"P{current} / {totalRacers}";
        }
    }

    public void SetLapTimes(float currentLap, float bestLap)
    {
        if (_lapTimeText != null)
        {
            string bestText = bestLap > 0 ? bestLap.ToString("F2") : "--";
            _lapTimeText.text = $"Lap Time: {currentLap:F2}s | Best: {bestText}s";
        }
    }

    public void ShowCountdownText(string text)
    {
        if (_countdownText != null)
        {
            _countdownText.text = text;
        }
    }

    public void ShowRaceFinished(string resultText)
    {
        if (_finishText != null)
        {
            _finishText.text = resultText;
        }
    }
}
