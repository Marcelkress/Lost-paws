using System;
using UnityEngine;

public class RespawnArea : MonoBehaviour
{
    public Transform respawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<IHealth>().SetRespawnPoint(respawnPosition);
        }
    }
}
