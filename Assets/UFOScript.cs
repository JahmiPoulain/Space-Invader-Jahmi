using UnityEngine;

public class UFOScript : MonoBehaviour
{

    bool startRight;
    public Vector3 startPos;

    public float speed;
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
        if (startRight)
        {
            transform.position += new Vector3( -speed, 0, 0);
            if (transform.position.x < -startPos.x)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            transform.position += new Vector3(speed, 0, 0);
            if (transform.position.x > startPos.x)
            {
                gameObject.SetActive(false);
            }
        }
    }

    bool LeftOrRight() // choisit si il commence à droite ou à gauche
    {
        float rng = Random.Range(0, 1);
        if (rng > 0)return true;
        return false;
        
    }

    public void HitUFO()
    {
        gameObject.SetActive(false);
        // ajouter score
    }
}
