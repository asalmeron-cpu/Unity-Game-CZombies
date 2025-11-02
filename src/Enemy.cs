using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("VIDA")]
    public float maxHealth = 100f;
    private float currentHealth;

    [HideInInspector] public RoundManager roundManager;

    [Header("DAÑO")]
    public float damageToPlayer = 45f;
    public float attackDelay = 1.5f; 
    private bool canDamage = true;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
      
    }

    public void SetHealth(float health)
    {
        maxHealth = Mathf.Max(health, 1f);
        currentHealth = maxHealth;
    
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
     
        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;


        if (roundManager != null)
            roundManager.EnemyDied();

        Destroy(gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (!canDamage || isDead) return;

        PlayerVida player = other.GetComponent<PlayerVida>();
        if (player != null)
        {
            player.TakeDamage(damageToPlayer);

            StartCoroutine(DamageCooldown());
        }
    }

    private System.Collections.IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(attackDelay);
        canDamage = true;
    }
}
