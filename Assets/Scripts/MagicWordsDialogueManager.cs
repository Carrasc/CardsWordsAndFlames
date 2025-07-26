using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagicWordsDialogueManager : MonoBehaviour
{
    [Header("Loading icon")]
    [SerializeField] private GameObject loadingIcon;

    [Header("Dialogue panel")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("UI Dialogue")]
    [SerializeField] private TextMeshProUGUI avatarNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image avatarLeftImage;
    [SerializeField] private Image avatarRightImage;

    List<DialogueData> dialoguesList = new List<DialogueData>();
    Dictionary<string, AvatarData> avatarsMap = new Dictionary<string, AvatarData>();

    // Best solution for emojis with textmeshpro, since we get coded words instead of unicode, map them here
    Dictionary<string, string> emojiMap = new Dictionary<string, string>
    {
        { "{satisfied}", "<sprite=\"EmojiOne\" index=0>" },
        { "{intrigued}", "<sprite=\"EmojiOne\" index=12>" },
        { "{neutral}", "<sprite=\"EmojiOne\" index=14>" },
        { "{affirmative}", "<sprite=\"EmojiOne\" index=8>" },
        { "{laughing}", "<sprite=\"EmojiOne\" index=6>" },
        { "{win}", "<sprite=\"EmojiOne\" index=11>" }
    };
    private int currentDialogueIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Loading data...
        loadingIcon.SetActive(true);
        dialoguePanel.SetActive(false);
        avatarLeftImage.gameObject.SetActive(false);
        avatarRightImage.gameObject.SetActive(false);

        // Fetch the data at runtime
        StartCoroutine(
            APIManager.Instance.GetDialoguesAndAvatars(
            (result) =>
            {
                loadingIcon.SetActive(false);
                dialoguePanel.SetActive(true);

                // Convert the dialogue array into a list, and save for when switching dialogues
                dialoguesList = new List<DialogueData>(result.dialogue);

                SetupAvatarsDict(result.avatars);
                SetDialogueData(currentDialogueIndex);
            })
        );
    }

    public void NextDialogueClicked()
    {
        // Loop if clicking next, at the end
        currentDialogueIndex = (currentDialogueIndex + 1) % dialoguesList.Count;
        SetDialogueData(currentDialogueIndex); 
    }

    private void SetDialogueData(int dialogueIndex)
    {
        dialogueText.text = ReplaceEmojis(dialoguesList[dialogueIndex].text);
        avatarNameText.text = dialoguesList[dialogueIndex].name;

        string keyAvatar = dialoguesList[dialogueIndex].name;

        // If the avatar exists, fill the sprite with the picture
        if (avatarsMap.ContainsKey(keyAvatar))
            GetAndSetAvatarImage(avatarsMap[keyAvatar]);
        else
        {
            // Avatar doesnt exist in the Dict, hide the avatar pictures in this case
            avatarLeftImage.gameObject.SetActive(false);
            avatarRightImage.gameObject.SetActive(false);
        }
    }

    private void GetAndSetAvatarImage(AvatarData avatarData)
    {
        if (avatarData.url == null || avatarData.url == "")
        {
            avatarLeftImage.gameObject.SetActive(false);
            avatarRightImage.gameObject.SetActive(false);
        }
        else
        {
            // Fetch the texture from the url...
            StartCoroutine(
                APIManager.Instance.SendGetRequestTexture(avatarData.url,
                    (result) =>
                    {
                        Sprite sprite = Sprite.Create(result, new Rect(0, 0, result.width, result.height), new Vector2(.5f, .5f));

                        // When right avatar talks, hide left avatar and viceversa
                        avatarRightImage.gameObject.SetActive(avatarData.position == "right");
                        avatarLeftImage.gameObject.SetActive(avatarData.position == "left");
                        avatarRightImage.sprite = sprite;
                        avatarLeftImage.sprite = sprite;
                    },
                    (error) =>
                    {
                        Debug.Log(error);
                        avatarLeftImage.gameObject.SetActive(false);
                        avatarRightImage.gameObject.SetActive(false);
                    })
            );
        }
    }

    /// <summary>
    /// Transform the array into a dictionary, so in each dialogue option, we can fetch the right url and position
    /// I was confused, since there are 2 Sheldon entries but the dialogue didnt indicate which to use. For this case, use 
    /// the last entry the array has (overwrites the first one).
    /// </summary>
    /// <param name="avatarsData">
    /// The array of avatarData, to transform into a Dict
    /// </param>
    private void SetupAvatarsDict(AvatarData[] avatarsData)
    {
        foreach (var d in avatarsData)
        {
            string key = d.name;
            avatarsMap[key] = d;
        }
    }

    private string ReplaceEmojis(string text)
    {
        foreach (var pair in emojiMap)
        {
            text = text.Replace(pair.Key, pair.Value);
        }

        return text;
    }
}