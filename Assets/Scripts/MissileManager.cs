using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileManager : MonoBehaviour
{
    [SerializeField]
    GameObject missilePrefab;
    [SerializeField]
    Transform firePoint;

    public int poolSize = 1;
    GameObject[] missilePool;
    int currentMissileIndex = 0;

    InputSystem_Actions controls;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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
        if (ctx.performed && !GameManager.instance.isPaused)
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

                    return;
                }
            }
            //Debug.Log("aucun missile dispo");
        }
    }

    // Update is called once per frame-
    void Update()
    {
        
    }

}
