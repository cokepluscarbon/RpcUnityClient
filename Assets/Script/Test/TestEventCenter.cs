using UnityEngine;
using System.Collections;

public class TestEventCenter : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        EventCenter.AddListener("test_event_1", OnTestEvent1);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EventCenter.Send("test_event_1", new object[] { 1, 2, 3, new Random() });
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            EventCenter.RemoveListener("test_event_1", OnTestEvent1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            EventCenter.RemoveListeners(this);
        }
    }


    private void OnTestEvent1(object[] args)
    {
        Debug.Log("test_event_1 args = " + args.Length);
    }
}
