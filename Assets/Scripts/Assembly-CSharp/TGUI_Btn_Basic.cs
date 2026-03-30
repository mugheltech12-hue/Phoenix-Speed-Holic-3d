using System.Collections;
using UnityEngine;

public class TGUI_Btn_Basic : MonoBehaviour
{
	private TextMesh txt;

	protected virtual void Awake()
	{
		txt = base.gameObject.GetComponent<TextMesh>();
	}

	protected virtual void OnClicked()
	{
		TGUI_InputController.Instance.touchAble = true;
	}

	private void StartClickAnimation()
	{
		StartCoroutine(ClickAnimation());
		TGUI_InputController.Instance.touchAble = false;
	}

	private IEnumerator ClickAnimation()
	{
		txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0.5f);
		yield return new WaitForSeconds(0.1f);
		txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1f);
		OnClicked();
	}
}
