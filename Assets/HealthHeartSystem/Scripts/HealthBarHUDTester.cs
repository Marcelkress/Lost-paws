/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 */

using UnityEngine;

public class HealthBarHUDTester : MonoBehaviour
{
    public void AddHealth()
    {
        PlayerStats.Instance.AddHealth();
    }

    public void Heal(int health)
    {
        PlayerStats.Instance.Heal(health);
    }

    public void Hurt(int dmg)
    {
        PlayerStats.Instance.TakeDamage(dmg);
    }
}
