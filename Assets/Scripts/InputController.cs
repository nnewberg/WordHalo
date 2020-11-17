using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class InputController : MonoBehaviour
{
    public Button ExplodeButton;
    public EventTrigger ExplodeButtonTrigger;
    public InputField TextInput;

    private EventTrigger.Entry buttonUp;
    private EventTrigger.Entry buttonDown;
    private BlendShapeRouter blendShapeRouter;

    public void Awake()
    {
        buttonDown = new EventTrigger.Entry();
        buttonUp = new EventTrigger.Entry();

        buttonDown.eventID = EventTriggerType.PointerDown;
        buttonUp.eventID = EventTriggerType.PointerUp;
        ExplodeButtonTrigger.triggers.Add(buttonDown);
        ExplodeButtonTrigger.triggers.Add(buttonUp);
    }

    public void Start()
    {
        blendShapeRouter = FindObjectOfType<BlendShapeRouter>();
    }

    public void AddExplodeButtonDownEvent(UnityAction<BaseEventData> triggerEvent) {
        buttonDown.callback.AddListener(triggerEvent);
    }

    public void AddExplodeButtonUpEvent(UnityAction<BaseEventData> triggerEvent)
    {
        buttonUp.callback.AddListener(triggerEvent);
    }

    public float NormalizedBrowCoefficient
    {
        get
        {
            if (blendShapeRouter == null) {
                blendShapeRouter = FindObjectOfType<BlendShapeRouter>();
                return 0f;
            }
            return Mathf.Clamp01(blendShapeRouter.BrowCoefficient / 60f);
        }
    }

    public string Text
    {
        get
        {
            return TextInput.text;
        }

        set
        {
            TextInput.text = value;
        }
    }

}
