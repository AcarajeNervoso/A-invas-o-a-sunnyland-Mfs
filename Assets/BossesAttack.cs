using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossesAttack : MonoBehaviour
{
    public float speed = 2f; // Velocidade do inimigo
    public Transform leftLimit; // Limite � esquerda
    public Transform rightLimit; // Limite � direita
    public int health = 5; // Vida do boss
    public Animator animator; // Refer�ncia ao Animator

    private bool movingRight = true; // Dire��o inicial

    void Update()
    {
        // Movimenta o boss
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);

            if (transform.position.x >= rightLimit.position.x)
            {
                movingRight = false; // Muda a dire��o
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (transform.position.x <= leftLimit.position.x)
            {
                movingRight = true; // Muda a dire��o
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

                // Aplica o knockback (dire��o oposta ao boss e empurra para longe)
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                player.ApplyKnockback(knockbackDirection, 10f); // 10f � a for�a do knockback, ajuste conforme necess�rio
            }
        }
    }

    public void TakeDamage()
    {
        health--;

        if (animator != null)
        {
            animator.SetTrigger("Hurt"); // Ativa anima��o de dano
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
            animator.SetTrigger("Die"); // Ativa anima��o de morte
        }

        Destroy(gameObject, 0.5f); // Destroi o boss ap�s a anima��o
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Inverte apenas o eixo X
        transform.localScale = localScale;
    }
}
