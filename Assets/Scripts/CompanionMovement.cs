using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CompanionMovement : MonoBehaviour
{
    private NavMeshAgent Agent;
    [SerializeField]
    private Player Player;
    [SerializeField]
    private Companion Companion;
    [Header("Idle Configs")]
    [SerializeField]
    [Range(0, 10f)]
    private float RotationSpeed = 2f;
    [Header("Follow Configs")]
    [SerializeField]
    private float FollowRadius = 2f;

    private Coroutine MovementCoroutine;
    private Coroutine StateChangeCoroutine;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player.OnStateChange += HandleStateChange;
    }

    private void HandleStateChange(PlayerState OldState, PlayerState NewState)
    {
        if (StateChangeCoroutine != null)
        {
            StopCoroutine(StateChangeCoroutine);
        }

        switch (NewState)
        {
            case PlayerState.Idle:
                StateChangeCoroutine = StartCoroutine(HandleIdlePlayer());
                break;
            case PlayerState.Moving:
                HandleMovingPlayer();
                break;
        }
    }

    private IEnumerator HandleIdlePlayer()
    {
        switch (Companion.State)
        {
            case CompanionState.Follow:
                yield return null; // 2 frames skipped in Follow Player, so need to be replicated here
                yield return null;
                yield return new WaitUntil(() => Companion.State == CompanionState.Idle);
                goto case CompanionState.Idle;
            case CompanionState.Idle:
                if (MovementCoroutine != null)
                {
                    StopCoroutine(MovementCoroutine);
                }
                Agent.enabled = false;
                MovementCoroutine = StartCoroutine(RotateAroundPlayer());
                break;
        }
    }

    private void HandleMovingPlayer()
    {
        Companion.ChangeState(CompanionState.Follow);
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }

        if (!Agent.enabled)
        {
            Agent.enabled = true;
            Agent.Warp(transform.position);
        }
        MovementCoroutine = StartCoroutine(FollowPlayer());
    }

    private IEnumerator RotateAroundPlayer()
    {
        WaitForFixedUpdate Wait = new WaitForFixedUpdate();
        while (true)
        {
            transform.RotateAround(Player.transform.position, Vector3.up, RotationSpeed);
            yield return Wait;
        }
    }

    private IEnumerator FollowPlayer()
    {
        yield return null; // Wait for player's destination to be updated!

        NavMeshAgent playerAgent = Player.GetComponentInChildren<NavMeshAgent>();
        Vector3 playerDestination = playerAgent.destination;
        Vector3 positionOffset = FollowRadius * new Vector3(
            Mathf.Cos(2 * Mathf.PI * Random.value),
            0,
            Mathf.Sin(2 * Mathf.PI * Random.value)
        ).normalized;

        Agent.SetDestination(playerDestination + positionOffset);

        yield return null; // wait for agent's destination
        yield return new WaitUntil(() => Agent.remainingDistance <= Agent.stoppingDistance);

        Companion.ChangeState(CompanionState.Idle);
    }
}
