using UnityEngine;

public class PauseManager : MonoBehaviour
{
	public static PauseManager Instance;

	public bool isGameOver;

	private float currTimeScale;

	private int state;

	[SerializeField]
	private Camera uicam;

	[SerializeField]
	private GameObject fadeGroup;

	[SerializeField]
	private GameObject GameManager;

	[SerializeField]
	private GameObject pause_bg;

	[SerializeField]
	private GameObject pause_txt;

	[SerializeField]
	private GameObject btn_pause_resume;

	[SerializeField]
	private GameObject btn_resume_text;

	[SerializeField]
	private GameObject btn_exit;

	[SerializeField]
	private Material btn_pause;

	[SerializeField]
	private Material btn_resume;

	private Renderer btn_renderer;

	private bool isTouchAble;

	private bool isResumeAni;

	private void Awake()
	{
		Instance = this;
		btn_renderer = btn_pause_resume.GetComponent<Renderer>();
		currTimeScale = Time.timeScale;
		SetResume();
		Time.timeScale = currTimeScale;
		isTouchAble = true;
		switch (DataManager.Instance.GetIntDB(DataManager.DBNameTag.SPEED))
		{
		case 0:
			currTimeScale = 0.7f;
			break;
		case 1:
			currTimeScale = 1f;
			break;
		case 2:
			currTimeScale = 1.3f;
			break;
		}
	}

	public void OnPauseStateChanged()
	{
		if (state == 1)
		{
			SetPause();
		}
		else if (state == 0)
		{
			SetResume();
		}
	}

	private void Update()
	{
		if (isTouchAble)
		{
			CheckInput();
		}
		if (isResumeAni && Time.timeScale < currTimeScale)
		{
			Time.timeScale *= 1.1f;
			if (Time.timeScale > currTimeScale)
			{
				Time.timeScale = currTimeScale;
				isResumeAni = false;
				isTouchAble = true;
			}
		}
	}

	private void CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnPauseStateChanged();
		}
		else
		{
			if (!Input.GetButtonDown("Fire1"))
			{
				return;
			}
			Ray ray = uicam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity))
			{
				string text = hitInfo.transform.name;
				if (text == btn_pause_resume.name)
				{
					OnPauseStateChanged();
				}
				else if (text == btn_exit.name)
				{
					ExitGameState();
				}
				else if (text == btn_resume_text.name)
				{
					SetResume();
				}
			}
		}
	}

	private void SetPause()
	{
		if (!isGameOver)
		{
			state = 0;
			Time.timeScale = 0f;
			isTouchAble = true;
			isResumeAni = false;
			fadeGroup.SetActive(false);
			pause_bg.SetActive(true);
			pause_txt.SetActive(true);
			btn_pause_resume.SetActive(true);
			btn_resume_text.SetActive(true);
			btn_exit.SetActive(true);
			btn_renderer.sharedMaterial = btn_resume;
		}
	}

	private void SetResume()
	{
		state = 1;
		Time.timeScale = 0.005f;
		isTouchAble = false;
		isResumeAni = true;
		pause_bg.SetActive(false);
		pause_txt.SetActive(false);
		btn_resume_text.SetActive(false);
		btn_exit.SetActive(false);
		btn_renderer.sharedMaterial = btn_pause;
	}

	private void ExitGameState()
	{
		SetResume();
		isResumeAni = false;
		Time.timeScale = currTimeScale;
		GameManager.SendMessage("SetOver");
	}

	private void OnApplicationPause()
	{
		SetPause();
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
