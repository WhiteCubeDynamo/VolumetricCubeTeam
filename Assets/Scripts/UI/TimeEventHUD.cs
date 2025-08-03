using UnityEngine;
using UnityEngine.UIElements;

public class TimeEventHUD : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private TimedEventInvoker timedEventInvoker;
    private string[] eventNames;

    private Label eventNameLabel;
    private Label timeEventLabel;
    private VisualElement hudContainer;

    private int currentEventIndex = -1;
    private float eventTimer = 0f;
    private bool isTracking = false;

    private void Start()
    {
        InitializeUI();

        if (timedEventInvoker != null)
        {
            // Extract event names from UnityEvent array
            ExtractEventNames();
            StartTracking();
        }
        else
        {
            Debug.LogWarning("TimedEventInvoker reference is not set in TimeEventHUD");
        }
    }

    private void InitializeUI()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        if (uiDocument?.rootVisualElement != null)
        {
            hudContainer = uiDocument.rootVisualElement.Q<VisualElement>("hud-container");
            timeEventLabel = uiDocument.rootVisualElement.Q<Label>("time-event-label");
            eventNameLabel = uiDocument.rootVisualElement.Q<Label>("event-name");

            UpdateEventDisplay("Waiting for events...");
        }
        else
        {
            Debug.LogError("UIDocument or root visual element not found");
        }
    }

    private void Update()
    {
        if (isTracking && timedEventInvoker != null)
        {
            TrackEventProgress();
        }
    }

    private void TrackEventProgress()
    {
        // Get the duration from the TimedEventInvoker through reflection or public property
        // Since the duration field is private, we'll estimate based on timing
        eventTimer += Time.deltaTime;

        // Estimate which event should be active based on duration
        // This is a simplified approach - in practice, you might want to modify TimedEventInvoker
        // to expose current event information
        float estimatedDuration = GetEstimatedDuration();

        if (estimatedDuration > 0)
        {
            int estimatedEventIndex = Mathf.FloorToInt(eventTimer / estimatedDuration) % GetEventCount();

            if (estimatedEventIndex != currentEventIndex)
            {
                currentEventIndex = estimatedEventIndex;
                UpdateCurrentEvent();
            }

            float timeUntilNext = estimatedDuration - (eventTimer % estimatedDuration);
            UpdateEventDisplay($"Next in: {timeUntilNext:F1}s");
        }
    }

    private float GetEstimatedDuration()
    {
        return timedEventInvoker.duration;
    }

    private int GetEventCount()
    {
        return eventNames.Length;
    }

    private void UpdateCurrentEvent()
    {
        if (currentEventIndex >= 0 && currentEventIndex < eventNames.Length)
        {
            string eventName = eventNames[currentEventIndex];
            UpdateEventName(eventName);
        }
    }

    private void UpdateEventDisplay(string message)
    {
        if (timeEventLabel != null)
        {
            timeEventLabel.text = $"Time Event: {message}";
        }
    }

    private void UpdateEventName(string eventName)
    {
        if (eventNameLabel != null)
        {
            eventNameLabel.text = eventName;
        }
    }

    public void StartTracking()
    {
        isTracking = true;
        eventTimer = 0f;
        currentEventIndex = -1;
        UpdateEventDisplay("Tracking started...");
    }

    public void StopTracking()
    {
        isTracking = false;
        UpdateEventDisplay("Tracking stopped");
        UpdateEventName("No event");
    }

    public void SetEventNames(string[] newEventNames)
    {
        eventNames = newEventNames;
    }

    public void SetTimedEventInvoker(TimedEventInvoker invoker)
    {
        timedEventInvoker = invoker;
        if (invoker != null)
        {
            ExtractEventNames();
            if (isTracking)
            {
                StartTracking();
            }
        }
    }

    // Method to be called when an event is triggered (you can call this from TimedEventInvoker)
    public void OnEventTriggered(int eventIndex)
    {
        if (eventIndex >= 0 && eventIndex < eventNames.Length)
        {
            currentEventIndex = eventIndex;
            UpdateEventName(eventNames[eventIndex]);
            UpdateEventDisplay("Event active!");
        }
    }

    // Toggle HUD visibility
    public void ToggleHUD()
    {
        if (hudContainer != null)
        {
            hudContainer.style.display = hudContainer.style.display == DisplayStyle.None ?
                DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void ExtractEventNames()
    {
        if (timedEventInvoker == null || timedEventInvoker.timeEvent == null)
        {
            eventNames = new string[0];
            return;
        }

        eventNames = new string[timedEventInvoker.timeEvent.Length];
        
        for (int i = 0; i < timedEventInvoker.timeEvent.Length; i++)
        {
            var unityEvent = timedEventInvoker.timeEvent[i];
            string eventName = "None"; // Default name
            
            // Try to extract meaningful name from the UnityEvent
            if (unityEvent != null)
            {
                // Get the persistent event count
                int eventCount = unityEvent.GetPersistentEventCount();
                
                if (eventCount > 0)
                {
                    // Get the first persistent call's target and method
                    var target = unityEvent.GetPersistentTarget(0);
                    string methodName = unityEvent.GetPersistentMethodName(0);
                    
                    if (target != null && !string.IsNullOrEmpty(methodName))
                    {
                        // Create a meaningful name from target and method
                        string targetName = target.name;
                        eventName = $"{targetName}.{methodName}";
                    }
                    else if (target != null)
                    {
                        eventName = target.name;
                    }
                    else if (!string.IsNullOrEmpty(methodName))
                    {
                        eventName = methodName;
                    }
                }
            }
            
            eventNames[i] = eventName;
        }
    }

    private void OnEnable()
    {
        if (uiDocument?.rootVisualElement != null)
        {
            InitializeUI();
        }
    }
}
