using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGUI_InputController : MonoBehaviour
{
	public static TGUI_InputController Instance;

	public TGUI_Pannel_Changer.Pannels currPannel;

	public GameObject btn_Popup;

	public bool isPopupState;

	public bool touchAble;

	private string Click_Method_Name = "StartClickAnimation";

	[SerializeField]
	private CSEffect cse;

	private List<GameObject> currBtnList;

	private Camera uicam;

	private void Awake()
	{
		Instance = this;
		currBtnList = new List<GameObject>();
		uicam = GetComponent<Camera>();
		isPopupState = false;
	}

	private void Start()
	{
		StartCoroutine(DelayTouchableTrue());
	}

	private void Update()
	{
		if (!touchAble)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnBackPressed();
		}
		else
		{
			if (!Input.GetButtonUp("Fire1"))
			{
				return;
			}
			Ray ray = uicam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity))
			{
				return;
			}
			string text = hitInfo.transform.name;
			if (isPopupState)
			{
				if (text == btn_Popup.name)
				{
					btn_Popup.SendMessage(Click_Method_Name);
				}
				return;
			}
			for (int i = 0; i < currBtnList.Count; i++)
			{
				if (text == currBtnList[i].name)
				{
					currBtnList[i].SendMessage(Click_Method_Name);
					break;
				}
			}
		}
	}

	private void OnBackPressed()
	{
		if (isPopupState)
		{
			btn_Popup.SendMessage(Click_Method_Name);
			return;
		}
		switch (currPannel)
		{
		case TGUI_Pannel_Changer.Pannels.PannelStarting:
			SoundManager.Instance.PauseMusic();
			//AdManager.Instance.HideBanner();
			cse.SetAni("EndingScene");
			touchAble = false;
			break;
		case TGUI_Pannel_Changer.Pannels.PannelCredit:
			TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.PannelCredit, TGUI_Pannel_Changer.Pannels.PannelStarting);
			break;
		case TGUI_Pannel_Changer.Pannels.PannelGameSettings:
			TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.PannelGameSettings, TGUI_Pannel_Changer.Pannels.PannelStarting);
			break;
		case TGUI_Pannel_Changer.Pannels.pannelSkinSettings:
			TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.pannelSkinSettings, TGUI_Pannel_Changer.Pannels.PannelGameSettings);
			break;
		case TGUI_Pannel_Changer.Pannels.PannelScore:
			TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.PannelScore, TGUI_Pannel_Changer.Pannels.PannelStarting);
			break;
		}
	}

	public void RegistBtns(GameObject[] _btns)
	{
		for (int i = 0; i < _btns.Length; i++)
		{
			currBtnList.Add(_btns[i]);
		}
	}

	public void UnregistBtns(GameObject[] _btns)
	{
		for (int i = 0; i < _btns.Length; i++)
		{
			currBtnList.Remove(_btns[i]);
		}
	}

	public void RegistBtn(GameObject _btn)
	{
		currBtnList.Add(_btn);
	}

	public void UnregistBtn(GameObject _btn)
	{
		currBtnList.Remove(_btn);
	}

	public void SetPopupState(bool _isPopupState)
	{
		isPopupState = _isPopupState;
	}

	private IEnumerator DelayTouchableTrue()
	{
		yield return new WaitForSeconds(1f);
		touchAble = true;
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
