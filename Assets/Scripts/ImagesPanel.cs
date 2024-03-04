using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagesPanel : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private Image image;

    private int spriteIndex = 0;

    float initialImageWidth;
    float initialImageHeight;

    void Start()
    {
        initialImageWidth = image.rectTransform.rect.width;
        initialImageHeight = image.rectTransform.rect.height;

        image.sprite = sprites[spriteIndex];
        AdjustImageRatioToSpriteRatio(sprites[spriteIndex]);
    }

    public void DisplayNextSprite()
    {
        if (spriteIndex < sprites.Length - 1)
        {
            spriteIndex++;
            image.sprite = sprites[spriteIndex];
            AdjustImageRatioToSpriteRatio(sprites[spriteIndex]);
        }
    }

    public void DisplayPreviousSprite()
    {
        if (spriteIndex > 0)
        {
            spriteIndex--;
            image.sprite = sprites[spriteIndex];
            AdjustImageRatioToSpriteRatio(sprites[spriteIndex]);
        }
    }


    private void AdjustImageRatioToSpriteRatio(Sprite sprite)
    {
        // Chat GPT 3.5

        float spriteAspectRatio = sprite.bounds.size.x / sprite.bounds.size.y;
        
        // Calculer le nouveau rapport d'aspect pour l'Image basé sur les dimensions initiales
        float imageAspectRatio = initialImageWidth / initialImageHeight;

        float targetWidth, targetHeight;

        // Ajuster la taille de l'Image pour correspondre au sprite sans dépasser les dimensions initiales
        if (spriteAspectRatio > imageAspectRatio)
        {
            // Le sprite est plus large que haut par rapport à l'image
            targetWidth = initialImageWidth;
            targetHeight = initialImageWidth / spriteAspectRatio;
        }
        else
        {
            // Le sprite est plus haut que large par rapport à l'image
            targetWidth = initialImageHeight * spriteAspectRatio;
            targetHeight = initialImageHeight;
        }

        image.rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

        image.rectTransform.anchoredPosition = Vector2.zero;
    }








    public bool IsFirstSprite() { return spriteIndex == 0; }
    public bool IsLastSprite() { return spriteIndex == sprites.Length - 1; }
}
