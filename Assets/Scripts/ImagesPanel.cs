using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagesPanel : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private Image image;

    private int spriteIndex = 0;



    void Start()
    {
        image.sprite = sprites[spriteIndex];
    }

    public void DisplayNextSprite()
    {
        if (spriteIndex < sprites.Length - 1)
        {
            spriteIndex++;
            image.sprite = sprites[spriteIndex];
        }
    }

    public void DisplayPreviousSprite()
    {
        if (spriteIndex > 0)
        {
            spriteIndex--;
            image.sprite = sprites[spriteIndex];
        }
    }

    public bool IsFirstSprite() { return spriteIndex == 0; }
    public bool IsLastSprite() { return spriteIndex == sprites.Length - 1; }
}
