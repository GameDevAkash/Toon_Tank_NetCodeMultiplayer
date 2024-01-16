using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] private float timeLeft = 2f;

    // Update is called once per frame
    void Update()
    {
        if (timeLeft < 0)
        {
            Destroy(gameObject);
            timeLeft = 2f;
        }
        timeLeft -= Time.deltaTime;
    }
}
