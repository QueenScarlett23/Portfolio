using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<EVENT_TYPE, Action<Dictionary<string, object>>> eventDictionary;

    private static EventManager eventManager;

    public static EventManager Instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();

                    //  Sets this to not be destroyed when reloading scene
                    DontDestroyOnLoad(eventManager);
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<EVENT_TYPE, Action<Dictionary<string, object>>>();
        }
    }

    public void StartListening(EVENT_TYPE eventType, Action<Dictionary<string, object>> listener)
    {
        Action<Dictionary<string, object>> thisEvent;

        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent += listener;
            Instance.eventDictionary[eventType] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            Instance.eventDictionary.Add(eventType, thisEvent);
        }
    }

    public void StopListening(EVENT_TYPE eventType, Action<Dictionary<string, object>> listener)
    {
        if (eventManager == null) return;
        Action<Dictionary<string, object>> thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent -= listener;
            Instance.eventDictionary[eventType] = thisEvent;
        }
    }

    public void TriggerEvent(EVENT_TYPE eventType, Dictionary<string, object> message = null)
    {
        Action<Dictionary<string, object>> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.Invoke(message);
        }
    }
}