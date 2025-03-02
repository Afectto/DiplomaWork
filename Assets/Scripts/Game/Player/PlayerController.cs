using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 LastMoveDirection { get; private set; }
    private Animator _animator;
    private float _speed;

    public void Initialize(Animator animator, float speed)
    {
        _animator = animator;
        _speed = speed;
    }

    public void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            LastMoveDirection = new Vector2(moveX, moveY).normalized;
            GetComponent<FightSystem>().SetDirection(LastMoveDirection);
        }

        _animator.SetFloat("Horizontal", moveX);
        _animator.SetFloat("Vertical", moveY);
        _animator.SetFloat("Speed", Mathf.Abs(moveX) + Mathf.Abs(moveY));

        Vector3 newPosition = transform.localPosition + new Vector3(moveX, moveY, 0) * _speed * Time.deltaTime;
        transform.localPosition = newPosition;
    }
}