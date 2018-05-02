using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonLerpIncreaser : MonoBehaviour, IPointerDownHandler, IPointerUpHandler//, IPointerExitHandler
{
    public float startInterval;
    [Range(0.4f, 0.99f)]
    public float lerpSpeed;
    [Range(0.02f, 0.99f)]
    public float minInterval;

    private Button myButton;

    private void Awake()
    {
        if (!myButton)
            myButton = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine("ValueChanger");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine("ValueChanger");
    }

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    isEndOfChanging = true;
    //}

    IEnumerator ValueChanger()
    {
        float interval = startInterval;
        while (true)
        {
            yield return new WaitForSecondsRealtime(interval);
            if (myButton.onClick != null)
            {
                myButton.onClick.Invoke();
                interval *= lerpSpeed;
                if (interval < minInterval)
                {
                    interval = minInterval;
                }
            }
        }
    }
}
