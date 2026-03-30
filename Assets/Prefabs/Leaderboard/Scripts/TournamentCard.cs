using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;

public class TournamentCard : MonoBehaviour
{
    [Header("Tournament Type")]
    public TournamentType tournamentType;
    public bool isLocked = false;

    [Header("Unlocked Content")]
    public GameObject unlockedContent;
    public TMP_Text titleText;
    public TMP_Text entryText;
    public TMP_Text timerText;
    public TMP_Text prizeText;
    public TMP_Text charityText;
    public Button playButton;
    public Button leaderboardButton; // ✅ naya button

    [Header("Locked Content")]
    public GameObject lockedContent;
    public Image cardBackground;

    [Header("Colors")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.25f, 0.25f, 0.25f, 1f);

    void Start()
    {
        if (!isLocked)
        {
            playButton.onClick.AddListener(OnPlayPressed);
            leaderboardButton.onClick.AddListener(OnLeaderboardPressed);

            // Leaderboard event subscribe
            if (tournamentType == TournamentType.Weekly)
                LeaderboardManager.Instance.OnWeeklyFetched += OnLeaderboardFetched;
            else if (tournamentType == TournamentType.Daily)
                LeaderboardManager.Instance.OnDailyFetched += OnLeaderboardFetched;
            else
                LeaderboardManager.Instance.OnMonthlyFetched += OnLeaderboardFetched;
        }
    }

    public void SetUnlocked(string title, string prize, string endsIn)
    {
        isLocked = false;
        titleText.text = title;
        entryText.text = "ENTRY: 1x 🎟";
        timerText.text = "ENDS IN: " + endsIn;
        prizeText.text = "PRIZE: " + prize;
        charityText.text = "A% OF $ GOES TO CHARITY";
        unlockedContent.SetActive(true);
        lockedContent.SetActive(false);
        cardBackground.color = unlockedColor;
        playButton.interactable = true;
        leaderboardButton.interactable = true;
    }

    public void SetLocked()
    {
        isLocked = true;
        unlockedContent.SetActive(false);
        lockedContent.SetActive(true);
        cardBackground.color = lockedColor;
    }

    void OnPlayPressed()
    {
        if (isLocked) return;
        Debug.Log("Play pressed: " + tournamentType);
        TournamentManager.Instance.EnterTournament(tournamentType);
        // Tournament panel band karo — game shuru
        PausingManager.Instance.StartPlayCountdown();
        TournamentPanelController.Instance.CloseTournament();


      
    }

    void OnLeaderboardPressed()
    {
        if (isLocked) return;
        LeaderboardUIManager.Instance.OpenPanelForType(tournamentType);
    }

    void OnLeaderboardFetched(List<PlayerLeaderboardEntry> entries) { }

    void OnDestroy()
    {
        if (LeaderboardManager.Instance == null) return;
        if (tournamentType == TournamentType.Weekly)
            LeaderboardManager.Instance.OnWeeklyFetched -= OnLeaderboardFetched;
        else if (tournamentType == TournamentType.Daily)
            LeaderboardManager.Instance.OnDailyFetched -= OnLeaderboardFetched;
        else
            LeaderboardManager.Instance.OnMonthlyFetched -= OnLeaderboardFetched;
    }
}