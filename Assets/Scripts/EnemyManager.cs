
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    [SerializeField]
    GameObject player;
        float playerBoundaryX;
    public EnemyPool EnemyPool;
    public int rows = 5;
    public int columns = 11;
    public float spacing = 1.5f;
    public float _stepDistance = 0.5f;
    public float _stepDistIncreasePerWave = 0.1f;
    public float stepDistanceVertical = 1f;

    public Vector2 startPosition = new Vector2(-6.5f, 7.5f);
    GameObject[,] enemies;
    public int remainingEnemies;

    bool isPaused = false;
    public bool isExploding = false;

    enum MoveState {MoveRight, MoveLeft}
    MoveState currentState = MoveState.MoveRight;

    public GameObject missilePrefab;
    public Transform missilePoint;
    public float missileInterval = 2f;

    public int explosionDuration = 17;

    bool firstWave = true;

    [Header("Enemy Explosion")]
    GameObject[] enemyExplPool;
    public GameObject enemyExplPrefab;
    public int enemyExplPoolSize;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        enemies = new GameObject[rows, columns];

        StartCoroutine(SpawnEnemies());        

        StartCoroutine(EnemyShooting());

        enemyExplPool = new GameObject[enemyExplPoolSize];
        for (int i = 0; i < enemyExplPoolSize; i++)
        {
            enemyExplPool[i] = Instantiate(enemyExplPrefab, new Vector3(100, 100, 0), Quaternion.identity);
            enemyExplPool[i].SetActive(false);
        }
    }

    IEnumerator SpawnEnemies()
    {
        var enemyTypes = EnemyPool.GetEnemyTypes();
        for (int row = rows - 1; row >= 0; row--)
        {
            var enemyType = GetEnemyTypForRow(row, enemyTypes);
            for (int col = 0; col < columns; col++)
            {
                GameObject enemy = EnemyPool.GetEnemy(enemyType.prefab);

                if ( enemy != null)
                {
                    float xPos = startPosition.x + (col * spacing);
                    float yPos = startPosition.y - (row * spacing);

                    enemy.transform.position = new Vector3(xPos, yPos, 0);

                    EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.enemyType = enemyType;
                        enemyScript.scoreData = enemyType.points;
                    }

                    enemies[row, col] = enemy;

                    remainingEnemies++;
                    yield return null;
                }
            }
        }
        //StopCoroutine(HandleEnemyMovement());
        if (!firstWave)
        {
            _stepDistance += _stepDistIncreasePerWave; // les ennemis vont plus vite            
        }
        else
        {
            StartCoroutine(HandleEnemyMovement()); // à la première vague on lance la coroutine de mouvement
            firstWave = false;
        }
        
    }



    IEnumerator HandleEnemyMovement()
    {
        //yield return new WaitForSeconds(1);
        while (remainingEnemies > 0)
        {
            bool boundaryReached = false;

            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (GameManager.instance.isPaused || isExploding)
                    {
                        yield return new WaitUntil(() => !GameManager.instance.isPaused && !isExploding); // si on est pas en pause ou explose
                    }
                    if (enemies[row, col] != null && enemies[row, col].activeSelf) // si il u a un ennemi et qu'il est actif                   
                    {                        
                        Vector3 direction = currentState == MoveState.MoveRight ? Vector3.right : Vector3.left;

                        MoveEnemy(enemies[row, col], direction, _stepDistance);

                        if (enemies[row, col] == null) continue;
                        EnemyScript enemyScript = enemies[row,col].GetComponent<EnemyScript>();
                        if (enemyScript != null) enemyScript.ChangeSprite();

                        if (ReachedBoundary(enemies[row, col])) boundaryReached = true;
                        //Debug.Log("dghjklgergjklergmergjegjsfgjfdklfdgsjfgjlfdgjlfdjklgfdjklfdgjklfdfdsjklfdgjklsdgjklfdssfdgljkfdgsfdg");
                        yield return null;
                    }                    
                }
            }

            if (boundaryReached)
            {
                yield return MoveAllEnnemiesDown();
                currentState = currentState == MoveState.MoveRight ? MoveState.MoveLeft : MoveState.MoveRight;
            }
        }
    }

    IEnumerator MoveAllEnnemiesDown()
    {
        //yield return new WaitForSeconds(1);
         for (int row = rows - 1; row >= 0; row--)
         {
             for (int col = 0; col < columns; col++)
             {

                 if (enemies[row, col] != null && enemies[row, col].activeSelf)
                 {
                     yield return new WaitUntil(() => !GameManager.instance.isPaused && !isExploding);

                     GameObject enemy = enemies[row, col];
                     if (enemy != null)
                     {
                         EnemyScript enemyScript = enemies[row, col].GetComponent<EnemyScript>();
                         if (enemyScript != null) enemyScript.ChangeSprite();
                     }
                     MoveEnemy(enemies[row, col], Vector3.down, stepDistanceVertical);
                     yield return null;

                 }

             }
         }
         
    }

    IEnumerator EnemyShooting()
    {
        //yield return new WaitForSeconds(1);
        while (true)
        {
            //Debug.Log("gagagagagaggaagagaggagagaga");
            yield return new WaitUntil(() => !GameManager.instance.isPaused && !isExploding);

            yield return new WaitForSeconds(Random.Range(missileInterval, missileInterval * 2));

            List<GameObject> shooters = GetBottomEnemies();

            if (shooters.Count > 0 && !GameManager.instance.isPaused && !isExploding)
            {
                GameObject shooter = shooters[Random.Range(0 , shooters.Count)];

                FireMissile(shooter);
                
            }
        }
    }

    List<GameObject> GetBottomEnemies()
    {
        List<GameObject> bottomEnemies = new List<GameObject>();
        for (int col = 0; col < columns; col++)
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                if(enemies[row, col] != null && enemies[row, col].activeSelf)
                {
                    bottomEnemies.Add(enemies[row, col]);
                    break;
                }
            }
        }
        return bottomEnemies;
    }

    void FireMissile(GameObject shooter)
    {
        Transform firePoint = shooter.transform.Find("FirePoint");
        if (firePoint != null)
        {
            Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("FireMissile(): FirePoint pas trouvé");
        }
    }

    void MoveEnemy(GameObject enemy, Vector3 direction, float stepDistance)
    {
        if (enemy == null) return;

        Vector3 newPosition = enemy.transform.position + direction * stepDistance;

        newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f; // garde 2 chiffres apres la virgule; 1000 garde 3 ;10000 garde 4
        newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f; // 
        newPosition.z = Mathf.Round(newPosition.z * 100f) / 100f;

        enemy.transform.position = newPosition;
    }

    public void ReturnEnemy(GameObject enemy, GameObject prefab)
    {
        ExplodeEnemy(enemy.transform.position);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (enemies[row, col] == enemy)
                {
                    enemies[row, col] = null;
                }
            }
        }

        GameManager.instance.AddScore(enemy.GetComponent<EnemyScript>().scoreData);

        if (!isExploding)
        {
            StartCoroutine(ExplosionCoroutine());
        }

        EnemyPool.ReturnToPool(enemy, prefab);

        remainingEnemies--;

        if(remainingEnemies <= 0) 
        {
            GameManager.instance.CompletedLevel();
            StartCoroutine(SpawnEnemies());
        }
    }

    void ExplodeEnemy(Vector3 pos)
    {
        // si une explosion est inactive on l'active à l'endroit ou était l'ennemi détruit
        for (int i = 0; i < enemyExplPoolSize; i++)
        {
            if (!enemyExplPool[i].activeSelf)
            {
                enemyExplPool[i].transform.position = pos;
                enemyExplPool[i].SetActive(true);
                return;
            }
        }
    }

    IEnumerator ExplosionCoroutine()
    {
        isExploding = true;
        int duration = explosionDuration;

        while (duration > 0)
        {
            duration--;
            yield return new WaitForEndOfFrame();
        }
        /*for (int i = explosionDuration; i <= 0; i--)
        {
            Debug.Log(i);
            yield return new WaitForEndOfFrame();
        }*/
        isExploding = false;
    }

    bool ReachedBoundary(GameObject enemy)
    {
        float xPos = enemy.transform.position.x;

        if (currentState == MoveState.MoveRight && xPos >= playerBoundaryX)
        {
            return true;
        }

        if (currentState == MoveState.MoveLeft && xPos <= -playerBoundaryX)
        {
            return true;
        }

        return false;
    }
    EnemyData.EnemyType GetEnemyTypForRow(int row, List<EnemyData.EnemyType> enemyTypes)
    {
        if (row == 0)
        {
            return enemyTypes[2];
        }
        else if (row <= 2) 
        {
            return enemyTypes[1];
        }
        else
        {
            return enemyTypes[0];
        }
    }
}
