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
        closeBtn.onClick.AddListener(() => leaderboardPanel.SetActive(false));
        leaderboardPanel.SetActive(false);

        // ✅ Event subscribe NAHI karo — panel khulne par seedha fetch karega
        // Card wale events se panel update nahi hoga
    }

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

        // ✅ Panel ka apna alag fetch — 100 results, card se alag
        LeaderboardManager.Instance.FetchForPanel(type, 100, OnPanelFetched);
    }

    public void ClearPanel()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Empty message dikhao
        GameObject row = Instantiate(playerRowPrefab, contentParent);
        row.transform.Find("RankText").GetComponent<TMP_Text>().text = "";
        row.transform.Find("NameText").GetComponent<TMP_Text>().text = "Koi players nahi abhi";
        row.transform.Find("ScoreText").GetComponent<TMP_Text>().text = "";
    }

    public void OpenPanel()
    {
        OpenPanelForType(TournamentType.Weekly);
    }

    // ✅ Sirf panel ke liye callback — card wale event se nahi
    void OnPanelFetched(List<PlayerLeaderboardEntry> entries)
    {
        PopulateList(entries);
    }

    void PopulateList(List<PlayerLeaderboardEntry> entries)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (entries.Count == 0)
        {
            GameObject row = Instantiate(playerRowPrefab, contentParent);
            row.transform.Find("RankText").GetComponent<TMP_Text>().text = "";
            row.transform.Find("NameText").GetComponent<TMP_Text>().text = "Koi players nahi abhi";
            row.transform.Find("ScoreText").GetComponent<TMP_Text>().text = "";
            return;
        }

        foreach (var entry in entries)
        {
            GameObject row = Instantiate(playerRowPrefab, contentParent);

            TMP_Text rankText = row.transform.Find("RankText").GetComponent<TMP_Text>();
            TMP_Text nameText = row.transform.Find("NameText").GetComponent<TMP_Text>();
            TMP_Text scoreText = row.transform.Find("ScoreText").GetComponent<TMP_Text>();

            rankText.text = $"#{entry.Position + 1}";
            nameText.text = !string.IsNullOrEmpty(entry.DisplayName)
                ? entry.DisplayName
                : $"Player_{entry.PlayFabId?.Substring(0, 8) ?? "Unknown"}";
            scoreText.text = entry.StatValue.ToString();
        }
    }

    // OnDestroy mein kuch nahi — events subscribe hi nahi kiye
    void OnDestroy() { }
}