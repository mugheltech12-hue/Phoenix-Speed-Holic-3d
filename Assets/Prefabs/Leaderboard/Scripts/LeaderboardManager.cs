using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    public Action<List<PlayerLeaderboardEntry>> OnWeeklyFetched;
    public Action<List<PlayerLeaderboardEntry>> OnDailyFetched;
    public Action<List<PlayerLeaderboardEntry>> OnMonthlyFetched;
    public Action<TournamentType, PlayerLeaderboardEntry> OnWinnerFetched;
    public Action<string> OnError;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ── LEADERBOARD FETCH ──
    public void FetchLeaderboard(TournamentType type, int maxResults = 100)
    {
        string statName = TournamentManager.Instance.GetStatName(type);

        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = statName,
                StartPosition = 0,
                MaxResultsCount = maxResults,
                ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true }
            },
        result =>
        {
            Debug.Log($"{type} leaderboard: {result.Leaderboard.Count} entries");

            if (type == TournamentType.Weekly) OnWeeklyFetched?.Invoke(result.Leaderboard);
            else if (type == TournamentType.Daily) OnDailyFetched?.Invoke(result.Leaderboard);
            else OnMonthlyFetched?.Invoke(result.Leaderboard);
        },
        error =>
        {
            OnError?.Invoke(error.ErrorMessage);
            Debug.LogError($"{type} error: " + error.ErrorMessage);
        });
    }

    // ── WINNER FETCH ──
    public void FetchWinner(TournamentType type)
    {
        string statName = TournamentManager.Instance.GetStatName(type);

        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = statName,
                StartPosition = 0,
                MaxResultsCount = 1,
                ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true }
            },
        result =>
        {
            PlayerLeaderboardEntry winner = result.Leaderboard.Count > 0
                ? result.Leaderboard[0] : null;

            OnWinnerFetched?.Invoke(type, winner);
            Debug.Log($"{type} winner: {winner?.DisplayName ?? "None"}");
        },
        error =>
        {
            OnError?.Invoke(error.ErrorMessage);
            Debug.LogError($"Winner fetch error: " + error.ErrorMessage);
        });
    }

    // ✅ Panel ke liye alag method — direct callback, event fire nahi hoga
    public void FetchForPanel(TournamentType type, int maxResults,
        System.Action<List<PlayerLeaderboardEntry>> onComplete)
    {
        string statName = TournamentManager.Instance.GetStatName(type);

        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = statName,
                StartPosition = 0,
                MaxResultsCount = maxResults,
                ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true }
            },
        result =>
        {
            Debug.Log($"Panel fetch {type}: {result.Leaderboard.Count} entries");
            onComplete?.Invoke(result.Leaderboard);
        },
        error =>
        {
            OnError?.Invoke(error.ErrorMessage);
            Debug.LogError($"Panel fetch error: " + error.ErrorMessage);
        });
    }



    public void FetchPlayerScore(TournamentType type, Action<int, int> onResult)
    {
        string statName = TournamentManager.Instance.GetStatName(type);

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = statName,
                MaxResultsCount = 1
            },
        result =>
        {
            if (result.Leaderboard.Count > 0)
            {
                var entry = result.Leaderboard[0];
                onResult?.Invoke(entry.Position + 1, entry.StatValue); // rank, score
            }
            else
            {
                onResult?.Invoke(0, 0);
            }
        },
        error =>
        {
            Debug.LogError("Player score fetch failed: " + error.ErrorMessage);
            onResult?.Invoke(0, 0);
        });
    }
}