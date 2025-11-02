using UnityEngine;

public class DobleTiro : MonoBehaviour
{
    public float distanciaInteraccion = 3f;
    public int puntuacionNecesaria = 1500;
    public Gun gun;

    Transform jugador;
    PlayerVida playerVida;
   
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
        if ( jugador == null || playerVida == null || gun == null) return;

        if (Vector3.Distance(jugador.position, transform.position) <= distanciaInteraccion)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (playerVida.GetScore() >= puntuacionNecesaria)
                {
                    playerVida.RemoveScore(puntuacionNecesaria);
                    gun.PonerDaño(gun.GetDamage() * 2);
                    gun.SetDobleTiro(true);
                    
                }
            }
        }
    }
}
