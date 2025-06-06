using UnityEngine;
using UnityEngine.AI;

public class Skeleton : MonoBehaviour
{
    public NavMeshAgent agent;
    // public GameObject attack;

    public float hitPoints;

    public Transform player;

    public LayerMask groundLevel, playerLevel;

    public Vector3 patrolPoint;
    public float patrolRange;


    public float attackCooldown;


    public float chaseRange, attackRange;
    public bool playerInChaseRange, playerInAttackRange;
    private bool _alreadyAttacked;
    private bool _patrolPointSet;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, playerLevel);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLevel);


        if (!playerInChaseRange && !playerInAttackRange) Patroling();
        if (playerInChaseRange && !playerInAttackRange) Chasing();
        if (playerInChaseRange && playerInAttackRange) Attacking();
    }

    private void Patroling()
    {
        if (!_patrolPointSet) SearchPatrolPoint();

        if (_patrolPointSet)
            agent.SetDestination(patrolPoint);

        var distanceToPatrolPoint = transform.position - patrolPoint;

        if (distanceToPatrolPoint.magnitude < 1f)
            _patrolPointSet = false;
    }

    private void SearchPatrolPoint()
    {
        var randomZ = Random.Range(-patrolRange, patrolRange);
        var randomX = Random.Range(-patrolRange, patrolRange);

            Vector3 randomPoint = transform.position + new Vector3(randomX, 0f, randomZ);

        NavMeshHit hit;
        // Prüfe, ob in der Nähe des Punkts ein gültiger NavMesh-Punkt ist
        if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            _patrolPointSet = true;
        }
        else
        {
            Debug.LogWarning("Kein gültiger Patrol Point gefunden.");
        }
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (_alreadyAttacked) return;
        //Instantiate(attack, transform);


        _alreadyAttacked = true;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }

    public void Ouch(float damage)
    {
        hitPoints -= damage;

        if (hitPoints <= 0) Death();
    }

    private void Death()
    {
        Destroy(gameObject, 2f);
    }
}