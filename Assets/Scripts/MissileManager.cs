using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileManager : MonoBehaviour
{
    public static MissileManager instance; // singleton pour acceder à playerMissileExpl
    [SerializeField]
    GameObject missilePrefab;
    [SerializeField]
    Transform firePoint;

    public int poolSize = 1;
    GameObject[] missilePool;
    int currentMissileIndex = 0;


    InputSystem_Actions controls;
    
    [Header("Player Missile Explosion")]
    public GameObject playerMissileExpl; // il n'y en a que un à la fois de toutes façon donc pas de pooling

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        controls = new InputSystem_Actions(); // initialiser input
        controls.Player.Fire.performed += ctx => Fire(ctx);
    }

    private void Start()
    {
        missilePool = new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            missilePool[i] = Instantiate(missilePrefab);
            missilePool[i].SetActive(false);
        }
    }
    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
    private void Fire(InputAction.CallbackContext ctx)
    {
        //Debug.Break();
        // set active le projectile sur le point
        //Debug.Log("googoo le gaga");
        if (ctx.performed && !GameManager.instance.isPaused && !PlayerScript.instance.respawnBool)
        {
            for (int i = 0; i < poolSize; i++)
            {

                int index = (currentMissileIndex + i) % poolSize; // ```
                if (!missilePool[i].activeSelf)
                {
                    missilePool[i].transform.position = firePoint.position;
                    missilePool[i].transform.rotation = firePoint.rotation;
                    missilePool[i].SetActive(true);

                    currentMissileIndex = (index + 1) % poolSize;
                    EnemyManager.instance.missilesShot++;
                    return;
                }
            }
            //Debug.Log("aucun missile dispo");
        }
    }

    public void MovePlayerMissileExpl(Vector3 pos)
    {
        playerMissileExpl.transform.position = pos;
        playerMissileExpl.SetActive(true);
    }

}
