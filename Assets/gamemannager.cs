using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    // Referência ao Canvas de Game Over
    public GameObject gameOverUI;  // UI de Game Over
    public bool isGameOver = false;

    // Atualize a cada quadro, verificando se o jogador morreu
    private void Update()
    {
        // Apenas exibe o menu de game over quando o jogador morre
        if (isGameOver)
        {
            ShowGameOverMenu();
        }
    }

    // Função chamada para mostrar a tela de Game Over
    public void ShowGameOverMenu()
    {
        // Ativa a UI de game over
        gameOverUI.SetActive(true);

        // Para o tempo do jogo para evitar movimento ou outros eventos enquanto a tela de Game Over está aberta
        Time.timeScale = 0f;
    }

    // Função para reiniciar a fase
    public void RestartGame()
    {
        // Reinicia a cena atual
        Time.timeScale = 1f;  // Restaura o tempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reinicia a cena atual
    }

    // Função para voltar ao menu principal
    public void MainMenu()
    {
        Time.timeScale = 1f;  // Restaura o tempo
        SceneManager.LoadScene(0);  // Substitua "0" pelo índice ou nome da sua cena de menu principal
    }
}
