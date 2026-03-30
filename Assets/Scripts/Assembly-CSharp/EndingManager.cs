using UnityEngine;

public class EndingManager : MonoBehaviour
{
	private void Start()
	{
		Application.Quit();
	}

	private void OnApplicationQuit()
	{
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				Object.Destroy(array[i]);
				array[i] = null;
			}
		}
		array = null;
	}
}
