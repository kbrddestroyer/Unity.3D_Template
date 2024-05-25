using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField, Range(0f, 10f)] private float fTriggerDistance;
    [SerializeField, Range(0f, 10f)] private float fMinDistance;
    [SerializeField, Range(0f, 10f)] private float fHp;
    [SerializeField, Range(0f, 10f)] private float fCorpseLifetime;
    [SerializeField, Range(0f, 10f)] private float fAttackDelay;
    [SerializeField, Range(1f, 10f)] private float fAttackPower;
    [SerializeField, Range(1f, 10f)] private float fWaypointDelay;
    [Header("Gizmos")]
    [SerializeField] private Color cTriggerColor;
    [SerializeField] private Color cMinDistanceColor;

    private GameObject[] waypoints;
    private Transform selectedWaypoint;

    private Player player;
    private Animator animator;
    private NavMeshAgent agent;
    [SerializeField] private bool bCanPunch = true;

    private bool bRunning = false;
    private bool Running
    {
        get => bRunning;
        set
        {
            if (bRunning != value)
            {
                animator.SetBool("running", value);
            }
            bRunning = value;
        }
    }

    public float HP
    {
        get => fHp;
        set
        {
            if (fHp > value)
            {
                animator.SetTrigger("take_damage");
            }
            fHp = value;
            bCanPunch = true;
            if (fHp <= 0)
            {
                Destroy(this.gameObject, fCorpseLifetime);
                RagdollActivator[] activators = transform.GetComponentsInChildren<RagdollActivator>();
                foreach (RagdollActivator activator in activators)
                {
                    activator.Activate();
                }
                animator.enabled = false;
                agent.enabled = false;
                enabled = false;
            }
        }
    }

    private IEnumerator changeState()
    {
        yield return new WaitForSeconds(fAttackDelay);
        bCanPunch = true;
    }

    public void ExitPunchAnimation()
    {
        Debug.LogWarning("Exit animation");
        StartCoroutine(changeState());
    }

    public void Damage()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > fMinDistance)
            return;

        player.HP -= fAttackPower;
    }

    private void Punch()
    {
        if (bCanPunch)
        {
            bCanPunch = false;
            animator.SetInteger("combat_anim", Random.Range(0, 2));
            animator.SetTrigger("combat");
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        agent = GetComponent<NavMeshAgent>();

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
    }

    private IEnumerator selectWaypoint()
    {
        Running = false;
        yield return new WaitForSeconds(fWaypointDelay);
        agent.destination = waypoints[Random.Range(0, waypoints.Length)].transform.position;
        Running = true;
    }

    private void UpdateWaypointState()
    {
        if (Vector3.Distance(agent.destination, transform.position) <= fMinDistance)
        {
            if (Running) StartCoroutine(selectWaypoint());
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= fTriggerDistance)
        {
            agent.destination = transform.position;
            if (distance <= fMinDistance)
            {
                Punch();
                Running = false;
            }
            else
            {
                Running = true;
                agent.destination = player.transform.position;
            }
        }
        else
        {
            UpdateWaypointState();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = cTriggerColor;
        Gizmos.DrawWireSphere(transform.position, fTriggerDistance);

        Gizmos.color = cMinDistanceColor;
        Gizmos.DrawWireSphere(transform.position, fMinDistance);
    }
#endif
}
