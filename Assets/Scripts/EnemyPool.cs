using UnityEngine;
using System.Collections.Generic;
using System;
public class EnemyPool : MonoBehaviour
{
    [SerializeField]
    EnemyData enemyData;

    Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < enemyData.enemyTypes.Count; i++)
        {
            var enemyType = enemyData.enemyTypes[i];

            if (poolDictionary.ContainsKey(enemyType.prefab))
            {
                Debug.LogWarning($"Le prefab {enemyType.prefab.name} pour le type {enemyType.name}" + $"est déja dans le pool");
                continue;
            }

            int poolSize = GetPoolSizeForEnemyType(i);

            Queue<GameObject> enemyQueue = new Queue<GameObject>();

            for (int j = 0; j < poolSize; j++)
            {
                Debug.Log("gokdkdkdkd");
                GameObject enemy = Instantiate(enemyType.prefab);
                enemy.SetActive(false);
                enemyQueue.Enqueue(enemy);
            }

            poolDictionary.Add(enemyType.prefab, enemyQueue);
        }
    }

    public GameObject GetEnemy(GameObject prefab)
    {
        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> enemyQueue) && enemyQueue.Count > 0)
        {
            GameObject enemy = enemyQueue.Dequeue();
            enemy.SetActive(true);
            return enemy;
        }

        Debug.Log("SPISPI");
        return null;
    }

    public void ReturnToPool(GameObject enemy, GameObject prefab)
    {
        enemy.SetActive(false);

        if (poolDictionary.TryGetValue(prefab, out var enemyQueue))
        {
            enemyQueue.Enqueue(enemy);
        }
        else
        {
            Debug.Log("tentative return ennemi dans un pool existant");
        }
    }

    public List<EnemyData.EnemyType> GetEnemyTypes() 
    {
        return enemyData.enemyTypes;
    }

    private int GetPoolSizeForEnemyType(int index)
    {
        switch (index)
        {
            case 0:
                return 22;
            case 1:
                return 22;
            case 2:
                return 11;
            default:
                return 0;
        }
    }
}
