
using JetBrains.Annotations;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    public float speed = 10f;
    public float minHeight = -10f;

    public Sprite[] sprites1;
    public Sprite[] sprites2;
    public Sprite[] sprites3;
    Sprite[] chosenSprites;
    int currentSpriteIndex;
    public int animationDelay;
    int currentAnimationFrameCount;
    SpriteRenderer spriteRenderer;
    private void Start()
    {
       spriteRenderer = GetComponent<SpriteRenderer>();
       Sprite[][] allArrays = { sprites1, sprites2, sprites3 }; // array d'array de sprite
       chosenSprites = allArrays[Random.Range(0,allArrays.Length)]; // l'array de sprites du missile est choisit aléatoirement
    }
    void Update()
    {
        ChangeSprite();
        transform.Translate(Vector3.down * speed * Time.deltaTime);
       
        if (transform.position.y < minHeight) ResetMissile();
    }

    void ChangeSprite()
    {
        if (currentAnimationFrameCount < animationDelay)
        {
            
            currentAnimationFrameCount++;
            //Debug.Log(currentAnimationFrameCount);
        }
        else
        {
            // à améliorer si temps
            currentAnimationFrameCount = 0;
            if (currentSpriteIndex < chosenSprites.Length - 1)
            {
                currentSpriteIndex++;
            }
            else
            {
                currentSpriteIndex = 0;
            }
            spriteRenderer.sprite = chosenSprites[currentSpriteIndex];
        }        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);

            GameManager.instance.GameOver();
            ResetMissile();
        }
        /*if (collision.CompareTag("Shield"))
        {
            //ResetMissile();
        }*/
    }

    public void ResetMissile()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
