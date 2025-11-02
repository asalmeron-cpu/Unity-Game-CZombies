using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class PlayerVida : MonoBehaviour
{


    public GameObject ola;

    [Header("Stats del jugador")]
    public float maxHealth = 100f;

    public float currentHealth;

    [Header("Puntuación")]
    public int score = 0;

    [Header("Power-ups")]
    public bool hasQuickRevive = false;

    [Header("UI")]
    public GameObject downedPanel; // Panel rojo traslúcido que se activa al tumbarse

    [Header("Referencias")]
    public PlayerMovementFPS playerMovement; // referencia al movimiento del jugador
    public float healthRegenRate = 5f; // Vida por segundo
    private bool isDowned = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (downedPanel != null)
            downedPanel.SetActive(false);

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovementFPS>();

        Debug.Log($"Jugador aparece con {currentHealth} de vida. Quick Revive: {hasQuickRevive}");
    }

    void Update()
    {
        
        // REGENERACION
        if (!isDowned && currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= amount;
        Debug.Log($"Jugador recibió {amount} de daño. Vida restante: {currentHealth}");
        downedPanel.SetActive(true);

        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            Invoke(nameof(DisablePanel), 0.5f);
        }

    }

    void Die()
    {
        Debug.Log("El jugador ha muerto.");
        GoDown();
    }

    void GoDown()
    {
        if (isDowned) return;
        isDowned = true;

        // ACTIVA PANEL ROJO
        if (downedPanel != null)
            downedPanel.SetActive(true);

        // REDUCIR VELOCIDAD
        if (playerMovement != null)
        {
            playerMovement.speed = playerMovement.sprintSpeed / 5f; // muy lento
            playerMovement.ForceProneState(); // postura tumbada
        }

        // Si tiene Quick Revive, revive después de 10 segundos
        if (hasQuickRevive)
            StartCoroutine(QuickReviveCoroutine());
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Muerte");
        }


    }

    IEnumerator QuickReviveCoroutine()
    {
        Debug.Log("Quick Revive activado, reviviendo en 10 segundos...");
        yield return new WaitForSeconds(10f);

        hasQuickRevive = false;
        currentHealth = maxHealth / 2f;
        RestoreNormalState();
    }

    void RestoreNormalState()
    {
        isDowned = false;

        if (downedPanel != null)
            downedPanel.SetActive(false);

        if (playerMovement != null)
        {
            playerMovement.speed = playerMovement.normalSpeed;
            playerMovement.ReleaseForcedState();
        }

    }

    // Puntuación
    public void AddScore(int points)
    {
        score += points;

    }

    public void RemoveScore(int points)
    {
        score -= points;
        if (score < 0) score = 0;

    }
    public int GetScore()
    {
        return score;
    }
    public void comprarquickrevive(int hola)
    {
        hasQuickRevive = true;


    }
    void DisablePanel()
    {
        downedPanel.SetActive(false);
    }

    public void Fulldie()
    {
        GoUltraDown();
    }


    public void GoUltraDown()
    {
        if (isDowned) return;
        isDowned = true;

        // ACTIVA PANEL ROJO
        if (downedPanel != null)
            downedPanel.SetActive(true);

        // REDUCIR VELOCIDAD
        if (playerMovement != null)
        {
            playerMovement.speed = playerMovement.sprintSpeed / 5f;
            playerMovement.ForceProneState();
        }
        SceneManager.LoadScene("Muerte");
    }
}
