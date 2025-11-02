using UnityEngine;

public class killzone : MonoBehaviour
{
    public PlayerVida player;


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            player.GoUltraDown();

        }
    }
}

