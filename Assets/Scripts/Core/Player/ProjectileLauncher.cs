using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private Transform ProjectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject ClientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private bool shouldFire;
    [SerializeField] private float timer;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;

    //private float previousFireTime;
    private float muzzleFlashTimer;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

   
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    void Update()
    {
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }
        if (!IsOwner) { return; }
        if(timer > 0)
            timer -= Time.deltaTime;
        if (!shouldFire) { return; }
        if (timer > 0) { return; }
        if (wallet.TotalCoins.Value < costToFire) { return; }

        PrimaryFireServerRPC(ProjectileSpawnPoint.position, ProjectileSpawnPoint.up);
        SpawnDummyProjectile(ProjectileSpawnPoint.position, ProjectileSpawnPoint.up);

        timer = 1/fireRate;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;
        GameObject ProjectileInstance =  Instantiate(ClientProjectilePrefab, spawnPos, Quaternion.identity);
        ProjectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, ProjectileInstance.GetComponent<Collider2D>());

        if(ProjectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * ProjectileSpeed;
        }
    }

    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 spawnPos, Vector3 direction)
    {
        if (wallet.TotalCoins.Value < costToFire) { return; }
        wallet.SpendCoins(costToFire);
        GameObject ProjectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
        ProjectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, ProjectileInstance.GetComponent<Collider2D>());
        if(ProjectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }
        if (ProjectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * ProjectileSpeed;
        }
        SpawnDummyProjectileClientRPC(spawnPos, direction);
    }

    [ClientRpc]
    public void SpawnDummyProjectileClientRPC(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) { return; }
        SpawnDummyProjectile(spawnPos, direction);
    }
}
