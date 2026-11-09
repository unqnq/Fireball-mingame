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

    [Header("Texts")]
    [SerializeField] private string winTitle = "Victory!";
    [SerializeField] private string loseTitle = "Game Over";
    [SerializeField] private Color winColor = new Color(1f, 0.75f, 0.3f);
    [SerializeField] private Color loseColor = new Color(0.75f, 0.6f, 1f);

    private void Awake()
    {
        endScreen.SetActive(false);
        restartButton.onClick.AddListener(Restart);
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
        endScreen.SetActive(true);
    }

    private static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
