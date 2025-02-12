using System.Linq;
using UnityEngine;

public class EnemyEntity : Entity
{
    private enum TargetType
    {
        none,
        warrior,
        building,
    }

    public float StoppingDistance;
    public float Damage;
    public float AttackRate;

    private TargetType targetType;
    private WarriorEntity warriorTarget;
    private Building buildingTarget;
    private float distanceToTarget;
    private float attackTime;

    private void Awake()
    {
        FindTarget();
    }

    private void Update()
    {
        if (targetType == TargetType.none) return;
        if (CheckTarget())
        {
            FindTarget();
            return;
        }

        MoveToTarget();
        AttackTarget();
    }

    private bool CheckTarget()
    {
        if(targetType == TargetType.none) return false;
        if(targetType == TargetType.warrior)
            return warriorTarget == null;
        if(targetType == TargetType.building)
            return buildingTarget == null;

        return false;
    }
    private void AttackTarget()
    {
        if (distanceToTarget > 1.1f) return;

        attackTime -= Time.deltaTime;
        if (attackTime > 0) return;
        attackTime = AttackRate;

        bool shouldUpdateTarget = false;

        if (targetType == TargetType.building)
        {
            shouldUpdateTarget = buildingTarget.Health <= Damage;

            buildingTarget.ApplyDamage(Damage);
        }
        if (targetType == TargetType.warrior)
        {
            shouldUpdateTarget = warriorTarget.Health <= Damage;

            warriorTarget.ApplyDamage(Damage);
        }

        if(shouldUpdateTarget)
            FindTarget();
    }
    private void MoveToTarget()
    {
        if (targetType == TargetType.warrior)
        {
            var warriorPosition = warriorTarget.transform.position;
            var dstToWarrior = warriorPosition - transform.position;
            distanceToTarget = dstToWarrior.magnitude;

            if (dstToWarrior.magnitude <= StoppingDistance) return;

            Move(transform.position + dstToWarrior.normalized * (dstToWarrior.magnitude - StoppingDistance));
        }
        if (targetType == TargetType.building)
        {
            var buildingPosition = buildingTarget.transform.position;
            var dstToBuilding = buildingPosition - transform.position;
            var verDelta = Mathf.Abs(buildingPosition.y - transform.position.y);
            var horDelta = Mathf.Abs(buildingPosition.x - transform.position.x);
            var stoppingDistance = verDelta > horDelta ? Mathf.Max(1, buildingTarget.Size.y / 2f) : Mathf.Max(1, buildingTarget.Size.x / 2f);
            var targetPosition = transform.position + dstToBuilding.normalized * (dstToBuilding.magnitude - stoppingDistance);
            distanceToTarget = dstToBuilding.magnitude;
            Move(targetPosition);
        }
    }

    private void FindTarget()
    {
        buildingTarget = G.BuildSystem.Buildings
            .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
            .FirstOrDefault();

        warriorTarget = G.BuildSystem.Buildings
            .Where(x => x is ArmyBuilding)
            .Select(x => ((ArmyBuilding)x).AliveWarriors)
            .Where(x => x.Count > 0)
            .Select(x => x.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First())
            .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
            .FirstOrDefault();

        if(buildingTarget != null && warriorTarget != null)
        {
            var dstToBuilding = Vector3.Distance(transform.position, buildingTarget.transform.position);
            var dstToWarrior = Vector3.Distance(transform.position, warriorTarget.transform.position);

            targetType = dstToBuilding < dstToWarrior ? TargetType.building : TargetType.warrior;
        }
        else if (buildingTarget != null)
        {
            targetType = TargetType.building;
        }
        else if (buildingTarget != null)
        {
            targetType = TargetType.warrior;
        }
        else
        {
            targetType = TargetType.none;
        }
    }
}