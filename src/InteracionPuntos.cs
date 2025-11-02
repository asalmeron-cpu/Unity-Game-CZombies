using UnityEngine;

public class InteracionPuntos : MonoBehaviour
{
   
    public float distanciaInteraccion = 3f;

    public int puntuacionNecesaria = 100;

    public zona1 puerta;   
    public string mensajeInteraccion = "Pulsa [E] para interactuar";

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



            if (Input.GetKeyDown(KeyCode.E))
            {

                if (playerVida.GetScore() >= puntuacionNecesaria)
                {
                    playerVida.RemoveScore(puntuacionNecesaria);
                    puerta.AbrirZona();
                    Destroy(objetoAActivar);

                }
                else
                {

                }
            }
        }
    }

   
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}
