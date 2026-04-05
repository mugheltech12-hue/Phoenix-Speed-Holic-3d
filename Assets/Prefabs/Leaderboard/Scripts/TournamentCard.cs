using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public TMP_Text Rank1Text;
    public TMP_Text Rank2Text;
    public TMP_Text Rank3Text;
    public TMP_Text PlayerScore;
    public Button playButton;

    [Header("Locked Content")]
    public GameObject lockedContent;
    public Image cardBackground;

    [Header("Colors")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.25f, 0.25f, 0.25f, 1f);

    private DateTime tournamentEndTime;
    private Coroutine timerCoroutine;
    private bool tournamentEnded = false;
    private bool pendingTimerStart = false;

    public static bool PendingLeaderboardOpen = false;
    public static TournamentType PendingLeaderboardType_Static;

    // ── START ──
    void Start()
    {
        if (!isLocked)
        {
            playButton.onClick.AddListener(OnPlayPressed);

            // Pehle subscribe karo, phir fetch
            if (tournamentType == TournamentType.Weekly)
                LeaderboardManager.Instance.OnWeeklyFetched += OnLeaderboardFetched;
            else if (tournamentType == TournamentType.Daily)
                LeaderboardManager.Instance.OnDailyFetched += OnLeaderboardFetched;
            else
                LeaderboardManager.Instance.OnMonthlyFetched += OnLeaderboardFetched;

            TournamentManager.Instance.OnScoreSubmitted += () =>
            {
                LeaderboardManager.Instance.FetchLeaderboard(tournamentType, 3);

                LeaderboardManager.Instance.FetchPlayerScore(tournamentType, (rank, score) =>
                {
                    if (PlayerScore != null)
                        PlayerScore.text = score > 0 ? $"MY SCORE: {score}" : "MY SCORE: --";
                });
            };

            SetUnlocked("WEEKLY TOURNAMENT", "$50 Gift Card");
        }
    }

    // ── SET UNLOCKED ──
    public void SetUnlocked(string title, string prize)
    {
        isLocked = false;
        titleText.text = title;
        entryText.text = "1x";
        prizeText.text = prize;
        charityText.text = "A% OF $ GOES TO CHARITY";
        unlockedContent.SetActive(true);
        lockedContent.SetActive(false);
        cardBackground.color = unlockedColor;
        playButton.interactable = true;

        // PlayFab se end time fetch karo
        if (gameObject.activeInHierarchy)
            FetchTournamentEndTimeFromPlayFab();
        else
            pendingTimerStart = true;
    }

    // ── SET LOCKED ──
    public void SetLocked()
    {
        isLocked = true;
        unlockedContent.SetActive(false);
        lockedContent.SetActive(true);
        cardBackground.color = lockedColor;
    }

    // ── ON ENABLE ──
    void OnEnable()
    {
        if (pendingTimerStart)
        {
            pendingTimerStart = false;
            FetchTournamentEndTimeFromPlayFab();
        }

        if (!isLocked && LeaderboardManager.Instance != null && !tournamentEnded)
        {
            LeaderboardManager.Instance.FetchLeaderboard(tournamentType, 3);

            LeaderboardManager.Instance.FetchPlayerScore(tournamentType, (rank, score) =>
            {
                if (PlayerScore != null)
                    PlayerScore.text = score > 0 ? $"MY SCORE: {score}" : "MY SCORE: --";
            });
        }
    }
    // ── PLAYFAB SE END TIME FETCH ──
    void FetchTournamentEndTimeFromPlayFab()
    {
        if (timerText != null)
            timerText.text = "Loading...";

        var request = new GetPlayerStatisticVersionsRequest
        {
            StatisticName = GetStatisticName(tournamentType)
        };

        PlayFabClientAPI.GetPlayerStatisticVersions(request,
            result =>
            {
                if (result.StatisticVersions != null && result.StatisticVersions.Count > 0)
                {
                    var latest = result.StatisticVersions[result.StatisticVersions.Count - 1];

                    if (latest.ScheduledDeactivationTime.HasValue)
                    {
                        tournamentEndTime = latest.ScheduledDeactivationTime.Value.ToUniversalTime();
                        Debug.Log($"[TournamentCard] {tournamentType} PlayFab end time: {tournamentEndTime}");
                    }
                    else
                    {
                        // Fallback: PlayFab ne time nahi diya (manually reset wala case)
                        tournamentEndTime = GetFallbackEndTime(tournamentType);
                        Debug.LogWarning($"[TournamentCard] {tournamentType} ScheduledDeactivationTime null — fallback use ho raha hai: {tournamentEndTime}");
                    }
                }
                else
                {
                    tournamentEndTime = GetFallbackEndTime(tournamentType);
                    Debug.LogWarning($"[TournamentCard] {tournamentType} No statistic versions found — fallback use ho raha hai");
                }

                // Timer start karo
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);
                timerCoroutine = StartCoroutine(TimerCountdown());
            },
            error =>
            {
                Debug.LogError($"[TournamentCard] GetPlayerStatisticVersions error: {error.GenerateErrorReport()}");

                // Fallback on error
                tournamentEndTime = GetFallbackEndTime(tournamentType);
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);
                timerCoroutine = StartCoroutine(TimerCountdown());
            }
        );
    }

    // ── STATISTIC NAME ──


    string GetStatisticName(TournamentType type)
    {
        switch (type)
        {
            case TournamentType.Weekly: return "WeeklyTournamentScore";
            case TournamentType.Daily: return "DailyTournamentScore";
            case TournamentType.Monthly: return "MonthlyTournamentScore";
            default: return "WeeklyTournamentScore";
        }
    }

    // ── FALLBACK END TIME (agar PlayFab ne time na diya) ──
    DateTime GetFallbackEndTime(TournamentType type)
    {
        DateTime now = DateTime.UtcNow;
        switch (type)
        {
            case TournamentType.Weekly:
                int daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                if (daysUntilMonday == 0) daysUntilMonday = 7;
                return now.Date.AddDays(daysUntilMonday);
                //return DateTime.UtcNow.AddMinutes(3); // Testing
            case TournamentType.Daily:
                return now.Date.AddDays(1);
            case TournamentType.Monthly:
                return new DateTime(now.Year, now.Month, 1).AddMonths(1);
            default:
                return now.AddDays(7);
        }
    }

    // ── TIMER COUNTDOWN ──
    // ── TIMER COUNTDOWN ──
    IEnumerator TimerCountdown()
    {
        Debug.Log($"[TournamentCard] Timer started. End time: {tournamentEndTime}");

        while (true)
        {
            TimeSpan remaining = tournamentEndTime - DateTime.UtcNow;

            if (remaining.TotalSeconds <= 0)
            {
                if (timerText != null)
                    timerText.text = "TOURNAMENT ENDED";
                OnTournamentEnded();
                yield break;
            }
            else
            {
                int days = (int)remaining.TotalDays;
                int hours = remaining.Hours;
                int minutes = remaining.Minutes;

                if (days > 0)
                    timerText.text = $"ENDS IN: {days}d {hours}h {minutes}m";
                else if (hours > 0)
                    timerText.text = $"ENDS IN: {hours}h {minutes}m";
                else
                    timerText.text = $"ENDS IN: {minutes}m";
            }

            yield return new WaitForSeconds(60f); // Seconds nahi chahiye to 60f kafi hai
        }
    }
    // ── TOURNAMENT ENDED ──
    void OnTournamentEnded()
    {
        if (tournamentEnded) return;
        tournamentEnded = true;

        if (TournamentManager.Instance.isInTournament)
            TournamentManager.Instance.OnScoreSubmitted += OnScoreSubmittedThenOpen;
        else
            OpenLeaderboardOrNotify();

        // Thodi delay ke baad restart
        StartCoroutine(RestartTournamentAfterDelay(10f));
    }

    // ── RESTART AFTER DELAY ──
    IEnumerator RestartTournamentAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        tournamentEnded = false;

        // PlayFab se naya time fetch karo (wo reset kar chuka hoga)
        FetchTournamentEndTimeFromPlayFab();

        //LeaderboardManager.Instance.FetchLeaderboard(tournamentType, 3);

        Debug.Log($"[TournamentCard] {tournamentType} Tournament restart ho gaya!");
    }

    // ── SCORE SUBMIT THEN OPEN ──
    void OnScoreSubmittedThenOpen()
    {
        TournamentManager.Instance.OnScoreSubmitted -= OnScoreSubmittedThenOpen;
        LeaderboardManager.Instance.FetchLeaderboard(tournamentType, 3);
        OpenLeaderboardOrNotify();
    }

    // ── OPEN LEADERBOARD OR NOTIFY ──
    void OpenLeaderboardOrNotify()
    {
        LeaderboardManager.Instance.OnWinnerFetched += OnWinnerFetched;
        LeaderboardManager.Instance.FetchWinner(tournamentType);

        bool gameRunning = PausingManager.Instance != null
            && PausingManager.Instance.isGameActive;

        if (gameRunning)
        {
            ShowTournamentEndedMessage();
            PendingLeaderboardOpen = true;
            PendingLeaderboardType_Static = tournamentType;
        }
    }

    // ── NOTIFICATION ──
    void ShowTournamentEndedMessage()
    {
        // Apna notification UI yahan connect karo agar ho
    }

    // ── WINNER CHECK ──
    void OnWinnerFetched(TournamentType type, PlayerLeaderboardEntry winner)
    {
        if (type != tournamentType) return;
        LeaderboardManager.Instance.OnWinnerFetched -= OnWinnerFetched;

        if (winner == null) return;

        string myPlayFabId = PlayFabSettings.staticPlayer.PlayFabId;
        if (!string.IsNullOrEmpty(myPlayFabId) && winner.PlayFabId == myPlayFabId)
        {
            TicketManager.Instance.AddGoldenTicket(1);
            Debug.Log($"[TournamentCard] {tournamentType} Winner! Golden Ticket mila!");
        }

        ClearTopThree();
    }

    void ClearTopThree()
    {
        TMP_Text[] rankTexts = { Rank1Text, Rank2Text, Rank3Text };
        for (int i = 0; i < rankTexts.Length; i++)
        {
            if (rankTexts[i] != null)
                rankTexts[i].text = $"#{i + 1}  ---  0";
        }
        if (PlayerScore != null)
            PlayerScore.text = "MY SCORE: --";

        // ✅ Leaderboard panel bhi clear karo
        if (LeaderboardUIManager.Instance != null)
            LeaderboardUIManager.Instance.ClearPanel();
    }

    // ── PLAY BUTTON ──
    void OnPlayPressed()
    {
        if (isLocked) return;

        TicketManager.Instance.SpendPinkTicket(1,
            onSuccess: () =>
            {


                BlinkImage.Instance?.ChangeColor();

                PausingManager.Instance.ResetUIForNewGame();
                GameManager.Instance.ResetGame();

                TournamentManager.Instance.EnterTournament(tournamentType);
                PausingManager.Instance.StartPlayCountdown();
                TournamentPanelController.Instance.CloseTournament();

                Button pauseBtn = PausingManager.Instance.ppPauseButton.GetComponent<Button>();
                if (!pauseBtn.interactable)
                    pauseBtn.interactable = true;
            },
            onFail: () => Debug.Log("[TournamentCard] Pink Ticket nahi hai!")
        );
    }

    // ── LEADERBOARD BUTTON ──
    void OnLeaderboardPressed()
    {
        if (isLocked || !tournamentEnded) return;
        LeaderboardUIManager.Instance.OpenPanelForType(tournamentType);
    }

    // ── TOP 3 UPDATE ──
    void OnLeaderboardFetched(List<PlayerLeaderboardEntry> entries)
    {
        TMP_Text[] rankTexts = { Rank1Text, Rank2Text, Rank3Text };

        for (int i = 0; i < rankTexts.Length; i++)
        {
            if (rankTexts[i] == null) continue;

            if (i < entries.Count)
            {
                var entry = entries[i];
                string displayName = entry.DisplayName ?? "Player";
                rankTexts[i].text = $"#{entry.Position + 1}  {displayName}  {entry.StatValue}";
            }
            else
            {
                rankTexts[i].text = $"#{i + 1}  ---  0";
            }
        }
    }

    // ── ON DESTROY ──
    void OnDestroy()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        if (LeaderboardManager.Instance != null)
        {
            if (tournamentType == TournamentType.Weekly)
                LeaderboardManager.Instance.OnWeeklyFetched -= OnLeaderboardFetched;
            else if (tournamentType == TournamentType.Daily)
                LeaderboardManager.Instance.OnDailyFetched -= OnLeaderboardFetched;
            else
                LeaderboardManager.Instance.OnMonthlyFetched -= OnLeaderboardFetched;

            LeaderboardManager.Instance.OnWinnerFetched -= OnWinnerFetched;
        }

        if (TournamentManager.Instance != null)
            TournamentManager.Instance.OnScoreSubmitted -= OnScoreSubmittedThenOpen;
    }
}