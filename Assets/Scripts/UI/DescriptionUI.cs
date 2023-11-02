using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(EventTrigger))]
public class DescriptionUI : MonoBehaviour {
    private EventTrigger _trigger;
    private string _descriptionText;
    private GameObject _popupWindow;
    private TMP_Text _popupWindowText;
    private void Awake() {
        InitializeEventTriggers();
    }

    private void InitializeEventTriggers() {
        _trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry startEntry = new EventTrigger.Entry();
        startEntry.eventID = EventTriggerType.PointerEnter;
        startEntry.callback.AddListener(OnDescriptionStart);

        EventTrigger.Entry endEntry = new EventTrigger.Entry();
        endEntry.eventID = EventTriggerType.PointerExit;
        endEntry.callback.AddListener(OnDescriptionEnd);

        _trigger.triggers.Add(startEntry);
        _trigger.triggers.Add(endEntry);
    }

    public void Initialize(GameObject popupWindow, string text) {
        _popupWindow = popupWindow;
        _popupWindowText = popupWindow.GetComponentInChildren<TMP_Text>();
        _descriptionText = text;
        _popupWindow.SetActive(false);
    }

    private void OnDescriptionStart(BaseEventData eventData) {
        _popupWindowText.text = _descriptionText;
        Vector3 windowPosition = transform.position;
        windowPosition.y += 20f;
        _popupWindow.transform.position = windowPosition;
        _popupWindow.SetActive(true);
    }

    private void OnDescriptionEnd(BaseEventData eventData) {
        _popupWindow.SetActive(false);
    }
}
