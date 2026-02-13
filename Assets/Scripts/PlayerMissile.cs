using System;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{

    public float speed = 10f;
    public float maxHeight = 8f;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y < maxHeight) return;
        MissileManager.instance.MovePlayerMissileExpl(transform.position); // exploser le missile
        ResetMissile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Break();
        if (collision.CompareTag("Enemy"))
        {
            EnemyManager enemyManager = FindFirstObjectByType<EnemyManager>();
            if (enemyManager != null)
            {
                GameObject go = collision.GetComponent<EnemyScript>().enemyType.prefab; 
                enemyManager.ReturnEnemy(collision.gameObject, go);
            }
            ResetMissile();
        }    
        else if (collision.CompareTag("UFO"))
        {
            UFOScript UFOscript = collision.GetComponent<UFOScript>();
            if (UFOscript != null)
            {
                UFOscript.HitUFO();
            }
            ResetMissile();
        }
    }

    public void ResetMissile()
    {
        gameObject.SetActive(false);
    }
}
