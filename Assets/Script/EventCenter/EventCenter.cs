using System;
using System.Collections.Generic;

public class EventCenter
{
    private static Dictionary<string, List<Action<object[]>>> eventListeners = new Dictionary<string, List<Action<object[]>>>();

    public static void Send(string eventStr, object[] args = null)
    {
        if (!string.IsNullOrEmpty(eventStr))
        {
            if (eventListeners.ContainsKey(eventStr))
            {
                List<Action<object[]>> listeners = eventListeners[eventStr];

                Action<object[]> listener = null;
                for (int i = listeners.Count - 1; i >= 0; i--)
                {
                    listener = listeners[i];
                    if (listener != null && listener.Target != null)
                    {
                        listener(args);
                    }
                    else {
                        listeners.RemoveAt(i);
                    }
                }
            }
        }
    }

    public static void AddListener(string eventStr, Action<object[]> listener)
    {
        if (!string.IsNullOrEmpty(eventStr))
        {
            List<Action<object[]>> listeners = GetOrCreateListeners(eventStr);
            listeners.Add(listener);
        }
    }

    public static void RemoveListener(string eventStr, Action<object[]> listener)
    {
        if (!string.IsNullOrEmpty(eventStr))
        {
            List<Action<object[]>> listeners = GetOrCreateListeners(eventStr);
            listeners.Remove(listener);
        }
    }

    public static void RemoveListeners(object obj)
    {
        if (obj != null)
        {
            foreach (List<Action<object[]>> listeners in eventListeners.Values)
            {
                Action<object[]> listener = null;
                for (int i = listeners.Count - 1; i >= 0; i--)
                {
                    listener = listeners[i];
                    if (listener == null || listener.Target == obj)
                    {
                        listeners.RemoveAt(i);
                    }
                }
            }
        }
    }

    private static List<Action<object[]>> GetOrCreateListeners(string eventStr)
    {
        List<Action<object[]>> listeners = null;
        if (!eventListeners.ContainsKey(eventStr))
        {
            listeners = new List<Action<object[]>>();
            eventListeners.Add(eventStr, listeners);
        }
        else {
            listeners = eventListeners[eventStr];
        }

        return listeners;
    }
}
