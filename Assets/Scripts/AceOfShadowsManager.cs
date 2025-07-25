using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
Since the instruction indicates to actually "create 144 sprites" I assume we NEED to have the 144 objects in the scene.
That being said, we wont have all 144 active at the same time, for performance, and only activate the top card and the one below.
Working similar to an object pool, but with all the elements actually in the scene (144 sprites for the cards).
*/

public class AceOfShadowsManager : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] private List<SpriteRenderer> cardsSR = new List<SpriteRenderer>();

    [Header("Transform of the stacks")]
    [SerializeField] private Transform stackOne;
    [SerializeField] private Transform stackTwo;

    [Header("Animation values")]
    [SerializeField] private float durationCardSlide = 0.5f;
    [SerializeField] private float waitTimePerCardSlide = 1.0f;

    [Header("Text cards counters")]
    [SerializeField] private TextMeshPro deckOneCounter;
    [SerializeField] private TextMeshPro deckTwoCounter;

    [Header("Finished text")]
    [SerializeField] private TextMeshPro animationDoneText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCardsSortOrder();
        UpdateCardsCounters();
        StartCoroutine(StartMovingCards());
    }

    private IEnumerator StartMovingCards()
    {
        while (stackOne.childCount > 0)
        {
            Transform card = stackOne.GetChild(stackOne.childCount - 1);
            StartCoroutine(MoveCardToTarget(card, stackTwo.position, durationCardSlide));

            // Update sort order test
            var sr = card.GetComponent<SpriteRenderer>();
            sr.sortingOrder = stackOne.childCount - 1;

            // Rotate the new first card, a little on the z axis
            if (stackOne.childCount >= 2)
            {
                Transform card2 = stackOne.GetChild(stackOne.childCount - 2);
                StartCoroutine(RotateCardAtAngle(card2, -20, 0.2f));
            }

            // Rotate previous card, a little less on the z axis for a cooler effect
            if (stackOne.childCount >= 3)
            {
                Transform card3 = stackOne.GetChild(stackOne.childCount - 3);
                StartCoroutine(RotateCardAtAngle(card3, -10, 0.4f));
            }

            // Activate the "last card" the user can see
            if (stackOne.childCount >= 4)
            {
                Transform card4 = stackOne.GetChild(stackOne.childCount - 4);
                card4.gameObject.SetActive(true);
            }

            // Re-parent immediatly
            card.SetParent(stackTwo);

            // Wait a second between each move
            yield return new WaitForSeconds(waitTimePerCardSlide);

            // After getting more that 3 cards, start hiding the ones at the bottom
            // Works like an object pool, but with all 144 cards existing in the scene
            if (stackTwo.childCount >= 3)
            {
                Transform cardToHide = stackTwo.GetChild(stackTwo.childCount - 3);
                cardToHide.gameObject.SetActive(false);
            }
        }

        Debug.Log("All cards moved.");
        animationDoneText.gameObject.SetActive(true);
    }

    private IEnumerator MoveCardToTarget(Transform card, Vector3 targetPos, float duration)
    {
        Vector3 startPos = card.position;
        Quaternion startRot = card.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            card.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            card.rotation = Quaternion.Lerp(startRot, Quaternion.identity, elapsed / duration);
            elapsed += Time.deltaTime;

            yield return null;
        }

        card.position = targetPos;
        card.rotation = Quaternion.identity;

        // Invert sort order when switching decks, since its now in reverse
        SpriteRenderer cardRenderer = card.GetComponent<SpriteRenderer>();
        cardRenderer.sortingOrder = cardRenderer.sortingOrder * -1;

        // After animation is done, update texts
        UpdateCardsCounters();
    }

    private IEnumerator RotateCardAtAngle(Transform card, float zAngle, float duration)
    {
        Quaternion startRot = card.rotation;
        Quaternion goalRot = Quaternion.Euler(0, 0, zAngle);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            card.rotation = Quaternion.Lerp(startRot, goalRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        card.rotation = goalRot;
    }

    private void UpdateCardsCounters()
    {
        deckOneCounter.text = stackOne.childCount + "";
        deckTwoCounter.text = stackTwo.childCount + "";
    }

    /// <summary>
    /// We need to do this, since 2D sprites are not rendered depending on child-position like the UI canvas
    /// So by setting the sort order, we can place the top-most card at the top of the stack.
    /// </summary>
    private void UpdateCardsSortOrder()
    {
        for (int i = 0; i < cardsSR.Count; i++)
        {
            cardsSR[i].sortingOrder = i;
        }
    }
}