using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class MPBulletSpawner : NetworkBehaviour
{
    public Rigidbody bullet;
    public Transform bulletPos;
    private float bulletSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire3") && IsOwner)
        {
            FireServerRPC(bulletSpeed, gameObject.GetComponent<MP_PlayerAttribs>().powerUp.Value);
        }

    }

    [ServerRpc]

    private void FireServerRPC(float speed, bool powerUp, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Fired Weapon");   
        Rigidbody bulletClone = Instantiate(bullet, bulletPos.position, transform.rotation);
        bulletClone.velocity = transform.forward * speed;
        bulletClone.GetComponent<MP_BulletScript>().spawnerPlayerId = serverRpcParams.Receive.SenderClientId;
        bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
        Destroy(bulletClone.gameObject, 3);

        if (powerUp)
        {
            Vector3 temp = new Vector3(1, 0, 0);
            bulletPos.Translate(temp, bulletPos);
            bulletClone = Instantiate(bullet, bulletPos.position, transform.rotation);
            bulletClone.velocity = transform.forward * speed;
            bulletClone.GetComponent<MP_BulletScript>().spawnerPlayerId = serverRpcParams.Receive.SenderClientId;
            bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
            Destroy(bulletClone.gameObject, 3);

            temp = new Vector3(-2, 0, 0);
            bulletPos.Translate(temp, bulletPos);
            bulletClone = Instantiate(bullet, bulletPos.position, transform.rotation);
            bulletClone.velocity = transform.forward * speed;
            bulletClone.GetComponent<MP_BulletScript>().spawnerPlayerId = serverRpcParams.Receive.SenderClientId;
            bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
            Destroy(bulletClone.gameObject, 3);

            temp = new Vector3(1, 0, 0);
            bulletPos.Translate(temp, bulletPos);
        }
    }
}
