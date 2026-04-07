using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentUIManager : MonoBehaviour
{
    public static TournamentUIManager Instance;

    [Header("Top Bar")]
    public TMP_Text freeTicketTimerText;
    public Button claimFreeTicketBtn;
    public Button getItNowBtn;
    public TMP_Text PinkTicket;
    public TMP_Text GoldenTicket;
    public Button leaderboardBtn;
    public Button rulesBtn;

    [Header("Tournament Cards")]
    public TournamentCard weeklyCard;
    public TournamentCard dailyCard;
    public TournamentCard monthlyCard;

    //[Header("Continue Panel")]
    //public GameObject continuePanel;
    //public TMP_Text continueCostText;
    //public Button continueBtn;
    //public Button quitBtn;

    [Header("Rules Panel")]
    public GameObject rulesPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Events
        TicketManager.Instance.OnTicketsUpdated += RefreshTicketUI;
        TicketManager.Instance.OnTimerUpdated += UpdateTimerText;
        TicketManager.Instance.OnFreeTicketReady += OnFreeTicketReady;
        //TournamentManager.Instance.OnTournamentFailed += ShowContinuePanel;

        // Buttons
        claimFreeTicketBtn.onClick.AddListener(() => TicketManager.Instance.ClaimFreeTicket());
        getItNowBtn.onClick.AddListener(OnAdBtn);
        leaderboardBtn.onClick.AddListener(() => LeaderboardUIManager.Instance.OpenPanel());
        rulesBtn.onClick.AddListener(() => rulesPanel.SetActive(!rulesPanel.activeSelf));
        //continueBtn.onClick.AddListener(OnContinue);
        //quitBtn.onClick.AddListener(OnQuit);

        // ✅ Weekly = Unlocked, Daily + Monthly = Locked
        weeklyCard.SetUnlocked("WEEKLY TOURNAMENT", "1x");
        dailyCard.SetLocked();
        monthlyCard.SetLocked();

        // Initial UI
        RefreshTicketUI();
        //continuePanel.SetActive(false);
        rulesPanel.SetActive(false);
    }

    // Weekly timer calculate karo
    string GetWeeklyTimer()
    {
        DateTime now = DateTime.UtcNow;
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0) daysUntilMonday = 7;
        DateTime nextMonday = now.Date.AddDays(daysUntilMonday);
        TimeSpan remaining = nextMonday - now;
        return $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
    }

    void RefreshTicketUI()
    {
        PinkTicket.text = $"{TicketManager.Instance.pinkTickets}x";
        GoldenTicket.text = $"{TicketManager.Instance.goldenTickets}x";
    }

    void UpdateTimerText(string timeStr)
    {
        bool ready = timeStr == "0h 0m";
        freeTicketTimerText.text = ready ? "FREE NOW!" : $"{timeStr}";
        claimFreeTicketBtn.interactable = ready;
    }

    void OnFreeTicketReady()
    {
        freeTicketTimerText.text = "FREE NOW!";
        claimFreeTicketBtn.interactable = true;
    }

    void OnAdBtn()
    {
        AdsManager.Instance.ShowRewardedVideo((result) =>
        {
            if (result == AdsResult.Finished)
            {
                TicketManager.Instance.OnAdWatched();
                Debug.Log("Player ko reward mila!");
            }
            else if (result == AdsResult.Skipped)
            {
                Debug.Log("Player ne skip kiya.");
            }
            else
            {
                Debug.Log("Ad fail ho gayi.");
            }
        });

      
    }

    //void ShowContinuePanel()
    //{
    //    continuePanel.SetActive(true);
    //    continueCostText.text = "Continue: 1 Pink Ticket";
    //}

    //void OnContinue()
    //{
    //    continuePanel.SetActive(false);
    //    TournamentManager.Instance.ContinueTournament();
    //}

    //void OnQuit()
    //{
    //    continuePanel.SetActive(false);
    //    TournamentManager.Instance.EndTournament(true);
    //}

    void OnDestroy()
    {
        if (TicketManager.Instance != null)
        {
            TicketManager.Instance.OnTicketsUpdated -= RefreshTicketUI;
            TicketManager.Instance.OnTimerUpdated -= UpdateTimerText;
            TicketManager.Instance.OnFreeTicketReady -= OnFreeTicketReady;
        }
        //if (TournamentManager.Instance != null)
        //    TournamentManager.Instance.OnTournamentFailed -= ShowContinuePanel;
    }
}