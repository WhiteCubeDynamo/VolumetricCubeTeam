using UnityEngine;
using TMPro;

public class EndScreenMessage : MonoBehaviour
{
    [TextArea]
    public string[] messages = new string[]
    {
        "To break the loop is to become it.",
        "One climb closer to forgetting why you started.",
        "You left the castle. But not the loop.",
        "You took from the castle, but what did it take from you?",
        "Freedom is just another loop.",
        "Did you break the loop, or did you become the loop?",
        "You adapted, but the loop stayed the same."
    };

    private TextMeshProUGUI tmpText;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false); // najprej skrito
    }

    public void Activate()
    {
        gameObject.SetActive(true); // prikaži objekt (če je bil skrit)
        if (tmpText != null)
        {
            tmpText.text = GetRandomMessage();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component not found.");
        }
    }

    string GetRandomMessage()
    {
        if (messages.Length == 0) return "";
        int index = Random.Range(0, messages.Length);
        return messages[index];
    }
}
