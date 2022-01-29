using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorManager : MonoBehaviour
{
    public Door[] allDoors = new Door[1];

    public DoorEvent m_doorOpened = new DoorEvent();

    #region Singleton stuff
    private static DoorManager _instance;
    public static DoorManager Instance
    {
        get { return _instance; }
    }
    #endregion

    private void Awake()
    {
        #region Singleton stuff
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        allDoors = FindObjectsOfType<Door>();
        foreach (Door item in allDoors)
        {
            item.isOpen = false;
        }
    }
}

[System.Serializable]
public class DoorEvent : UnityEvent<Door>
{ }
