using System.Collections.Generic;
using UnityEngine;

public class CardPrefab : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> cardFacesSprites = new List<Sprite>();

    private void OnEnable()
    {
        // So it looks nicer and cards look different, chhose a random face card when activated
        spriteRenderer.sprite = cardFacesSprites[Random.Range(0, cardFacesSprites.Count)];
    }
}
