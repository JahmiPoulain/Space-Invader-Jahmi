using UnityEngine;


public class GeneralExplosion : MonoBehaviour
{
    // dure 16 frames puis est désactivé
    // sert pour les explosion d'ennemi et de missile
    int frames = 0;
    private void FixedUpdate()
    {
        // se désactive tout seul après être resté assez de frames
        if (frames >= 17)
        {
            transform.position = new Vector3 (100, 100, 0);
            frames = 0;
            gameObject.SetActive(false);
        }
        frames++;
    }
}
