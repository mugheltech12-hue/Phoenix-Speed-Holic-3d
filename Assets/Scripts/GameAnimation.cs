using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameAnimation : MonoBehaviour
{
    public VideoPlayer gameOverAnimation;

    private void OnEnable()
    {
        // Subscribe — video will play once login completes
        PlayFabLoginManager.OnLoginComplete += StartVideo;
    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks
        PlayFabLoginManager.OnLoginComplete -= StartVideo;
    }

    private void StartVideo()
    {
        gameOverAnimation.Play();
        gameOverAnimation.loopPointReached += OnCutSceneEnded;
    }

    public void OnCutSceneEnded(VideoPlayer vp)
    {
        PlayerPrefs.SetInt("VideoPlayed", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene_ModeT");
    }
}