using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]

    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) { return; }
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }
    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        healthBarImage.fillAmount = (float)newHealth / health.MaxHealth;
    }
}
