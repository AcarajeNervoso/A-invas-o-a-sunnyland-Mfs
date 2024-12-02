using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    public string cena;
    public GameObject optionsPanel;
    public GameObject levelselect;
    public GameObject creditsPanel;
    public GameObject PauseMenuCanva;

    // Start is called before the first frame update
    void Start()
    {

    }


    void Update()
    {

    }

    // linha de comando para loadar a gameplay
    public void StartGame(int cena)
    {
        StartCoroutine(StartLevel(cena));
        // SceneManager.LoadScene(cena);
    }

    // Config de mostrar configuracoes
    public void ShowOptions()
    {
        optionsPanel.SetActive(true);
    }
    // só um teste para tela de pause
    //public void ShowOptions()
    //{
    //optionsPanel.SetActive(true);
    //}

    // Config tela creditos
    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
    }

    // Linha de voltar para menu principal
    public void BackToMenu()
    {
        optionsPanel.SetActive(false);
        levelselect.SetActive(false);
        creditsPanel.SetActive(false);
        PauseMenuCanva.SetActive(false);
    }

    // config level select

    public void ShowLeves()
    {
        levelselect.SetActive(true);
    }


    // Remova o modificador 'public'
    public void Quit()
    {
        // Editor unity
        //UnityEditor.EditorApplication.isPlaying = false;
        // Jogo compilado
        Application.Quit();
    }
    public IEnumerator StartLevel(int cena)
    {

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(cena);
    }
    // Declaração de nível superior removida
}