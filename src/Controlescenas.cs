using UnityEngine;
using UnityEngine.SceneManagement;
public class PasarScena : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Jugar()
    {
        SceneManager.LoadScene("Juego");
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Opciones()
    {
        SceneManager.LoadScene("Opciones");
    }
    public void Salir()
    {
        Application.Quit();
    }
}
