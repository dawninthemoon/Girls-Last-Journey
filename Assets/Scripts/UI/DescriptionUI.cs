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
    private static readonly string PopupWindowPrefabName = "TextPopupWindow";
    private void Awake() {
        AssetLoader.Instance.LoadAssetAsync<GameObject>(PopupWindowPrefabName, (op) => {
            _popupWindow = Instantiate(op.Result);
            _popupWindowText = _popupWindow.GetComponentInChildren<TMP_Text>();
            _popupWindow.SetActive(false);
        });
        InitializeEventTriggers();
    }

    private void InitializeEventTriggers() {
        _trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry startEntry = new EventTrigger.Entry();
        startEntry.eventID = EventTriggerType.PointerEnter;
        startEntry.callback.AddListener(OnDescriptionStart);

        EventTrigger.Entry endEntry = new EventTrigger.Entry();
        endEntry.eventID = EventTriggerType.PointerExit;
        endEntry.callback.AddListener(OnDescriptionStart);

        _trigger.triggers.Add(startEntry);
        _trigger.triggers.Add(endEntry);
    }

    public void SetDescriptionText(string text) {
        _descriptionText = text;
    }

    private void OnDescriptionStart(BaseEventData eventData) {
        _popupWindowText.text = _descriptionText;
        _popupWindow.SetActive(false);
    }

    private void OnDescriptionEnd(BaseEventData eventData) {
        _popupWindow.SetActive(false);
    }
}
