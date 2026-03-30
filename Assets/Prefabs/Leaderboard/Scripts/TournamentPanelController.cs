using UnityEngine;
using UnityEngine.UI;

public class TournamentPanelController : MonoBehaviour
{
    public static TournamentPanelController Instance; // ✅ Singleton

    [Header("Panels")]
    public GameObject tournamentPanel;
    public GameObject gameplayUI;

    [Header("Button")]
    public Button openTournamentBtn;
    public Button closeTournamentBtn;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        openTournamentBtn.onClick.AddListener(OpenTournament);
        closeTournamentBtn.onClick.AddListener(CloseTournament);
        tournamentPanel.SetActive(false);

        OpenTournament();
    }

    public void OpenTournament()
    {
        tournamentPanel.SetActive(true);
        gameplayUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void CloseTournament()
    {
        tournamentPanel.SetActive(false);
        gameplayUI.SetActive(true);
        Time.timeScale = 1f;
    }
}