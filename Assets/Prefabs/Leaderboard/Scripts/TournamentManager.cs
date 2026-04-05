using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public enum TournamentType { Weekly, Daily, Monthly}

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
    public Action OnScoreSubmitted;
    public Action<string> OnError;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    // Awake ya Start mein call karo
    void Start()
    {
        InitializePlayerScore();
    }

    // ✅ Naye user ka score 0 se initialize karo — leaderboard mein appear hoga
    void InitializePlayerScore()
    {
        foreach (TournamentType type in Enum.GetValues(typeof(TournamentType)))
        {
            string statName = GetStatName(type);

            // Pehle check karo — score hai ya nahi
            PlayFabClientAPI.GetPlayerStatistics(
                new GetPlayerStatisticsRequest
                {
                    StatisticNames = new List<string> { statName }
                },
            result =>
            {
                bool hasScore = false;
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == statName)
                    {
                        hasScore = true;
                        break;
                    }
                }

                // Score nahi hai — 0 se initialize karo
                if (!hasScore)
                {
                    PlayFabClientAPI.UpdatePlayerStatistics(
                        new UpdatePlayerStatisticsRequest
                        {
                            Statistics = new List<StatisticUpdate>
                            {
                            new StatisticUpdate { StatisticName = statName, Value = 0 }
                            }
                        },
                    r => Debug.Log($"{statName} initialized with 0 for new player"),
                    e => Debug.LogError($"Init failed: " + e.ErrorMessage));
                }
            },
            error => Debug.LogError("Stats fetch failed: " + error.ErrorMessage));
        }
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
    // ✅ FIX: Ticket spend yahan nahi hogi — TournamentCard.OnPlayPressed() mein hoti hai
    // Yeh method sirf tournament state set karta hai
    public void EnterTournament(TournamentType type)
    {
        if (isInTournament)
        {
            OnError?.Invoke("Pehla tournament complete karo!");
            return;
        }

        isInTournament = true;
        hasUsedContinue = false;
        currentTournamentScore = 0;
        currentTournamentType = type;
        OnTournamentEntered?.Invoke();
        Debug.Log("Tournament entered: " + type.ToString());
    }

    // ── GAME FAILED ──

    // ── GAME FAILED ──
    public void OnGameFailed()
    {
        if (!isInTournament) return;

        if (!hasUsedContinue)
        {
            // ❌ Pehle EndTournament yahan call hoti thi — REMOVE karo
            // Player continue kar sakta hai, tournament abhi khatam nahi
            OnTournamentFailed?.Invoke();
            // isInTournament = true rehne do
        }
        else
        {
            // Continue use ho chuka hai — ab actually end karo
            EndTournament(true);
        }
    }
    //public void OnGameFailed()
    //{
    //    if (!isInTournament) return;

    //    if (!hasUsedContinue)
    //    {
    //        OnTournamentFailed?.Invoke();
    //        // ✅ Yahan bhi EndTournament call karo
    //        EndTournament(true);
    //    }
    //    else
    //    {
    //        EndTournament(true);
    //    }
    //}

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
        else
            Debug.Log("Score submit nahi hua — score: " + currentTournamentScore); // debug

        OnTournamentCompleted?.Invoke();
    }

    // ── SUBMIT SCORE ──
    void SubmitScore(int score, TournamentType type)
    {
        string statName = GetStatName(type);
        Debug.Log($"SubmitScore called — score: {score}, type: {type}");

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

            Debug.Log($"Existing score: {existingScore}, New score: {score}"); // ✅

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
                    Debug.Log($"✅ Score successfully saved: {score}"); // ✅
                    OnScoreSubmitted?.Invoke();
                    LeaderboardManager.Instance.FetchLeaderboard(currentTournamentType, 3);
                },
                e => Debug.LogError("❌ Submit failed: " + e.ErrorMessage)); // ✅
            }
            else
            {
                Debug.Log($"⚠️ Skip — new {score} <= existing {existingScore}");
            }
        },
        error => Debug.LogError("❌ Fetch failed: " + error.ErrorMessage)); // ✅
    }
}