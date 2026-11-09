using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// End screen from the scene Canvas: title, message and Restart button.
public class GameUI : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text message;
    [SerializeField] private Button restartButton;
    [SerializeField] private TMP_Text buttonLabel;
    [SerializeField] private TMP_Text levelText;

    [SerializeField] private float fadeTime = 0.5f;

    [Header("Texts")]
    [SerializeField] private string winTitle = "Victory!";
    [SerializeField] private string loseTitle = "Game Over";
    [SerializeField] private string nextLevelText = "Next Level";
    [SerializeField] private string restartText = "Restart";
    [SerializeField] private Color winColor = new Color(1f, 0.75f, 0.3f);
    [SerializeField] private Color loseColor = new Color(0.75f, 0.6f, 1f);

    private void Awake()
    {
        endScreen.SetActive(false);
        restartButton.onClick.AddListener(Restart);
    }

    private void Start()
    {
        levelText.text = $"Level {LevelProgress.Current}";
    }

    private void OnEnable()
    {
        game.Finished += Show;
    }

    private void OnDisable()
    {
        game.Finished -= Show;
    }

    private void Show(bool win, string text)
    {
        title.text = win ? winTitle : loseTitle;
        title.color = win ? winColor : loseColor;
        message.text = text;
        buttonLabel.text = win ? nextLevelText : restartText;
        endScreen.SetActive(true);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        if (!endScreen.TryGetComponent(out CanvasGroup group)) group = endScreen.AddComponent<CanvasGroup>();
        for (float t = 0f; t < 1f; t += Time.deltaTime / fadeTime)
        {
            group.alpha = t;
            yield return null;
        }
        group.alpha = 1f;
    }

    // same scene for every level, but with new obstacles
    private static void Restart()
    {
        LevelGenerator.RegenerateOnLoad = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
