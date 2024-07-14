using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public bool FacingLeft { get { return facingLeft; } set { facingLeft = value; } }

    private bool facingLeft = false;

    private PlayerControls playerControls;
    public Vector2 movement;

    public float moveSpeed = 5f;
    public float startingMoveSpeed;

    [SerializeField] public float dashSpeed = 4f;
    [SerializeField] private int dashStamina = 100;
    [SerializeField] public TrailRenderer trailRenderer;
    [SerializeField] private Transform swordWeaponCollider;
    [SerializeField] private Transform spearWeaponCollider;

    private Knockback knockback;

    private bool isDashing;

    private Vector2 input;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public bool isMenuActive = false;

    private Charecter charecter;

    public LayerMask collisionLayer; // Layer to check for collisions

    private void Awake()
    {
        instance = this;

        playerControls = new PlayerControls();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody2D>();
        charecter = GetComponent<Charecter>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
        startingMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        if (!isMenuActive)
        {
            HandleUpdate();
        }
    }

    public void HandleUpdate()
    {
        PlayerInput();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public Transform GetSwordWeaponCollider()
    {
        return swordWeaponCollider;
    }
    public Transform GetSpearWeaponCollider()
    {
        return spearWeaponCollider;
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);

        // Check if the player is moving
        if (movement.magnitude > 0)
        {
            animator.SetBool("Idle", false); // Player is not idle
        }
        else
        {
            animator.SetBool("Idle", true); // Player is idle
        }
    }

    private void Move()
    {
        if (knockback.GettingKnockedBack || charecter.isDead) { return; }
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    public void Warp(Vector3 destinationPos)
    {
        StopAllCoroutines();
        destinationPos.z = 0; // Ensure z is 0
        transform.position = destinationPos;
    }

    void Interact()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 1f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    private void Restart()
    {
        // Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replaces the existing one
        // and not load all the scene objects in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            spriteRenderer.flipX = true;
            facingLeft = true;
        }
        else
        {
            spriteRenderer.flipX = false;
            facingLeft = false;
        }
    }

    public void Dash()
    {
        if (!isDashing)
        {
            Vector3 dashDirection = GetMouseDirection();
            if (dashDirection != Vector3.zero)
            {
                isDashing = true;
                StartCoroutine(DashRoutine(dashDirection));
            }
        }
    }

    private Vector3 GetMouseDirection()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 dashDirection = (worldMousePosition - transform.position).normalized;
        dashDirection.z = 0; // Ensure z is 0
        return dashDirection;
    }

    private IEnumerator DashRoutine(Vector3 dashDirection)
    {
        float dashTime = 0.2f;
        float dashCD = 0.25f;
        float dashDistance = moveSpeed * dashSpeed * dashTime;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dashDirection, out hit, dashDistance, collisionLayer))
        {
            dashDistance = hit.distance; // Limit dash distance to the collision point
        }

        float dashEndTime = Time.time + dashTime;
        trailRenderer.emitting = true;

        while (Time.time < dashEndTime)
        {
            float distanceToMove = Mathf.Min(dashDistance, moveSpeed * dashSpeed * Time.deltaTime);
            Vector3 newPosition = transform.position + dashDirection * distanceToMove;
            newPosition.z = 0; // Ensure z is 0
            transform.position = newPosition;
            dashDistance -= distanceToMove;

            if (dashDistance <= 0)
                break;

            yield return null;
        }

        trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }
}
