using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get { return facingLeft; } }

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;
    [SerializeField] private AudioSource dashSoundEffect;
 

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private Knockback knockback;
    private float startingMoveSpeed;

    private bool facingLeft = false;
    public bool isDashing = false;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        knockback = GetComponent<Knockback>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();

        startingMoveSpeed = moveSpeed;

        ActiveInventory.Instance.EquipStartingWeapon();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
       AdjustPlayerFacingDirection();
       Move();
    }

    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }
    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue <Vector2> ();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        if (knockback.GettingKnockedBack || PlayerHealth.Instance.IsDead) { return; } // Can't move if getting knocked back or deadge

        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x) { 
            mySpriteRenderer.flipX = true;
            facingLeft = true;
        } else
        {
            mySpriteRenderer.flipX = false;
            facingLeft = false;
        }
    }
    private void Dash()
    {
        if (!isDashing && Stamina.Instance.CurrentStamina > 0)
        {
            Stamina.Instance.UseStamina();
            isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
            dashSoundEffect.Play();

            PlayerHealth.Instance.canTakeDamage = false; // Turn off canTakeDamage
        }
    }


    private  IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = .2f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
        PlayerHealth.Instance.canTakeDamage = true; // Re-enable canTakeDamage
    }
}
