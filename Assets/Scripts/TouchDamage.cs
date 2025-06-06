using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TouchDamage : MonoBehaviour
{
    public int damage;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<IHealth>().TakeDamage(damage, true);
        }
    }
}
