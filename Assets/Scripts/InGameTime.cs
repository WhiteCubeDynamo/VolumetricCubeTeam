using UnityEngine;
using System.Collections;
using TMPro;
public class InGameTime : MonoBehaviour
{
    public int hour = 0;
    public int minute = 0;
    public int day = 1;
    public bool debugToggled = false;

    //text
    public TMP_Text daytext;
    public TMP_Text hourtext;
    
    public void Start()
    {
        StartCoroutine(time());
    }
    public void OnReset()
    {
        minute += 1;
        if (minute == 30)
        {
            hour += 1;
            minute = 0;
        }
        if (hour > 24)
        {
            hour = 0;
            minute = 0;
            day += 1;
        }
        if (debugToggled)
        {
            Debug.Log("hour: " + hour + ", minute: " + minute);
        }

        hourtext.SetText(hour.ToString() + ": " + minute * 2);
        daytext.SetText("Time: "+day.ToString());
        

        StartCoroutine(time());
    }
    IEnumerator time()
    {
        yield return new WaitForSeconds(1);
        OnReset();
    }

}
