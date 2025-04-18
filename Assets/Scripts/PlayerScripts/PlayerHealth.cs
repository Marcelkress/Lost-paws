using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IHealth
{
    public int maxHealth;
    public float respawnDelay = 0.2f;
    public float duration = 1f;
    private int currentHealth;

    private bool dead;

    [HideInInspector] public Transform currentRespawnPoint;
    
    public UnityEvent TakeDamageEvent = new ();
    public UnityEvent HealEvent = new();
    
    void Awake()
    {
        currentHealth = maxHealth;
        dead = false;
    }

    /// <summary>
    /// How much damage should the player take. Should the player respawn upon taking damage
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="respawn"></param>
    public void TakeDamage(int amount, bool respawn)
    {
        currentHealth -= amount;
        
        TakeDamageEvent.Invoke();
        
        if( currentHealth <= 0 && !dead)
        {
            dead = true;
            SceneManager.instance.ReloadScene();
        }

        if (respawn && currentRespawnPoint != null)
        {
            StartCoroutine(RespawnDelay());
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        HealEvent.Invoke();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public void SetRespawnPoint(Transform point)
    {
        currentRespawnPoint = point;
    }

    private IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, currentRespawnPoint.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = currentRespawnPoint.position; 
    }
}

public interface IHealth
{
    public void TakeDamage(int amount, bool respawn);

    public void Heal(int amount);

    public int GetCurrentHealth();

    public void SetRespawnPoint(Transform point);
}
