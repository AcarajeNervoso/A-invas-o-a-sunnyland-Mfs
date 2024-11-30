using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f; // Velocidade do inimigo
    public Transform leftLimit; // Limite à esquerda
    public Transform rightLimit; // Limite à direita

    private bool movingRight = true; // Direção inicial

    void Update()
    {
        // Movimenta o inimigo
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            // Verifica se atingiu o limite direito
            if (transform.position.x >= rightLimit.position.x)
            {
                movingRight = false; // Muda a direção
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            // Verifica se atingiu o limite esquerdo
            if (transform.position.x <= leftLimit.position.x)
            {
                movingRight = true; // Muda a direção
                Flip();
            }
        }
    }

    // Inverte o sprite do inimigo
    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Inverte apenas o eixo X
        transform.localScale = localScale;
    }
}

