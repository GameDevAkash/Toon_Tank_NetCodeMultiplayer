using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Coin>(out Coin coin))
        {
            int value = coin.Collect();
            if (!IsServer) { return; }
            if (IsServer) { TotalCoins.Value += value; }
        }
    }
}
