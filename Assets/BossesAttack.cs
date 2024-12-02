using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossesAttack : MonoBehaviour
{
    public float speed = 2f; // Velocidade do inimigo
    public Transform leftLimit; // Limite à esquerda
    public Transform rightLimit; // Limite à direita
    public int health = 5; // Vida do boss
    public Animator animator; // Referência ao Animator

    private bool movingRight = true; // Direção inicial

    void Update()
    {
        // Movimenta o boss
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);

            if (transform.position.x >= rightLimit.position.x)
            {
                movingRight = false; // Muda a direção
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (transform.position.x <= leftLimit.position.x)
            {
                movingRight = true; // Muda a direção
                Flip();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // Aplica dano ao jogador
                player.TakeDamage();

                // Aplica o knockback (direção oposta ao boss e empurra para longe)
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                player.ApplyKnockback(knockbackDirection, 10f); // 10f é a força do knockback, ajuste conforme necessário
            }
        }
    }

    public void TakeDamage()
    {
        health--;

        if (animator != null)
        {
            animator.SetTrigger("Hurt"); // Ativa animação de dano
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Ativa animação de morte
        }

        Destroy(gameObject, 0.5f); // Destroi o boss após a animação
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Inverte apenas o eixo X
        transform.localScale = localScale;
    }
}
