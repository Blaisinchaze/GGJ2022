using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Face
{
    NEUTRAL = 0,
    ATTACKING = 1,
    IN_RANGE = 2,
    DRAINED = 3,
    DAMAGED = 4,
}

public class EnemyFaceUpdate : MonoBehaviour
{
    public float minChangeDelay;
    public Enemy enemy;
    [Space]
    public Sprite[] sprites;
    private float changeTimer;
    internal Queue<Face> faceQueue = new Queue<Face>();
    private SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        faceQueue.Enqueue(Face.NEUTRAL);
    }

    // Update is called once per frame
    void Update()
    {
        if (changeTimer <= 0)
        {
            if (faceQueue.Count > 0)
            {
                faceQueue.Dequeue();
                changeTimer = minChangeDelay;
            }
        }
        else
        {
            changeTimer -= Time.deltaTime;
        }

        if (faceQueue.Count == 0)
        {
            faceQueue.Enqueue(Face.NEUTRAL);
        }

        rend.sprite = sprites[(int)faceQueue.Peek()];
    }

    public void QueueFace(Face face)
    {
        faceQueue.Enqueue(face);

        while (faceQueue.Count > 3)
        {
            faceQueue.Dequeue();
        }
    }
}
