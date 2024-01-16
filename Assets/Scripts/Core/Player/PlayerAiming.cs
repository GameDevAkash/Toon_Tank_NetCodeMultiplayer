using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform turretTransform;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float turningrate = 10;

    private void LateUpdate()
    {
        if (!IsOwner) { return; }
        /*float zRotation = Camera.main.ScreenToWorldPoint(inputReader.AimPosition).x * -turningrate*Time.fixedDeltaTime;
        turretTransform.Rotate(0f, 0f, zRotation);*/
        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);
        turretTransform.up = new Vector2(aimWorldPosition.x - turretTransform.position.x, aimWorldPosition.y- turretTransform.position.y);
    }
}
