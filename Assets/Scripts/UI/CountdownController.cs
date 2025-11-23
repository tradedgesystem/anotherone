using System.Collections;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    private int _seconds;
    private System.Action _onFinished;
    private System.Action<string> _onTick;

    public void Init(int seconds, System.Action onFinished, System.Action<string> onTick)
    {
        _seconds = Mathf.Max(1, seconds);
        _onFinished = onFinished;
        _onTick = onTick;
        StartCoroutine(RunCountdown());
    }

    private IEnumerator RunCountdown()
    {
        for (int i = _seconds; i > 0; i--)
        {
            _onTick?.Invoke(i.ToString());
            yield return new WaitForSeconds(1f);
        }
        _onTick?.Invoke("GO!");
        yield return new WaitForSeconds(0.5f);
        _onTick?.Invoke(string.Empty);
        _onFinished?.Invoke();
    }
}
