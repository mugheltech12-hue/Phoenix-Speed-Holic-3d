using System.Collections.Generic;
using UnityEngine;

public class TGUI_Pannel_Changer : MonoBehaviour
{
	public enum Pannels
	{
		PannelStarting = 0,
		PannelGameSettings = 1,
		pannelSkinSettings = 2,
		PannelCredit = 3,
		PannelScore = 4
	}

	private List<TextMesh> textObjList_Curr;

	private List<TextMesh> textObjList_Target;

	public static TGUI_Pannel_Changer Instance;

	[SerializeField]
	private Transform PannelStarting;

	[SerializeField]
	private Transform PannelGameSettings;

	[SerializeField]
	private Transform PannelSkinSettings;

	[SerializeField]
	private Transform PannelCredit;

	[SerializeField]
	private Transform PannelScore;

	private bool isAnimation;

	private Transform disAppear;

	private Transform appear;

	private const float startSpeed = 0.1f;

	private const float accRate = 2.2f;

	private const float limitSpeed = 130f;

	private float pointX_Appear;

	private float pointX_Disappear = 10f;

	private float speed;

	private void Awake()
	{
		Instance = this;
		float num = 1.7777778f;
		float num2 = (float)Screen.width / (float)Screen.height;
		pointX_Appear *= num2 / num;
		pointX_Disappear *= num2 / num;
		speed = 0.1f;
	}

	public void RequestPannelChange(Pannels _current, Pannels _target)
	{
		TGUI_InputController.Instance.currPannel = _target;
		switch (_current)
		{
		case Pannels.PannelStarting:
			disAppear = PannelStarting;
			break;
		case Pannels.PannelGameSettings:
			disAppear = PannelGameSettings;
			break;
		case Pannels.pannelSkinSettings:
			disAppear = PannelSkinSettings;
			break;
		case Pannels.PannelCredit:
			disAppear = PannelCredit;
			break;
		case Pannels.PannelScore:
			disAppear = PannelScore;
			break;
		}
		switch (_target)
		{
		case Pannels.PannelStarting:
			appear = PannelStarting;
			break;
		case Pannels.PannelGameSettings:
			appear = PannelGameSettings;
			break;
		case Pannels.pannelSkinSettings:
			appear = PannelSkinSettings;
			break;
		case Pannels.PannelCredit:
			appear = PannelCredit;
			break;
		case Pannels.PannelScore:
			appear = PannelScore;
			break;
		}
		textObjList_Curr = disAppear.GetComponent<TGUI_Pannel>().GetTextObjList();
		textObjList_Target = appear.GetComponent<TGUI_Pannel>().GetTextObjList();
		isAnimation = true;
		TGUI_InputController.Instance.touchAble = false;
		appear.gameObject.SetActive(true);
	}

	private void Update()
	{
		if (!isAnimation || !(disAppear.position.x < pointX_Disappear))
		{
			return;
		}
		disAppear.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
		appear.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
		speed *= 2.2f;
		if (speed > 130f)
		{
			speed = 130f;
		}
		if (disAppear.position.x < pointX_Disappear / 2f)
		{
			for (int i = 0; i < textObjList_Curr.Count; i++)
			{
				if (textObjList_Curr[i].color.r > 0f)
				{
					textObjList_Curr[i].color = new Color(textObjList_Curr[i].color.r, textObjList_Curr[i].color.g, textObjList_Curr[i].color.b, textObjList_Curr[i].color.a - 5f * Time.deltaTime);
				}
			}
		}
		if (appear.position.x < pointX_Disappear / 2f)
		{
			for (int j = 0; j < textObjList_Target.Count; j++)
			{
				if (textObjList_Target[j].color.r < 1f)
				{
					textObjList_Target[j].color = new Color(textObjList_Target[j].color.r, textObjList_Target[j].color.g, textObjList_Target[j].color.b, textObjList_Target[j].color.a + 5f * Time.deltaTime);
				}
			}
		}
		if (disAppear.position.x > pointX_Disappear)
		{
			speed = 0.1f;
			for (int k = 0; k < textObjList_Curr.Count; k++)
			{
				textObjList_Curr[k].color = new Color(textObjList_Curr[k].color.r, textObjList_Curr[k].color.g, textObjList_Curr[k].color.b, 0f);
			}
			for (int l = 0; l < textObjList_Target.Count; l++)
			{
				textObjList_Target[l].color = new Color(textObjList_Target[l].color.r, textObjList_Target[l].color.g, textObjList_Target[l].color.b, 1f);
			}
			disAppear.position = new Vector3(pointX_Disappear, disAppear.position.y, disAppear.position.z);
			appear.position = new Vector3(pointX_Appear, appear.position.y, appear.position.z);
			disAppear.SendMessage("DisablePannel");
			appear.SendMessage("EnablePannel");
			disAppear.gameObject.SetActive(false);
			TGUI_InputController.Instance.touchAble = true;
			isAnimation = false;
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
