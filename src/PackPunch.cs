using UnityEngine;

public class PackAPunch : MonoBehaviour
{
    public float distanciaInteraccion = 3f;
    public int puntuacionNecesaria = 10000;
    public Gun gun;

    Transform jugador;
    PlayerVida playerVida;
    bool usado = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            jugador = player.transform;
            playerVida = player.GetComponent<PlayerVida>();
        }
    }

    void Update()
    {
        if (usado || jugador == null || playerVida == null || gun == null) return;

        if (Vector3.Distance(jugador.position, transform.position) <= distanciaInteraccion)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (playerVida.GetScore() >= puntuacionNecesaria)
                {
                    playerVida.RemoveScore(puntuacionNecesaria);
                    gun.ActivarExplosivo(true);
                    usado = true;
                }
            }
        }
    }
}
