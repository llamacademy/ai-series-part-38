public class Player : AbstractStateBehaviour<PlayerState>
{
    private void Start()
    {
        ChangeState(PlayerState.Idle);
    }
}