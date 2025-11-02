using UnityEngine;

public class QuickReviveMaquina : MonoBehaviour
{
   
    public float distanciaInteraccion = 3f;

    public int puntuacionNecesaria = 1500;


    public string mensajeInteraccion = "E PARA USAR";
    public GameObject objetoAActivar;

    private Transform jugador;
    private PlayerVida playerVida;

    void Start()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");

        if (playerGO)
        {
            jugador = playerGO.transform;
            playerVida = playerGO.GetComponent<PlayerVida>();
        }
        else
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (!jugador || !playerVida) return;

        float distancia = Vector3.Distance(jugador.position, transform.position);

     
        if (distancia <= distanciaInteraccion)
        {
    
            Debug.Log($" {mensajeInteraccion}");

            if (Input.GetKeyDown(KeyCode.E))
            {
              
                if (playerVida.GetScore() >= puntuacionNecesaria)
                {
                    playerVida.RemoveScore(puntuacionNecesaria);
                    playerVida.comprarquickrevive(1);
                }
                else
                {
                    Debug.Log($"Necesitas {puntuacionNecesaria} puntos.");
                }
            }
        }
    }


}
