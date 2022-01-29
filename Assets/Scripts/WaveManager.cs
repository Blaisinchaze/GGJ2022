using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    #region Singleton stuff
    private static WaveManager _instance;
    public static WaveManager Instance
    {
        get { return _instance; }
    }
    #endregion

    int waveNum = 0;
    public GameObject prefab;
    [Space]
    public int enemiesAddedPerWave;
    public float durationAddedPerEnemy;
    public float minDuration;
    public float maxDuration;
    [Space]
    [SerializeField]
    private List<Transform> activeSpawners = new List<Transform>();


    private List<GameObject> enemies = new List<GameObject>();
    private float waveTimer;
    private List<int> indexHistory = new List<int>();

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
        waveTimer = 5;
        DoorManager.Instance.m_doorOpened.AddListener(AddDoorsSpawners);
    }

    // Update is called once per frame
    void Update()
    {
        if (waveTimer > 0)
        {
            waveTimer -= Time.deltaTime;
        }
        else
        {
            SpawnWave();
        }
    }

    public void SpawnWave()
    {
        for (int i = 0; i < Random.Range(waveNum, (waveNum * enemiesAddedPerWave)+1); i++)
        {
            GameObject newEnemy = Instantiate(prefab, transform);

            int index = Random.Range(0, activeSpawners.Count);

            while (indexHistory.Contains(index))
            {
                index = Random.Range(0, activeSpawners.Count);
            }

            indexHistory.Add(index);

            if (indexHistory.Count > activeSpawners.Count / 2)
            {
                indexHistory.RemoveAt(0);
            }

            newEnemy.transform.position = activeSpawners[index].position;

            enemies.Add(newEnemy);
            waveTimer += durationAddedPerEnemy;
        }

        waveTimer = Mathf.Clamp(waveTimer, minDuration, maxDuration);
        waveNum++;
    }

    public void AddDoorsSpawners(Door door)
    {
        foreach (Transform spawnPoint in door.spawnPoints)
        {
            if (activeSpawners.Contains(spawnPoint) == false)
            {
                activeSpawners.Add(spawnPoint);
            }
        }
    }
}
