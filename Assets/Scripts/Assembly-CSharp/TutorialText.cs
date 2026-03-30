using System.Collections;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
	public TextMesh details;

	private Transform mTransform;

	private int tutorial;

	private int state;

	private void Awake()
	{
		switch (PlayerPrefs.GetInt("MODE"))
		{
		case 1:
			tutorial = PlayerPrefs.GetInt("TUTORIAL_T");
			break;
		case 2:
			tutorial = PlayerPrefs.GetInt("TUTORIAL_G");
			break;
		}
		if (tutorial == 1)
		{
			switch (PlayerPrefs.GetInt("MODE"))
			{
			case 1:
				PlayerPrefs.SetInt("TUTORIAL_T", 0);
				break;
			case 2:
				PlayerPrefs.SetInt("TUTORIAL_G", 0);
				break;
			}
			details.color = new Color(details.color.r, details.color.g, details.color.b, 0f);
			state = -1;
			StartCoroutine(ChangeState(0, 1f));
			mTransform = base.transform;
		}
		else
		{
			base.gameObject.SetActive(false);
			base.transform.parent = null;
		}
	}

	private void Update()
	{
		CheckState();
	}

	private void CheckState()
	{
		switch (state)
		{
		case 0:
			if (details.color.a < 1f)
			{
				details.color = new Color(details.color.r, details.color.g, details.color.b, details.color.a + 1f * Time.deltaTime);
				if (details.color.a >= 1f)
				{
					details.color = new Color(details.color.r, details.color.g, details.color.b, 1f);
					state = 1;
				}
			}
			break;
		case 1:
			StartCoroutine(ChangeState(2, 3f));
			break;
		case 2:
			if (details.color.a > 0f)
			{
				details.color = new Color(details.color.r, details.color.g, details.color.b, details.color.a - 1f * Time.deltaTime);
				if (details.color.a <= 0f)
				{
					details.color = new Color(details.color.r, details.color.g, details.color.b, 0f);
					mTransform.parent = null;
					base.gameObject.SetActive(false);
				}
			}
			break;
		}
	}

	private IEnumerator ChangeState(int _state, float _delay)
	{
		state = 100;
		yield return new WaitForSeconds(_delay);
		state = _state;
	}
}
