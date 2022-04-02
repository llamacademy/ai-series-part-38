using UnityEngine;

public class BadCompanionMovement : MonoBehaviour
{
    [SerializeField]
    private Transform Player;
    [SerializeField]
    [Range(0, 5f)]
    private float RotationSpeed = 0.33f;
    
    private void FixedUpdate()
    {
        transform.RotateAround(Player.position, Vector3.up, RotationSpeed);
    }
}
