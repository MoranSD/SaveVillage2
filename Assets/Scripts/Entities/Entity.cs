using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action<Guid> OnDie;

    public float Health;
    public float Speed;
    public Guid UnicId;

    public void ApplyDamage(float damage)
    {
        Health -= damage;

        if(Health <= 0)
        {
            OnDie?.Invoke(UnicId);
            Destroy(gameObject);
        }
    }

    public void Move(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, Speed * Time.deltaTime);
    }
}
