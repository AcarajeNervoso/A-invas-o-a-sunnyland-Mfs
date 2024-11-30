using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public int health = 3;

    [Header("Habilidades")]
    public bool magiaLiberada;
    public bool doublejumpLiberado;
    public bool dashLiberado;
    public bool walljumpLiberado;
    public bool escalarLiberado;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;
    private bool isLeader;
    private bool isInvulnerable = false; // Para controlar a invulnerabilidade
    public float invulnerabilityDuration = 3f; // Duração da invulnerabilidade em segundos
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        FlipSprite(horizontal);
        
        if (magiaLiberada)
        {

        }

        if (isLeader)
        {
            if (vertical != 0)
            {

            }
        }

        if (isGrounded)
        {
            if (horizontal != 0)
            {
                animator.SetBool("isRunning", true); // Se houver movimento, isRunning será true, senão, false
            }
            else if (horizontal == 0)
            {
                // Debug.Log("idle");
                animator.SetBool("isRunning", false);
            }
        }

     


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == 3)
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == 3)
        {
            isGrounded  = false;
        }
    }

    private void HandleEnemyCollision(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        Vector2 enemyCenter = collision.collider.bounds.center;

        if (contactPoint.y > enemyCenter.y)
        {
            Destroy(collision.gameObject);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce / 2);
        }
        else
        {
            TakeDamage(); // Toma dano se não colidir por cima
        }
    }

    private void TakeDamage()
    {
        if (isInvulnerable) return; // Impede tomar dano durante a invulnerabilidade

        health -= 1;
        Debug.Log("Dano recebido! Vida restante: " + health);

        if (health <= 0)
        {
            Debug.Log("Você morreu!");
            // Adicione aqui o comportamento para morte do jogador
        }

        StartCoroutine(InvulnerabilityCoroutine());
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        Debug.Log("Invulnerável!");

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 6; i++)
            {
                spriteRenderer.color = new Color(1, 1, 1, 0.5f); // Semitransparente
                yield return new WaitForSeconds(invulnerabilityDuration / 6 / 2);
                spriteRenderer.color = Color.white; // Cor original
                yield return new WaitForSeconds(invulnerabilityDuration / 6 / 2);
            }
        }

        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false; // Garante que volte ao estado normal
        Debug.Log("Invulnerabilidade acabou!");
    }

    void FlipSprite(float horizontal)
    {
        if (horizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontal < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}
