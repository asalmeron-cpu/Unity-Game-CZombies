using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public PlayerVida player; 
    public TMP_Text scoreText;
    public GameObject quickrevive; 
    public GameObject dobletiro; 
    public TMP_Text rondastext;
    public RoundManager rondas;
    public TMP_Text vida;
    public Gun arma;
    public TMP_Text armatxt;

    public TMP_Text munientera;

    void Update()
    {
        //CADA FRAME
        if (player != null && scoreText != null)
        {
            scoreText.text = $"Score: {player.score}";
        }

        if (player != null && quickrevive != null ){
            if (player.hasQuickRevive)
            {
            quickrevive.SetActive(true);
            }else{
                quickrevive.SetActive(false);
            }
            
        }
        if (player != null && rondas != null && rondastext != null)
        {
            rondastext.text = $"Ronda: {rondas.currentRound}";
        }
        if (player != null && rondas != null && vida != null)
        {
            int displayHealth = Mathf.RoundToInt(player.currentHealth / 5f) * 5;
            vida.text = $"{displayHealth}";

        }
        if (arma != null && armatxt != null)
        {
            armatxt.text = $"{arma.currentAmmo}";
            munientera.text = $"{arma.reserveAmmo}";

        }
         
        if (arma.GetDobleTiro())
        {
            dobletiro.SetActive(true);
        }else{
            dobletiro.SetActive(false);
        }
            
        
        
    }   
}
