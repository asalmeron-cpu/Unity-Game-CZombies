using UnityEngine;
using System.Collections.Generic;

public class zona1: MonoBehaviour
{
    public RoundManager roundManager;
    public List<Transform> newSpawnPoints;

    public void AbrirZona()
    {
        if (!roundManager) return;

        foreach (var point in newSpawnPoints)
        {
            if (point != null && !roundManager.spawnPoints.Contains(point))
                roundManager.spawnPoints.Add(point);
        }

        Debug.Log("Se ha abierto la zona.");
    }
}
