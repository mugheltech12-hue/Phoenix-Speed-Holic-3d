using System;
using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class TicketManager : MonoBehaviour
{
    public static TicketManager Instance;

    [Header("Ticket Counts")]
    public int pinkTickets = 0;
    public int goldenTickets = 0;

    [Header("Free Ticket Timer")]
    public float freeTicketIntervalHours = 12f;
    private DateTime lastFreeTicketTime;
    private Coroutine timerCoroutine;

    public Action OnTicketsUpdated;
    public Action<string> OnTimerUpdated;
    public Action OnFreeTicketReady;
    public Action<string> OnError;

    void Awake()
    {
        Instance = this;
       
    }

    void Start()
    {
        StartCoroutine(WaitForPlayFabLogin());
    }

    // ✅ FIX 1: PlayFab login ka wait karo, phir sab initialize karo
    IEnumerator WaitForPlayFabLogin()
    {
        // PlayFab login hone ka wait — jab tak SessionTicket nahi milta
        yield return new WaitUntil(() =>
            PlayFabClientAPI.IsClientLoggedIn());

        FetchTicketBalance();
        LoadFreeTicketTimer();
    }

    // =============================================
    // FETCH BALANCE FROM PLAYFAB
    // =============================================
    public void FetchTicketBalance()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result =>
        {
            pinkTickets = result.VirtualCurrency.ContainsKey("PT") ? result.VirtualCurrency["PT"] : 0;
            goldenTickets = result.VirtualCurrency.ContainsKey("GT") ? result.VirtualCurrency["GT"] : 0;
            OnTicketsUpdated?.Invoke();
            Debug.Log($"PT: {pinkTickets} | GT: {goldenTickets}");
        },
        error =>
        {
            OnError?.Invoke(error.ErrorMessage);
            Debug.LogError("Ticket fetch failed: " + error.ErrorMessage);
        });
    }

    // =============================================
    // SPEND PINK TICKET
    // =============================================
    public void SpendPinkTicket(int amount, Action onSuccess, Action onFail = null)
    {
        if (pinkTickets < amount)
        {
            OnError?.Invoke("Pink Tickets kam hain!");
            onFail?.Invoke();
            return;
        }

        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = "PT",
            Amount = amount
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(request,
        result =>
        {
            pinkTickets = result.Balance;
            OnTicketsUpdated?.Invoke();
            onSuccess?.Invoke();
        },
        error =>
        {
            OnError?.Invoke(error.ErrorMessage);
            onFail?.Invoke();
        });
    }

    // =============================================
    // ADD PINK TICKET
    // =============================================
    public void AddPinkTicket(int amount)
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "PT",
            Amount = amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request,
        result =>
        {
            pinkTickets = result.Balance;
            OnTicketsUpdated?.Invoke();
        },
        error => OnError?.Invoke(error.ErrorMessage));
    }

    // =============================================
    // ADD GOLDEN TICKET
    // =============================================
    public void AddGoldenTicket(int amount)
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "GT",
            Amount = amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request,
        result =>
        {
            goldenTickets = result.Balance;
            OnTicketsUpdated?.Invoke();
        },
        error => OnError?.Invoke(error.ErrorMessage));
    }

    // =============================================
    // FREE TICKET TIMER
    // =============================================
    public void LoadFreeTicketTimer()
    {
        string saved = PlayerPrefs.GetString("LastFreeTicketTime", "");

        if (string.IsNullOrEmpty(saved))
        {
            // Pehli baar — seedha claim allow karo
            lastFreeTicketTime = DateTime.UtcNow.AddHours(-freeTicketIntervalHours);
        }
        else
        {
            // ✅ FIX 2: Safe parsing — locale issue se bachao
            if (!DateTime.TryParse(saved, null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out lastFreeTicketTime))
            {
                // Parse fail ho toh reset karo
                Debug.LogWarning("Timer parse fail — reset kar raha hoon");
                lastFreeTicketTime = DateTime.UtcNow.AddHours(-freeTicketIntervalHours);
            }
        }

        RestartTimer();
    }

    // ✅ FIX 3: Alag method — restart karna easy ho
    void RestartTimer()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(FreeTicketTimerCoroutine());
    }

    IEnumerator FreeTicketTimerCoroutine()
    {
        while (true)
        {
            DateTime nextFreeTime = lastFreeTicketTime.AddHours(freeTicketIntervalHours);
            TimeSpan remaining = nextFreeTime - DateTime.UtcNow;

            if (remaining.TotalSeconds <= 0)
            {
                OnTimerUpdated?.Invoke("0h 0m 0s");
                OnFreeTicketReady?.Invoke();
                yield break;
            }

            int hours = (int)remaining.TotalHours;
            int minutes = remaining.Minutes;
            int seconds = remaining.Seconds;
            OnTimerUpdated?.Invoke($"{hours}h {minutes}m {seconds}s");

            yield return new WaitForSeconds(1f);
        }
    }

    public void ClaimFreeTicket()
    {
        DateTime nextFreeTime = lastFreeTicketTime.AddHours(freeTicketIntervalHours);
        if (DateTime.UtcNow < nextFreeTime)
        {
            OnError?.Invoke("Abhi ticket available nahi!");
            return;
        }

        AddPinkTicket(1);

        lastFreeTicketTime = DateTime.UtcNow;

        // ✅ FIX 2: Safe format save karo
        PlayerPrefs.SetString("LastFreeTicketTime",
            lastFreeTicketTime.ToString("O")); // "O" = Round-trip format, locale-safe
        PlayerPrefs.Save();

        RestartTimer();
        Debug.Log("Free ticket claimed!");
    }

    public void OnAdWatched()
    {
        AddPinkTicket(1);
    }
}