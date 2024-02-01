using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    public HostGameManager GameManager { get; private set; }
    public static HostSingleton Instance 
    { 
        get 
        {
            if(instance != null) { return instance; }

            instance = FindObjectOfType<HostSingleton>();
            if (instance == null)
            {
                Debug.LogError("No HostSingleton in the scene !");
                return null;
            }
            return instance;
        } 
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
    private void OnDestroy()
    {
        GameManager.Dispose();
    }
}
