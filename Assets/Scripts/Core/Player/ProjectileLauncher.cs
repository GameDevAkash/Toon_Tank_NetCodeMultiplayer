using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform ProjectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject ClientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private bool shouldFire;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;

    private float previousFireTime;
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
        if (!shouldFire) { return; }
        if (Time.time < (1 / fireRate) + previousFireTime) { return; }


        PrimaryFireServerRPC(ProjectileSpawnPoint.position, ProjectileSpawnPoint.up);
        SpawnDummyProjectile(ProjectileSpawnPoint.position, ProjectileSpawnPoint.up);

        previousFireTime = Time.time;
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
