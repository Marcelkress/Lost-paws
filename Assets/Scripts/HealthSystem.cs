using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 9;
    public int currentHealth;

    public UnityEvent takeDamage;
    public UnityEvent heal;

    private Animator anim;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        takeDamage.Invoke();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        heal.Invoke();
    }
    
}
