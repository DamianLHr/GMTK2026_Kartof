using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator anim;
    
    private Rigidbody2D rb;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; 
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;

        if (rb.linearVelocity.magnitude > 0.1f)
            anim.SetBool("isWalking", true);
        else
            anim.SetBool("isWalking", false);

        if (movementInput.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(movementInput.x), 1, 1);
    }
}