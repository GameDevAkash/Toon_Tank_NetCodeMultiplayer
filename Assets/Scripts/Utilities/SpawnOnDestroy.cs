using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject prefabDust;

    private void OnDestroy()
    {
        Instantiate(prefabDust, transform.position, Quaternion.identity);
    }
}
