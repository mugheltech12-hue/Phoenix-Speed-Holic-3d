using UnityEngine;

public class TGUI_Message : MonoBehaviour
{
	public static TGUI_Message Instance;

	private Transform[] cTransform;

	private Transform mTransform;

	private TextMesh PopUp_text;

	private void OnDestroy()
	{
		Instance = null;
	}

	private void Awake()
	{
		Instance = this;
		mTransform = base.transform;
		cTransform = new Transform[mTransform.childCount];
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i] = mTransform.GetChild(i);
			if (cTransform[i].name == "PopUp_Text")
			{
				PopUp_text = cTransform[i].GetComponent<TextMesh>();
			}
		}
	}

	public void StartPopUp(string _text)
	{
		TGUI_InputController.Instance.SetPopupState(true);
		PopUp_text.text = _text;
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i].gameObject.SetActive(true);
		}
	}

	public void EndPopUp()
	{
		TGUI_InputController.Instance.SetPopupState(false);
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i].gameObject.SetActive(false);
		}
	}
}
