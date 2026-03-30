using UnityEngine;

public class IntroBall : MonoBehaviour
{
	private const float cDegree = 600f;

	private const float mDegree = 50f;

	private Transform mTransform;

	private Transform cTransform;

	private int degreeState;

	private int currDegree;

	private int destDegree;

	private void Awake()
	{
		mTransform = base.transform;
		cTransform = mTransform.GetChild(0);
		currDegree = 10;
	}

	private void Update()
	{
		switch (degreeState)
		{
		case 1:
			mTransform.Rotate(new Vector3(0f, 0f, 50f * Time.deltaTime));
			cTransform.Rotate(new Vector3(0f, 0f, -600f * Time.deltaTime));
			break;
		case 2:
			mTransform.Rotate(new Vector3(0f, 0f, -50f * Time.deltaTime));
			cTransform.Rotate(new Vector3(0f, 0f, 600f * Time.deltaTime));
			break;
		}
	}

	public void OnTilePosChanged(int _destDegree)
	{
		if (currDegree < _destDegree)
		{
			degreeState = 1;
		}
		else if (currDegree > _destDegree)
		{
			degreeState = 2;
		}
		else
		{
			degreeState = 0;
		}
		mTransform.rotation = Quaternion.identity;
		mTransform.Rotate(new Vector3(0f, 0f, 10 * currDegree));
		currDegree = _destDegree;
	}
}
