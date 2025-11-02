using UnityEngine;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    // --- SPAWNER ---
    public GameObject enemyPrefab;          // enemigo a spawnear
    public List<Transform> spawnPoints;     // lugares donde spawnear

    // --- RONDAS ---
    public int currentRound = 0;            // ronda actual
    public int baseEnemies = 5;             // enemigos base
    public int extraEnemiesPerRound = 2;    // enemigos extra por ronda
    public float spawnDelay = 1f;           // tiempo entre spawns
    public float nextRoundDelay = 3f;       // tiempo antes de siguiente ronda

    // --- DIFICULTAD ---
    public float baseEnemyHealth = 100f;    // vida base enemigos
    public float healthIncreasePerRound = 20f; // cuanto aumenta por ronda

    private int totalEnemiesThisRound;      // total enemigos que toca spawnear
    private int enemiesAlive;               // enemigos vivos
    private int enemiesSpawned;             // enemigos que ya spawnearon
    private float spawnTimer;               // control tiempo spawn
    private bool roundInProgress;           // si ronda esta activa

    void Start()
    {
        if(!enemyPrefab || spawnPoints == null || spawnPoints.Count == 0)
        {
            enabled = false; // desactivo si no hay enemigo o spawn
            return;
        }

        StartNextRound(); // empiezo primera ronda
    }

    void Update()
    {
        if(!roundInProgress) return; // si ronda no activa no hago nada

        spawnTimer -= Time.deltaTime; // reduzco tiempo

        // SPAWNEAR ENEMIGO
        if(enemiesSpawned < totalEnemiesThisRound && spawnTimer <= 0f)
        {
            SpawnEnemy();       // spawneo enemigo
            enemiesSpawned++;   // cuento que ya spawneo
            spawnTimer = spawnDelay; // reseteo timer
        }

        // PASAR DE RONDA SI NO HAY ENEMIGOS
        if(enemiesSpawned >= totalEnemiesThisRound && enemiesAlive <= 0)
        {
            roundInProgress = false;
            Debug.Log("Ronda " + currentRound + " completada");
            Invoke(nameof(StartNextRound), nextRoundDelay); // empiezo siguiente ronda despues de delay
        }
    }

    void StartNextRound()
    {
        currentRound++; // aumento ronda
        totalEnemiesThisRound = baseEnemies + (currentRound-1)*extraEnemiesPerRound; // calculo total enemigos
        enemiesAlive = 0; 
        enemiesSpawned = 0;
        spawnTimer = 0f;
        roundInProgress = true;

        spawnDelay = Mathf.Max(0.2f, spawnDelay - 0.1f); // disminuyo spawnDelay para aumentar dificultad
    }

    void SpawnEnemy()
    {
        // elijo un spawn random
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject newEnemy = Instantiate(enemyPrefab, point.position, point.rotation);

        EnemyHealth eh = newEnemy.GetComponent<EnemyHealth>();
        if(!eh)
        {
            Destroy(newEnemy); // si no tiene EnemyHealth lo borro
            return;
        }

        // le digo al enemigo que este round manager lo controla
        eh.roundManager = this;

        // le asigno vida segun ronda
        float newHealth = baseEnemyHealth + (currentRound-1)*healthIncreasePerRound;
        eh.SetHealth(newHealth);

        RegisterEnemy(eh); // cuento enemigo vivo
    }

    public void EnemyDied()
    {
        enemiesAlive = Mathf.Max(enemiesAlive-1, 0); // resto uno al morir
    }

    public void RegisterEnemy(EnemyHealth enemy)
    {
        enemiesAlive++; // sumo enemigo vivo
    }
}
