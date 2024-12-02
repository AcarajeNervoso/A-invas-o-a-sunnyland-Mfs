using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public int health = 3;
    public bool magiaLiberada;
    public bool doublejumpLiberado;
    public bool dashLiberado;
    public bool walljumpLiberado;
    public bool escalarLiberado;

    public GameObject gameOverCanvas; // Referência ao Canvas de Game Over

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;
    private bool isInvulnerable = false;
    private bool canDoubleJump = false;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTime = 0.2f;
    private float dashSpeed = 20f;
    public float invulnerabilityDuration = 3f;

    // Novos ajustes para escalar
    private bool isClimbing = false;  // Para saber se está escalando
    private float climbSpeed = 3f;  // A velocidade de escalar
    private float horizontalMove;  // Movimentação horizontal
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Variáveis para Wall Jump
    private bool isTouchingWall = false; // Detecta se está tocando a parede
    private bool canWallJump = false; // Controla se o jogador pode fazer Wall Jump
    private float wallJumpForceX = 5f; // Força do wall jump na direção X
    private float wallJumpForceY = 5f; // Força do wall jump na direção Y
    private float wallCheckDistance = 0.5f; // Distância para verificar a parede

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if (gameOverCanvas.activeSelf) return;

        if (isDashing) return;

        horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        // Se o jogador estiver escalando
        if (isClimbing)
        {
            rb.gravityScale = 0; // Desativa a gravidade
            rb.velocity = new Vector2(0, verticalMove * climbSpeed); // Movimentação vertical

            // Permitir o movimento horizontal apenas quando não estiver escalando
            if (horizontalMove != 0)
            {
                // Lógica para impedir movimento horizontal enquanto escala, ou pode ser customizado
                rb.velocity = new Vector2(horizontalMove * moveSpeed, rb.velocity.y);
            }
        }
        else
        {
            rb.gravityScale = 1; // Restaura a gravidade
            rb.velocity = new Vector2(horizontalMove * moveSpeed, rb.velocity.y);
        }

        FlipSprite(horizontalMove);

        // Animações de corrida e idle
        if (isGrounded)
        {
            animator.SetBool("isRunning", horizontalMove != 0);
        }

        // Pulo e Double Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = true;
            }
            else if (doublejumpLiberado && canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false;
            }
            else if (isTouchingWall && walljumpLiberado)
            {
                // Wall Jump: Lança o jogador na direção oposta da parede
                WallJump();
            }
        }

        // Dash
        if (dashLiberado && Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash(horizontalMove));
        }
    }

    // Verifica colisão com a escada
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Escada"))
        {
            isClimbing = true; // Inicia o processo de escalada
        }

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == 3)
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("boss"))
        {
            HandleEnemyCollision(collision);
        }

        if (collision.gameObject.CompareTag("Vazio"))
        {
            TakeDamage();
        }

        // Verifica se está tocando a parede
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
            canWallJump = true; // Permite o Wall Jump
        }
    }

    // Quando sai da escada, a escalada é interrompida
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Escada"))
        {
            isClimbing = false; // Interrompe a escalada
        }

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == 3)
        {
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
            canWallJump = false; // Impede o Wall Jump se não estiver tocando a parede
        }
    }

    private void WallJump()
    {
        // Verifica a direção da parede
        float wallDirection = spriteRenderer.flipX ? 1 : -1;

        // Aplica o Wall Jump com base na direção da parede
        rb.velocity = new Vector2(wallDirection * wallJumpForceX, wallJumpForceY);

        // Impede o Wall Jump múltiplos seguidos
        canWallJump = false;
    }

    private void HandleEnemyCollision(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        Vector2 enemyCenter = collision.collider.bounds.center;

        if (contactPoint.y > enemyCenter.y)
        {
            if (collision.transform.CompareTag("boss"))
            {
                collision.transform.GetComponent<BossesAttack>().TakeDamage();
                Destroy(collision.gameObject, 0.5f);
            }
            else
            {
                Destroy(collision.gameObject);
            }
            rb.velocity = new Vector2(rb.velocity.x, jumpForce / 2);
        }
        else
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (isInvulnerable) return;

        health -= 1;
        Debug.Log("Dano recebido! Vida restante: " + health);

        if (health <= 0)
        {
            Debug.Log("Você morreu!");
            ActivateGameOverCanvas();
        }

        StartCoroutine(InvulnerabilityCoroutine());
    }

    private void ActivateGameOverCanvas()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 6; i++)
            {
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                yield return new WaitForSeconds(invulnerabilityDuration / 6 / 2);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(invulnerabilityDuration / 6 / 2);
            }
        }

        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    private IEnumerator Dash(float horizontal)
    {
        isDashing = true;
        canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        rb.velocity = new Vector2(horizontal * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(1f);
        canDash = true;
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

    public void ApplyKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        if (isInvulnerable) return;

        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }
}
