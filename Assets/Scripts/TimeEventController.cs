using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TimedEventInvoker : MonoBehaviour
{
    [SerializeField] private UnityEvent[] timeEvent;
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool loop = true;
    private Coroutine runningCoroutine;

    private void Start()
    {
        if (duration > 0f && timeEvent != null && timeEvent.Length > 0)
        {
            runningCoroutine = StartCoroutine(EventLogic());
        }
        else
        {
            Debug.LogWarning("Duration must be > 0 and at least one event assigned.");
        }
    }

    private IEnumerator EventLogic()
    {
        do
        {
            for (int i = 0; i < timeEvent.Length; i++)
            {
                yield return new WaitForSeconds(duration);
                timeEvent[i]?.Invoke();
            }
        } while (loop);
    }

    public void StopInvoking()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    public void Restart()
    {
        StopInvoking();
        runningCoroutine = StartCoroutine(EventLogic());
    }
}
