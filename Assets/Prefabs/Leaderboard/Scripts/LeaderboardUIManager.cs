using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;

public class LeaderboardUIManager : MonoBehaviour
{
    public static LeaderboardUIManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("UI References")]
    public GameObject leaderboardPanel;
    public Transform contentParent;
    public GameObject playerRowPrefab;
    public TMP_Text tournamentNameText;
    public Button closeBtn;

    private TournamentType currentType = TournamentType.Weekly;

    void Start()
    {
        LeaderboardManager.Instance.OnWeeklyFetched += OnFetched;
        LeaderboardManager.Instance.OnDailyFetched += OnFetched;
        LeaderboardManager.Instance.OnMonthlyFetched += OnFetched;

        closeBtn.onClick.AddListener(() => leaderboardPanel.SetActive(false));
        leaderboardPanel.SetActive(false);
    }

    // ✅ Card ka leaderboard button yeh call karega
    public void OpenPanelForType(TournamentType type)
    {
        currentType = type;
        leaderboardPanel.SetActive(true);

        switch (type)
        {
            case TournamentType.Weekly:
                tournamentNameText.text = "WEEKLY LEADERBOARD";
                break;
            case TournamentType.Daily:
                tournamentNameText.text = "DAILY LEADERBOARD";
                break;
            case TournamentType.Monthly:
                tournamentNameText.text = "MONTHLY LEADERBOARD";
                break;
        }

        LeaderboardManager.Instance.FetchLeaderboard(type, 100);
    }

    public void OpenPanel()
    {
        OpenPanelForType(TournamentType.Weekly);
    }

    void OnFetched(List<PlayerLeaderboardEntry> entries)
    {
        PopulateList(entries);
    }

    void PopulateList(List<PlayerLeaderboardEntry> entries)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var entry in entries)
        {
            GameObject row = Instantiate(playerRowPrefab, contentParent);

            TMP_Text rankText = row.transform.Find("RankText").GetComponent<TMP_Text>();
            TMP_Text nameText = row.transform.Find("NameText").GetComponent<TMP_Text>();
            TMP_Text scoreText = row.transform.Find("ScoreText").GetComponent<TMP_Text>();

            rankText.text = $"#{entry.Position + 1}";
            nameText.text = entry.DisplayName ?? "Player";
            scoreText.text = entry.StatValue.ToString();
        }
    }

    void OnDestroy()
    {
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.OnWeeklyFetched -= OnFetched;
            LeaderboardManager.Instance.OnDailyFetched -= OnFetched;
            LeaderboardManager.Instance.OnMonthlyFetched -= OnFetched;
        }
    }
}