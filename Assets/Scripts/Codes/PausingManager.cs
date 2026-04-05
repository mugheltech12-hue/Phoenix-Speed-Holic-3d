using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PausingManager : MonoBehaviour
{


    public static PausingManager Instance;
    private int state;

    public GameManager gameManager;
    public float previousZSpeed = 15f;

    public VideoPlayer gameOverAnimation;

    private int score = 0;

    [Header("Text Elements")]
    public Text scoreText;
    public Text startCountText;

    [Header("Panels")]
    public GameObject pausPanel;
    public GameObject failPanel;
    public GameObject mainUICanvas;

    [Header("Ui Buttons")]
    public GameObject playButton, watchAdButton;
    public Button leaderBoardButton;
    public GameObject soundOnButton, soundOffButton;
    public GameObject completePlayButton, completeSoundButton;
    public GameObject gameNameObject, ppButtonsParent, ppPlayButton, ppPauseButton, ppResumeButton;

    [Header("Music Components")]
    public AudioSource audioSource;
    public Button musicButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    [Header("Tournament Continue")]
    public GameObject continueTournamentBtn; // naya button assign karo


    private Coroutine countDownCoroutine;


    public bool isGameActive = false;

    //[Header("Tournament End Notification")]
    //public GameObject tournamentEndedNotification;
    //public TMP_Text tournamentEndedText; // ✅ Text component

    // Resume ke liye track karna ke game paused tha ya nahi
    private bool isResuming = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        state = 2;
        OnPlayMusic();
        PlayerPrefs.SetInt("FirstPlay", 1);

        ppPauseButton.SetActive(true);
        ppPauseButton.GetComponent<Button>().interactable = false;
        ppResumeButton.SetActive(false);



    }

    private void Update()
    {
        score = score + 1;
        scoreText.text = "" + (gameManager.score - 1);
    }

    #region Music

    public void OnPlayMusic()
    {
        if (PlayerPrefs.GetInt("MusicOn", 1) == 1)
        {
            audioSource.Play();
            musicButton.GetComponent<Image>().sprite = musicOffSprite;
        }
        else
        {
            audioSource.Stop();
            musicButton.GetComponent<Image>().sprite = musicOnSprite;
        }

        musicButton.onClick.RemoveAllListeners();
        musicButton.onClick.AddListener(ToggleMusic);
    }

    public void ToggleMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            musicButton.GetComponent<Image>().sprite = musicOnSprite;
            PlayerPrefs.SetInt("MusicOn", 0);
        }
        else
        {
            audioSource.Play();
            musicButton.GetComponent<Image>().sprite = musicOffSprite;
            PlayerPrefs.SetInt("MusicOn", 1);
        }

        PlayerPrefs.Save();
    }

    #endregion

    // =============================================
    // PAUSE BUTTON PRESS — Panel show karo
    // =============================================
    public void OnPauseButtonPressed()
    {
        TournamentPanelController.Instance.OpenTournament(); // ✅ Tournament panel kholo
        // Game ki speed rok do
        previousZSpeed = gameManager.speedZ;
        gameManager.speedZ = 0f;
        state = 1; // 1 = paused state (resume hoga)

        // Gameplay buttons hide karo
        ppPauseButton.SetActive(false);
        ppButtonsParent.SetActive(false);
        ppResumeButton.SetActive(true);
        // Pause Panel show karo
        //pausPanel.SetActive(true);

        // Countdown rok do agar chal raha ho
        if (countDownCoroutine != null)
        {
            StopCoroutine(countDownCoroutine);
            countDownCoroutine = null;
        }

        startCountText.gameObject.SetActive(false);
    }

    // =============================================
    // RESUME BUTTON PRESS — Ad → Counter → Game
    // =============================================
    public void OnResumeButtonPressed()
    {
        // Pause panel band karo
        //pausPanel.SetActive(false);


        ppPauseButton.SetActive(true);
        ppResumeButton.SetActive(false);

        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowInterstitial();
        }
        // Documentation ke mutabiq:
        // 1. Pehle Interstitial Ad dikhao
        // 2. Phir Counter (3,2,1)
        // 3. Game resume

        // Interstitial Ad call karo — apna AdsManager ka function yahan dalo
        // AdsManager.Instance.ShowInterstitialAd(OnInterstitialAdClosed);

        // Agar AdsManager nahi hai abhi, to seedha counter chala do:
        OnInterstitialAdClosed();
    }

    // Ye function tab call hoga jab Ad band ho jaye
    public void OnInterstitialAdClosed()
    {
        // Counter show karo middle mein

        TournamentPanelController.Instance.CloseTournament(); // ✅ Tournament panel band karo
        startCountText.gameObject.SetActive(true);

        if (countDownCoroutine != null)
            StopCoroutine(countDownCoroutine);

        countDownCoroutine = StartCoroutine(ResumeCountDown());
    }

    IEnumerator ResumeCountDown()
    {
        // Score hide karo, countdown show karo
        scoreText.gameObject.SetActive(false);
        startCountText.gameObject.SetActive(true);

        startCountText.text = "3";
        yield return new WaitForSeconds(1f);

        startCountText.text = "2";
        yield return new WaitForSeconds(1f);

        startCountText.text = "1";
        yield return new WaitForSeconds(1f);

        // Countdown band, score wapas show karo
        startCountText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);

        // Game resume karo wahin se jahan paused tha
        ActuallyResumeGame();
    }

    void ActuallyResumeGame()
    {
        state = 2;
        gameManager.speedZ = previousZSpeed;
        gameManager.SetContollerDegree();

        // Gameplay buttons wapas dikha do
        ppPauseButton.SetActive(true);
        ppButtonsParent.SetActive(true);
    }

    // =============================================
    // GAME OVER
    // =============================================
    public void PauseGameOver()
    {
        isGameActive = false; // ✅ add karo
        if (gameManager.score - 1 > PlayerPrefs.GetInt("HighScores"))
        {
            PlayerPrefs.SetInt("HighScores", gameManager.score);
        }

        previousZSpeed = gameManager.speedZ;
        gameManager.speedZ = 0f;
        //BlinkImage.Instance.ChangeColor();
        state = 0;

        ppPauseButton.SetActive(false);
        soundOnButton.SetActive(false);
        soundOffButton.SetActive(false);
        startCountText.gameObject.SetActive(false);
        playButton.SetActive(false);
        watchAdButton.SetActive(false);

        failPanel.SetActive(true);

        bool showContinue = TournamentManager.Instance != null
          //&& TournamentManager.Instance.isInTournament
          //&& !TournamentManager.Instance.hasUsedContinue
          && TicketManager.Instance.pinkTickets >= 1;

        continueTournamentBtn.SetActive(showContinue);

        if (TournamentCard.PendingLeaderboardOpen)
        {
            TournamentCard.PendingLeaderboardOpen = false;

            // ✅ Fail panel par message dikhao
            //if (tournamentEndedText != null)
            //    tournamentEndedText.text = "🏆 Tournament Results Tayar Hain!";

            //if (tournamentEndedNotification != null)
            //    tournamentEndedNotification.SetActive(true);

            // 1 second delay ke baad leaderboard kholo
            StartCoroutine(OpenLeaderboardAfterDelay(
                TournamentCard.PendingLeaderboardType_Static, 1f));
        }
    }


    IEnumerator OpenLeaderboardAfterDelay(TournamentType type, float delay)
    {
        yield return new WaitForSeconds(delay);

        //if (tournamentEndedNotification != null)
        //    tournamentEndedNotification.SetActive(false);

        LeaderboardUIManager.Instance.OpenPanelForType(type);
    }

    // ✅ PausingManager.cs — OnTournamentContinuePressed() aur ContinueCountDown() replace karo

    public void OnTournamentContinuePressed()
    {
        continueTournamentBtn.SetActive(false);
        failPanel.SetActive(false);

        // PT spend karo
        TournamentManager.Instance.ContinueTournament();

        // GameManager reset karo — controller is still DISABLED inside ResetForContinue
        gameManager.ResetForContinue();

        // Speed abhi bhi 0 rehne do
        // Controller bhi abhi disabled hai — countdown ke baad enable hoga

        state = 2;

        if (countDownCoroutine != null)
            StopCoroutine(countDownCoroutine);

        countDownCoroutine = StartCoroutine(ContinueCountDown());
    }

    IEnumerator ContinueCountDown()
    {
        // ✅ Controller disable rakho countdown ke dauran
        gameManager.controller.enabled = false;

        scoreText.gameObject.SetActive(false);
        startCountText.gameObject.SetActive(true);

        startCountText.text = "3";
        yield return new WaitForSeconds(1f);

        startCountText.text = "2";
        yield return new WaitForSeconds(1f);

        startCountText.text = "1";
        yield return new WaitForSeconds(1f);

        startCountText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);

        // ✅ Speed pehle restore karo
        gameManager.speedZ = previousZSpeed;
        gameManager.SetContollerDegree();

        // ✅ Tab controller enable karo
        gameManager.controller.enabled = true;

        ppPauseButton.SetActive(true);
        ppButtonsParent.SetActive(true);
    }


    //public void OnTournamentContinuePressed()
    //{
    //    continueTournamentBtn.SetActive(false);
    //    failPanel.SetActive(false);

    //    // PT spend karo
    //    TournamentManager.Instance.ContinueTournament();

    //    // GameManager reset karo (player center mein)
    //    gameManager.ResetForContinue();

    //    // Speed abhi mat do — countdown ke baad milegi
    //    state = 2;

    //    // 3-2-1 countdown phir resume
    //    if (countDownCoroutine != null)
    //        StopCoroutine(countDownCoroutine);

    //    countDownCoroutine = StartCoroutine(ContinueCountDown());
    //}

    //IEnumerator ContinueCountDown()
    //{
    //    // UI setup
    //    scoreText.gameObject.SetActive(false);
    //    startCountText.gameObject.SetActive(true);

    //    startCountText.text = "3";
    //    yield return new WaitForSeconds(1f);

    //    startCountText.text = "2";
    //    yield return new WaitForSeconds(1f);

    //    startCountText.text = "1";
    //    yield return new WaitForSeconds(1f);

    //    startCountText.gameObject.SetActive(false);
    //    scoreText.gameObject.SetActive(true);

    //    // Ab game actually resume karo
    //    gameManager.speedZ = previousZSpeed;
    //    gameManager.SetContollerDegree();

    //    ppPauseButton.SetActive(true);
    //    ppButtonsParent.SetActive(true);
    //}
    public void OnRetryButtonPressed()
    {
        // ✅ Tournament end karo before reload
        if (TournamentManager.Instance != null && TournamentManager.Instance.isInTournament)
            TournamentManager.Instance.EndTournament(true);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // =============================================
    // PLAY BUTTON (Home Screen se pehli baar)
    // =============================================
    //public IEnumerator OnPlayButtonPressed()
    //{

    //    scoreText.gameObject.SetActive(false);
    //    startCountText.gameObject.SetActive(true);

    //    startCountText.text = "3";
    //    yield return new WaitForSeconds(1f);

    //    startCountText.text = "2";
    //    yield return new WaitForSeconds(1f);

    //    startCountText.text = "1";
    //    yield return new WaitForSeconds(1f);

    //    // Countdown band, score wapas show karo
    //    startCountText.gameObject.SetActive(false);
    //    scoreText.gameObject.SetActive(true);

    //    soundOnButton.SetActive(false);
    //    startCountText.gameObject.SetActive(true);
    //    scoreText.gameObject.SetActive(true);
    //    leaderBoardButton.gameObject.SetActive(false);
    //    gameManager.speedZ = 0f;
    //    Time.timeScale = 1f;

    //    if (countDownCoroutine != null)
    //    {
    //        StopCoroutine(countDownCoroutine);
    //    }

    //    if (PlayerPrefs.GetInt("FirstPlay", 1) == 1)
    //    {
    //        PlayerPrefs.SetInt("FirstPlay", 0);
    //        PlayerPrefs.Save();
    //        startCountText.gameObject.SetActive(false);

    //        if (state == 2)
    //        {
    //            ppPauseButton.SetActive(true);
    //            gameNameObject.SetActive(false);
    //            ppButtonsParent.SetActive(true);
    //            playButton.SetActive(false);
    //            PlayGame();
    //        }
    //        completeSoundButton.SetActive(false);
    //    }
    //    else
    //    {
    //        countDownCoroutine = StartCoroutine(StartGameCountDown());
    //    }
    //}

    public void StartPlayCountdown()
    {
        if (countDownCoroutine != null)
            StopCoroutine(countDownCoroutine);

        countDownCoroutine = StartCoroutine(OnPlayButtonPressed());
    }

    public IEnumerator OnPlayButtonPressed()
    {
        isGameActive = true; // ✅ add karo
        Time.timeScale = 1;// UI Setup

        scoreText.gameObject.SetActive(false);
        startCountText.gameObject.SetActive(true);
        //soundOnButton.SetActive(false);
        leaderBoardButton.gameObject.SetActive(false);
        playButton.SetActive(false);
        gameNameObject.SetActive(false);
        ppPauseButton.SetActive(false);
        ppButtonsParent.SetActive(false);
        //completeSoundButton.SetActive(false);

        // Sirf ek baar countdown
        startCountText.text = "3";
        yield return new WaitForSeconds(1f);

        startCountText.text = "2";
        yield return new WaitForSeconds(1f);

        startCountText.text = "1";
        yield return new WaitForSeconds(1f);

        // Countdown khatam
        startCountText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);

        // Gameplay buttons show karo
        ppPauseButton.SetActive(true);
        ppButtonsParent.SetActive(true);

        // FirstPlay flag clear karo
        PlayerPrefs.SetInt("FirstPlay", 0);
        PlayerPrefs.Save();

        // ✅ Game start
        state = 2;
        Time.timeScale = 1f;
        PlayGame();
    }


    IEnumerator StartGameCountDown()
    {
        startCountText.text = "";
        yield return new WaitForEndOfFrame();

        startCountText.text = "3";
        yield return new WaitForSeconds(1f);

        startCountText.text = "2";
        yield return new WaitForSeconds(1f);

        startCountText.text = "1";
        yield return new WaitForSeconds(1f);

        startCountText.gameObject.SetActive(false);
        completeSoundButton.SetActive(false);

        if (state == 2)
        {
            ppPauseButton.SetActive(true);
            gameNameObject.SetActive(false);
            ppButtonsParent.SetActive(true);
            playButton.SetActive(false);
            PlayGame();
        }
    }

    public void PlayGame()
    {
        gameManager.speedZ = previousZSpeed;
        gameManager.SetContollerDegree();
    }

    public void OnCutSceneEnded(VideoPlayer vp)
    {
        PlayerPrefs.SetInt("VideoPlayed", 1);
        PlayerPrefs.Save();

        gameOverAnimation.gameObject.SetActive(false);
        mainUICanvas.SetActive(true);
    }

    // Ye purana ResumeGame scene reload karta tha — ab use mat karo
    // Sirf OnResumeButtonPressed() use karo
    public void ResumeGame()
    {
        pausPanel.SetActive(false);
        OnResumeButtonPressed();
    }

    IEnumerator StartCountDownAfterGameOver()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 3; i > 0; i--)
        {
            startCountText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        startCountText.gameObject.SetActive(false);
        OnVideoEnd(null);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        gameOverAnimation.gameObject.SetActive(false);
        state = 2;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnWatchAdButtonClick()
    {
        if (countDownCoroutine != null)
            StopCoroutine(countDownCoroutine);
    }

    public void PauseGame()
    {
        previousZSpeed = gameManager.speedZ;
        state = 2;
        //Time.timeScale = 1f;
        gameManager.speedZ = 0f;
        playButton.SetActive(true);
        ppPauseButton.SetActive(false);
        //soundOnButton.SetActive(true);
        completeSoundButton.SetActive(true);


    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("VideoPlayed", 0);
    }


    public void ResetUIForNewGame()
    {
        // Hide any active panels
        failPanel.SetActive(false);
        if (pausPanel != null) pausPanel.SetActive(false);

        // Reset game-active flag
        isGameActive = false;

        // Restore speed reference so PlayGame() works correctly
        //previousZSpeed = gameManager.initialSpeedZ; // see note below

        previousZSpeed = 15f;
    }
}