using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    public Action<List<PlayerLeaderboardEntry>> OnWeeklyFetched;
    public Action<List<PlayerLeaderboardEntry>> OnDailyFetched;
    public Action<List<PlayerLeaderboardEntry>> OnMonthlyFetched;
    public Action<string> OnError;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void FetchLeaderboard(TournamentType type, int maxResults = 100)
    {
        string statName = TournamentManager.Instance.GetStatName(type);

        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = statName,
                StartPosition = 0,
                MaxResultsCount = maxResults,
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true
                }
            },
        result =>
        {
            Debug.Log($"{type} leaderboard: {result.Leaderboard.Count} entries");
            if (type == TournamentType.Weekly)
                OnWeeklyFetched?.Invoke(result.Leaderboard);
            else if (type == TournamentType.Daily)
                OnDailyFetched?.Invoke(result.Leaderboard);
            else
                OnMonthlyFetched?.Invoke(result.Leaderboard);
        },
        error =>
        {
            OnError?.Invoke(error.ErrorMessage);
            Debug.LogError($"{type} error: " + error.ErrorMessage);
        });
    }
}