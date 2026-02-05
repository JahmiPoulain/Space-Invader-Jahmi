using System.Xml.Serialization;
using UnityEngine;

public class PixelPerfectCollision : MonoBehaviour
{
    public SpriteRenderer shieldSprite; // Sprite renderer du boublier
    Texture2D shieldTexture;            // texture du sprite
    public GameObject maskPrefab;       // missile slpash avec spritemask
    public float yOffset = 0f;          // offset vertical en espace world

    private void Start()
    {
        shieldTexture = Instantiate(shieldSprite.sprite.texture);

        shieldSprite.sprite = Sprite.Create(shieldTexture, shieldSprite.sprite.rect, new Vector2(0.5f, 0.5f), shieldSprite.sprite.pixelsPerUnit);

        if (!shieldTexture.isReadable)
        {
            Debug.Log(" texture pas lisible");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Missile"))
        {
            Debug.Log("COLLIDED");
            BoxCollider2D missileCollider = collision.GetComponent<BoxCollider2D>();
            if (missileCollider == null)
            {
                Debug.Log("pas de collider 2d missile");
                return;
            }

            if (IsPixelHitAndModify(missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                InstantiateMaskAtPosition(worldImpactPoint);

                collision.gameObject.GetComponent<PlayerMissile>()?.ResetMissile();
                collision.gameObject.GetComponent<EnemyMissile>()?.ResetMissile();
            }
        }
       
        }

    bool IsPixelHitAndModify(BoxCollider2D missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint)
    {
        worldImpactPoint = Vector2.zero;
        uvImpactPoint = Vector2.zero;

        // .bounds = les limites d'un collider
        Bounds missileBounds = missileCollider.bounds;

        Vector3 bottomLeft = shieldSprite.transform.InverseTransformPoint(missileBounds.min);

        Vector3 topRight = shieldSprite.transform.InverseTransformPoint(missileBounds.max);

        bottomLeft.y += yOffset;
        topRight.y += yOffset;


        Bounds spriteBounds = shieldSprite.sprite.bounds;
        //prendre en compte les dimensions de la texture et le rect du sprite
        Rect textureRect = shieldSprite.sprite.textureRect;

        // normalier les coords du mussile dans l'espace uv
        float uMin = (bottomLeft.x - spriteBounds.min.x) / spriteBounds.size.x;
        float vMin = (bottomLeft.y - spriteBounds.min.y) / spriteBounds.size.y;

        float uMax = (topRight.x - spriteBounds.min.x) / spriteBounds.size.x;
        float vMax = (topRight.y - spriteBounds.min.y) / spriteBounds.size.y;
        // verifier si les uv du missile sont dans les limites de la texture
        uMin = Mathf.Clamp01(uMin);
        vMin = Mathf.Clamp01(vMin);
        uMax = Mathf.Clamp01(uMax);
        vMax = Mathf.Clamp01(vMax);

        // determier le poiint d'impact uv
        uvImpactPoint = new Vector2((uMin + uMax) / 2f, (vMin + vMax) / 2f);

        worldImpactPoint = shieldSprite.transform.TransformPoint(new Vector3(spriteBounds.min.x + uvImpactPoint.x * spriteBounds.size.x, spriteBounds.min.y + uvImpactPoint.y * spriteBounds.size.y, 0));
        bool pixelModified = false;

        // parcourir les pixels touchés pas la texture
        for (float u = uMin; u <= uMax; u += 1f / shieldTexture.width)
        {
            for (float v = vMin; v <= vMax; v += 1f / shieldTexture.height)
            {
                int x = Mathf.FloorToInt(textureRect.x + u * textureRect.width);
                int y = Mathf.FloorToInt(textureRect.y + v * textureRect.height);

                //vérifier si les coods de la texture sont valide
                if (x >= 0 && x < shieldTexture.width && y >= 0 && y < shieldTexture.height)
                {
                    // lire la couleur du pixel dans la texture

                    Color pixel = shieldTexture.GetPixel(x, y);

                    if (pixel.a > 0)
                    {
                        shieldTexture.SetPixel(x, y, new Color(0,0,0,0));
                        pixelModified = true;
                    }
                }
            }
        }

        if (pixelModified)
        {
            shieldTexture.Apply();
        }

        return pixelModified;
    }

    void InstantiateMaskAtPosition(Vector2 worldPosition)
    {
        Debug.Log(worldPosition);
        GameObject maskInstance = Instantiate(maskPrefab, worldPosition, Quaternion.identity);

        // positionne le masque sur le même z que le bouclier
        maskInstance.transform.position = new Vector3(worldPosition.x, worldPosition.y, shieldSprite.transform.position.z);
        Debug.Log(shieldSprite.transform.position.z);
    }

}
