using System;
using System.Linq;
using UnityEngine;

public class WarriorEntity : Entity
{
    public int SpawnPointId;

    public float StoppingDistance;

    public float Damage;
    public float AttackRate;

    private float attackTime;

    private bool hasTarget;
    private EnemyEntity target;
    private float distanceToTarget;

    private void Awake()
    {
        FindTarget();
    }

    private void Update()
    {
        if (hasTarget)
        {
            if (CheckTarget())
            {
                FindTarget();
                return;
            }

            MoveToTarget();
            AttackTarget();
        }
    }

    private bool CheckTarget()
    {
        return target == null;
    }
    private void AttackTarget()
    {
        if (distanceToTarget > 1.1f) return;

        attackTime -= Time.deltaTime;
        if (attackTime > 0) return;
        attackTime = AttackRate;

        bool shouldUpdateTarget = target.Health <= Damage;
        target.ApplyDamage(Damage);

        if (shouldUpdateTarget)
            FindTarget();
    }
    private void MoveToTarget()
    {
        var targetPosition = target.transform.position;
        var dstToWarrior = targetPosition - transform.position;
        distanceToTarget = dstToWarrior.magnitude;

        if (dstToWarrior.magnitude <= StoppingDistance) return;

        Move(transform.position + dstToWarrior.normalized * (dstToWarrior.magnitude - StoppingDistance));
    }
    private void FindTarget()
    {
        target = G.EnemySystem.Wave.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault();
        hasTarget = target != null;
    }
}
