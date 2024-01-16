using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class JoinServer : MonoBehaviour
{
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }
}
