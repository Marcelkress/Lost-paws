using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IHealth
{
    public int maxHealth;
    private int currentHealth;

    public UnityEvent TakeDamageEvent = new ();
    public UnityEvent HealEvent = new();
    
    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        
        TakeDamageEvent.Invoke();
        
        if( currentHealth <= 0)
        {
            Debug.Log("Dead");
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
}

public interface IHealth
{
    public void TakeDamage(int amount);

    public void Heal(int amount);

    public int GetCurrentHealth();
}
