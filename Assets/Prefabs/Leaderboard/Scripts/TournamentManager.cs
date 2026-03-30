using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public enum TournamentType { Weekly, Daily, Monthly }

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager Instance;

    public bool isInTournament = false;
    public bool hasUsedContinue = false;
    public int currentTournamentScore = 0;
    public TournamentType currentTournamentType;

    public Action OnTournamentEntered;
    public Action OnTournamentFailed;
    public Action OnTournamentCompleted;
    public Action<string> OnError;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public string GetStatName(TournamentType type)
    {
        switch (type)
        {
            case TournamentType.Weekly: return "WeeklyTournamentScore";
            case TournamentType.Daily: return "DailyTournamentScore";
            case TournamentType.Monthly: return "MonthlyTournamentScore";
            default: return "WeeklyTournamentScore";
        }
    }

    // ── ENTER TOURNAMENT ──
    public void EnterTournament(TournamentType type)
    {
        if (isInTournament) { OnError?.Invoke("Pehla tournament complete karo!"); return; }

        TicketManager.Instance.SpendPinkTicket(1,
        onSuccess: () =>
        {
            isInTournament = true;
            hasUsedContinue = false;
            currentTournamentScore = 0;
            currentTournamentType = type;
            OnTournamentEntered?.Invoke();
            Debug.Log("Tournament entered: " + type.ToString());
        },
        onFail: () => OnError?.Invoke("Pink Ticket nahi! Pehle ticket lo."));
    }

    // ── GAME FAILED ──
    public void OnGameFailed()
    {
        if (!isInTournament) return;
        if (!hasUsedContinue)
            OnTournamentFailed?.Invoke();
        else
            EndTournament(true);
    }

    // ── CONTINUE ──
    public void ContinueTournament()
    {
        if (hasUsedContinue) { OnError?.Invoke("Sirf 1 baar continue!"); return; }

        TicketManager.Instance.SpendPinkTicket(1,
        onSuccess: () => { hasUsedContinue = true; Debug.Log("Continue used!"); },
        onFail: () => { OnError?.Invoke("PT nahi!"); EndTournament(true); });
    }

    // ── SCORE UPDATE ──
    public void UpdateScore(int score)
    {
        if (!isInTournament) return;
        currentTournamentScore = Mathf.Max(currentTournamentScore, score);
    }

    // ── END TOURNAMENT ──
    public void EndTournament(bool submitScore)
    {
        if (!isInTournament) return;
        isInTournament = false;

        if (submitScore && currentTournamentScore > 0)
            SubmitScore(currentTournamentScore, currentTournamentType);

        OnTournamentCompleted?.Invoke();
    }

    // ── SUBMIT SCORE ──
    void SubmitScore(int score, TournamentType type)
    {
        string statName = GetStatName(type);

        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest
            {
                StatisticNames = new List<string> { statName }
            },
        result =>
        {
            int existingScore = 0;
            foreach (var stat in result.Statistics)
            {
                if (stat.StatisticName == statName)
                {
                    existingScore = stat.Value;
                    break;
                }
            }

            Debug.Log($"Existing: {existingScore} | New: {score}");

            if (score > existingScore)
            {
                PlayFabClientAPI.UpdatePlayerStatistics(
                    new UpdatePlayerStatisticsRequest
                    {
                        Statistics = new List<StatisticUpdate>
                        {
                            new StatisticUpdate { StatisticName = statName, Value = score }
                        }
                    },
                r =>
                {
                    Debug.Log($"{statName} updated: {score}");
                    CheckIfWinner(score, type);
                },
                e => Debug.LogError("Submit failed: " + e.ErrorMessage));
            }
            else
            {
                Debug.Log($"Score {score} purane {existingScore} se kam — skip!");
            }
        },
        error => Debug.LogError("Fetch failed: " + error.ErrorMessage));
    }

    // ── WINNER CHECK ──
    void CheckIfWinner(int myScore, TournamentType type)
    {
        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = GetStatName(type),
                StartPosition = 0,
                MaxResultsCount = 1
            },
        result =>
        {
            if (result.Leaderboard.Count > 0 && myScore >= result.Leaderboard[0].StatValue)
            {
                TicketManager.Instance.AddGoldenTicket(1);
                Debug.Log("Winner! Golden Ticket mila!");
            }
        },
        error => Debug.LogError("Winner check failed: " + error.ErrorMessage));
    }
}