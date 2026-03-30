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

    // Events — UI inhe sun ke update hogi
    public Action OnTicketsUpdated;
    public Action<string> OnTimerUpdated;  // "2h 30m" format
    public Action OnFreeTicketReady;
    public Action<string> OnError;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
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
            Debug.Log($"PT spent! Remaining: {pinkTickets}");
        },
        error =>
        {
            OnError?.Invoke(error.ErrorMessage);
            onFail?.Invoke();
        });
    }

    // =============================================
    // ADD PINK TICKET (ad reward ya free claim)
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
            Debug.Log($"PT added! Total: {pinkTickets}");
        },
        error => OnError?.Invoke(error.ErrorMessage));
    }

    // =============================================
    // ADD GOLDEN TICKET (tournament win pe)
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
            Debug.Log($"GT added! Total: {goldenTickets}");
        },
        error => OnError?.Invoke(error.ErrorMessage));
    }

    // =============================================
    // FREE TICKET TIMER (12 hrs)
    // =============================================
    public void LoadFreeTicketTimer()
    {
        string saved = PlayerPrefs.GetString("LastFreeTicketTime", "");
        if (string.IsNullOrEmpty(saved))
        {
            // Pehli baar — abhi claim karo
            lastFreeTicketTime = DateTime.UtcNow.AddHours(-freeTicketIntervalHours);
        }
        else
        {
            lastFreeTicketTime = DateTime.Parse(saved);
        }

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
                // Ready hai claim karne ke liye
                OnTimerUpdated?.Invoke("0h 0m");
                OnFreeTicketReady?.Invoke();
                yield break;
            }
            else
            {
                string timeStr = $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
                OnTimerUpdated?.Invoke(timeStr);
            }

            yield return new WaitForSeconds(60f); // har minute update
        }
    }

    public void ClaimFreeTicket()
    {
        // Timer check
        DateTime nextFreeTime = lastFreeTicketTime.AddHours(freeTicketIntervalHours);
        if (DateTime.UtcNow < nextFreeTime)
        {
            OnError?.Invoke("Abhi ticket available nahi!");
            return;
        }

        // Ticket add karo
        AddPinkTicket(1);

        // Timer reset
        lastFreeTicketTime = DateTime.UtcNow;
        PlayerPrefs.SetString("LastFreeTicketTime", lastFreeTicketTime.ToString());
        PlayerPrefs.Save();

        // Timer dobara shuru
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(FreeTicketTimerCoroutine());

        Debug.Log("Free ticket claimed!");
    }

    // =============================================
    // AD TICKET REWARD
    // =============================================
    public void OnAdWatched()
    {
        // Yeh rewarded ad complete hone pe call karo
        AddPinkTicket(1);
        Debug.Log("Ad ticket added!");
    }
}