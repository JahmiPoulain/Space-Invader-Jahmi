using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public EnemyData.EnemyType enemyType;
    public int scoreData;
    public Sprite sprite01;
    public Sprite sprite02;
    SpriteRenderer spriteRenderer;
    bool isSprite01;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite01;
            isSprite01 = true;
        }
        else
        {
            //Debug.LogError("spriterenderer pas assigne");
        }
    }

    // Update is called once per frame
    public void ChangeSprite()
    {
        isSprite01 = !isSprite01;
        spriteRenderer.sprite = isSprite01 ? sprite01 : sprite02;
        // si (condition) ? vrai : faux
    }
}
