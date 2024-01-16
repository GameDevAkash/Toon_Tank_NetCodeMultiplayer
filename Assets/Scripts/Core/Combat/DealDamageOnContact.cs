using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    private ulong ownerClientID;

    public void SetOwner(ulong ownerClientID)
    {
        this.ownerClientID = ownerClientID;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody!= null)
        {
            if(collision.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
            {
                if(ownerClientID == netObj.OwnerClientId)
                {
                    return;
                }
            }
            if(collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damage);
            }
        }
    }
}
