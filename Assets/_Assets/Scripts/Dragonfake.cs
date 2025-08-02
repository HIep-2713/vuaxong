using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragonfake : MonoBehaviour
{
    public float move;
    public float speed = 5f;
    public Animator ani;
    public Rigidbody2D rb;
    public bool Isfacing = true;   
    public float jumpForce = 5f; 
    public int jumpCount=0; 
    public bool isCrouching = false; 
    public Vector3 theScale;
    private bool isGrounded;
    public bool isAttacking = false ;
    public GameObject bulletPrefab; 
    public Transform firePoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        {
            if (isGrounded)
            {
                jumpCount = 0;
            }

            // Only handle input if the character is not attacking
            if (!isAttacking)
            {
                HandleMovement();
                HandleJump();
                HandleCrouch();
            }
            else
            {
                // Stop all movement while attacking
                rb.velocity = Vector2.zero;
            }

            HandleAttack();
        }
    }
    void HandleMovement()
    {
        // Di chuy?n trái ph?i b?ng A/D
        if (Input.GetKey(KeyCode.A))
            move = -1f;
        else if (Input.GetKey(KeyCode.D))
            move = 1f;
        else
            move = 0f;
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        
        if (move > 0 && !Isfacing)
            Flip();
        else if (move < 0 && Isfacing)
            Flip();
        ani.SetFloat("move", Mathf.Abs(move));
    }
    private void Flip()
    {
        Isfacing = !Isfacing;
        theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && jumpCount < 2)
        {
            if (isGrounded || jumpCount < 1) // Allow jump if grounded or has one jump left
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                ani.SetTrigger("Jump");
                jumpCount++;
            }
        }
    }
        void HandleCrouch()
        {
            if (Input.GetKeyDown(KeyCode.S) && !isCrouching)
            {
                isCrouching = true;
                ani.SetBool("isCrouching", true);
            }
            else if (Input.GetKeyUp(KeyCode.S) && isCrouching)
            {
                isCrouching = false;
                ani.SetBool("isCrouching", false);
            }
        }
    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            ani.SetTrigger("Attack");
            isAttacking = true;
            Shoot();
        }
        else
        {
            isAttacking = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
        {
            // Ki?m tra xem ??i t??ng va ch?m có ph?i là m?t ??t không (ví d? qua tag)
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
            }
        }
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Instantiate the bullet at the firePoint's position and rotation
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

        // Determine the direction based on the character's facing
        Vector2 shootDirection;
        if (Isfacing)
        {
            shootDirection = Vector2.right;
        }
        else
        {
            shootDirection = Vector2.left;

            // Also flip the bullet's sprite to match the direction
            Vector3 bulletScale = bulletObject.transform.localScale;
            bulletScale.x *= -1;
            bulletObject.transform.localScale = bulletScale;
        }

        // Call the new SetDirection method on the bullet
        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection);
        }
    }
    public void FinishAttack()
    {
        isAttacking = false;
    }
}
    
