using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class HealthPickup : MonoBehaviour
{
    public int healAmount, dontHealPast;
    private void OnTriggerEnter2D(Collider2D other)
    {
        IHealth health = other.transform.GetComponent<IHealth>();

        if (health != null)
        {
            if (health.GetCurrentHealth() == dontHealPast)
            {
                return;
            }
            
            health.Heal(healAmount);
            
            GetComponent<Animator>().SetTrigger("Disappear");
        }
    }
}
