using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float health = 100;
    public void TakeDamage(float _value)
    {
        health = Mathf.Clamp(health - _value, 0, 200);
        GameEvents.instance.health.SetValueAndForceNotify(health);

        if (health <= 0)
            GameEvents.instance.playerDead.SetValueAndForceNotify(true);
    }
}