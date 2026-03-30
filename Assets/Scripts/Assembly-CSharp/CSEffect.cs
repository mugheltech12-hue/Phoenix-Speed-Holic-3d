using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSEffect : MonoBehaviour
{
	private Camera fadeCam;

	private Material mat;

	private bool disAppear;

	private bool isAni;

	private string SceneName;

	private void Awake()
	{
		fadeCam = GameObject.Find("Fade Camera").GetComponent<Camera>();
		mat = GetComponent<MeshRenderer>().material;
		mat.color = new Color(0f, 0f, 0f, 1f);
		disAppear = true;
		isAni = true;
	}

	private void Update()
	{
		if (!isAni)
		{
			return;
		}
		if (!disAppear)
		{
			if (mat.color.a < 1f)
			{
				mat.color = new Color(0f, 0f, 0f, mat.color.a + 1.5f * Time.deltaTime);
				if (mat.color.a > 1f)
				{
					mat.color = new Color(0f, 0f, 0f, 1f);
					isAni = false;
					SceneManager.LoadScene(SceneName);
				}
			}
		}
		else
		{
			if (!(mat.color.a > 0f))
			{
				return;
			}
			mat.color = new Color(0f, 0f, 0f, mat.color.a - 1.5f * Time.deltaTime);
			if (mat.color.a < 0f)
			{
				if (SceneManager.GetActiveScene().name == "IntroScene")
				{
					//AdManager.Instance.ShowBanner();
				}
				mat.color = new Color(0f, 0f, 0f, 0f);
				fadeCam.gameObject.SetActive(false);
				base.enabled = false;
				isAni = false;
			}
		}
	}

	private IEnumerator ChangeScene(float _delay)
	{
		yield return new WaitForSeconds(_delay);
		SceneManager.LoadScene(SceneName);
	}

	public void SetAni(string _SceneName)
	{
		if (_SceneName == "GameScene_ModeG" || _SceneName == "GameScene_ModeT")
		{
			//AdManager.Instance.HideBanner();
		}
		fadeCam.gameObject.SetActive(true);
		base.enabled = true;
		SceneName = _SceneName;
		disAppear = false;
		isAni = true;
	}
}
