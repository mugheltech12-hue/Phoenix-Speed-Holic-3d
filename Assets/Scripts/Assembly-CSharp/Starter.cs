using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Starter : MonoBehaviour
{
	private void Awake()
	{
		Screen.sleepTimeout = -1;
		Application.targetFrameRate = 60;
	}

	private void Start()
	{
		StartCoroutine(DelayStart());
	}

	private IEnumerator DelayStart()
	{
		yield return new WaitForSeconds(1.2f);
		SceneManager.LoadScene("IntroScene");
	}
}
