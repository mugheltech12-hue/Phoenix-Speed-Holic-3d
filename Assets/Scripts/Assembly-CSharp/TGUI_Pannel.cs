using System.Collections.Generic;
using UnityEngine;

public class TGUI_Pannel : MonoBehaviour
{
	[SerializeField]
	private GameObject[] btns;

	private List<TextMesh> textObjList;

	public bool isFirstPannel;

	private void Awake()
	{
		textObjList = new List<TextMesh>();
		foreach (Transform item in base.transform)
		{
			if (item.GetComponent<TextMesh>() != null)
			{
				textObjList.Add(item.GetComponent<TextMesh>());
			}
		}
	}

	private void Start()
	{
		if (isFirstPannel)
		{
			TGUI_InputController.Instance.currPannel = TGUI_Pannel_Changer.Pannels.PannelStarting;
			base.gameObject.SetActive(true);
			EnablePannel();
			for (int i = 0; i < textObjList.Count; i++)
			{
				textObjList[i].color = new Color(textObjList[i].color.r, textObjList[i].color.g, textObjList[i].color.b, 1f);
			}
		}
		else
		{
			base.gameObject.SetActive(false);
			for (int j = 0; j < textObjList.Count; j++)
			{
				textObjList[j].color = new Color(textObjList[j].color.r, textObjList[j].color.g, textObjList[j].color.b, 0f);
			}
		}
	}

	public List<TextMesh> GetTextObjList()
	{
		return textObjList;
	}

	private void EnablePannel()
	{
		TGUI_InputController.Instance.RegistBtns(btns);
	}

	private void DisablePannel()
	{
		TGUI_InputController.Instance.UnregistBtns(btns);
	}
}
