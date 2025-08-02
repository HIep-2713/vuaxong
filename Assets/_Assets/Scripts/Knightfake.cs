using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knightfake : MonoBehaviour
{
    public float move;
    public float speed = 5f;
    public Animator ani;
    public Rigidbody2D rb;
    public bool Isfacing = true;
    public float jumpForce = 5f;
    public int jumpCount = 0;
    public bool isCrouching = false;
    public Vector3 theScale;
    public bool isCast = false;
    private bool isGrounded;
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
        if (isGrounded)
        {
            jumpCount = 0;
        }

        // Only handle input if the character is not casting
        if (!isCast)
        {
            HandleMovement();
            HandleJump();
            HandleCrouch();
        }
        else
        {
            // Stop all movement while casting
            rb.velocity = Vector2.zero;
        }

        HandleCast();
    }

    void HandleMovement()
    {
        // Di chuyen trái phai bang A/D
        if (Input.GetKey(KeyCode.LeftArrow))
            move = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
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
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount < 2)
        {
            if (isGrounded || jumpCount < 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                ani.SetTrigger("Jump");
                jumpCount++;
            }
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isCrouching)
        {
            isCrouching = true;
            ani.SetBool("isCrouching", true);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow) && isCrouching)
        {
            isCrouching = false;
            ani.SetBool("isCrouching", false);
        }
    }

    void HandleCast()
    {
        // Only allow casting if the character isn't already casting
        if (Input.GetKeyDown(KeyCode.Keypad1) && !isCast)
        {
            ani.SetTrigger("Cast");
            isCast = true;
            Shoot();
        }
        else
        {
            isCast = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
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

    // This is the function that the Animation Event will call
    public void FinishCast()
    {
        isCast = false;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

        Vector2 shootDirection;
        if (Isfacing)
        {
            shootDirection = Vector2.right;
        }
        else
        {
            shootDirection = Vector2.left;

            Vector3 bulletScale = bulletObject.transform.localScale;
            bulletScale.x *= -1;
            bulletObject.transform.localScale = bulletScale;
        }

        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection);
        }
    }
}