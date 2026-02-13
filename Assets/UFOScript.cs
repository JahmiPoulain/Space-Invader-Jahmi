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
            transform.position = startPos;
        else
            transform.position = new Vector3 (-startPos.x, startPos.y, startPos.z);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (frames >= 3)
        {
            if (startRight)
            {
                transform.position += new Vector3(-stepDistance, 0, 0);
                if (transform.position.x < -startPos.x)
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
        if (rng > 0)return true;
        return false;
        
    }

    public void HitUFO() // appelé quand un muissile le touche
    {
        gameObject.SetActive(false);
        EnemyManager.instance.UFODestroyed();
    }
}
