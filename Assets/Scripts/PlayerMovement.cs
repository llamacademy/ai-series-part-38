using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Player Player;
    [SerializeField]
    private Camera Camera = null;
    [SerializeField]
    private LayerMask LayerMask;
    [SerializeField]
    private Vector3 CameraOffset;
    private NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Application.isFocused && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Ray ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask))
            {
                Agent.SetDestination(hit.point);
                Player.ChangeState(PlayerState.Moving);
            }
        }

        if (Agent.enabled 
            && Agent.remainingDistance <= Agent.stoppingDistance 
            && Player.State == PlayerState.Moving)
        {
            Player.ChangeState(PlayerState.Idle);
        }
    }

    private void LateUpdate()
    {
        Camera.transform.position = transform.position + CameraOffset;
    }
}
