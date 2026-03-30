using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameAnimation : MonoBehaviour
{
    public VideoPlayer gameOverAnimation;

    // Start is called before the first frame update
    void Start()
    {

        gameOverAnimation.Play();
        gameOverAnimation.loopPointReached += OnCutSceneEnded;
    }

    public void OnCutSceneEnded(VideoPlayer vp)
    {
        PlayerPrefs.SetInt("VideoPlayed", 1);
        PlayerPrefs.Save();

        //gameOverAnimation.gameObject.SetActive(false);
        SceneManager.LoadScene("GameScene_ModeT");
    }
}
