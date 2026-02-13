using UnityEngine;

public class UFOScript : MonoBehaviour
{

    bool startRight;
    public Vector3 startPos;

    public float stepDistance;
    int frames;
    void Start()
    {
         
    }

    private void OnEnable()
    {
        startRight = LeftOrRight();

        if (startRight)
            transform.position = startPos; // spawn à droite
        else
            transform.position = new Vector3 (-startPos.x, startPos.y, startPos.z); // spawn à gauche
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (frames >= 3) // Toutes les 3 frames
        {
            if (startRight) // si il commence à droite
            {
                transform.position += new Vector3(-stepDistance, 0, 0); // on va à gauche
                if (transform.position.x < -startPos.x) // si il ateint l'autre opposé de l'écran
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                transform.position += new Vector3(stepDistance, 0, 0);
                if (transform.position.x > startPos.x)
                {
                    gameObject.SetActive(false);
                }
            }
            frames = 0;
        }
        frames++;
    }

    bool LeftOrRight() // choisit si il commence à droite ou à gauche
    {
        float rng = Random.Range(0, 2);
        if (rng > 0)return true; // 50%
        return false; // 50%
        // on peut l'optimiser d'avantage ça??
    }

    public void HitUFO() // appelé quand un muissile le touche
    {
        gameObject.SetActive(false);
        EnemyManager.instance.UFODestroyed();
    }
}
