using UnityEngine;

public class MainController_ModeG : MonoBehaviour
{
	public Transform mTransform;

	public Transform chdTransform;

	public Transform ps2Transform;

	public float degree;

	private bool isClockWise;

	private bool isRotate;

	private void Awake()
	{
		mTransform = base.transform;
	}

	private void Update()
	{
		CheckAccelometer();
		CheckRotate();
	}

	private void CheckAccelometer()
	{
		if ((double)Input.acceleration.x <= -0.04)
		{
			isClockWise = true;
			isRotate = true;
		}
		else if ((double)Input.acceleration.x >= 0.04)
		{
			isClockWise = false;
			isRotate = true;
		}
		else
		{
			isRotate = false;
		}
	}

	private void CheckRotate()
	{
		if (isRotate)
		{
			if (isClockWise)
			{
				mTransform.Rotate(new Vector3(0f, 0f, (0f - degree) * Time.deltaTime));
				ps2Transform.Rotate(new Vector3(0f, 0f, (0f - degree) * Time.deltaTime));
				chdTransform.Rotate(new Vector3(0f, 0f, 10f * degree * Time.deltaTime));
			}
			else
			{
				mTransform.Rotate(new Vector3(0f, 0f, degree * Time.deltaTime));
				ps2Transform.Rotate(new Vector3(0f, 0f, degree * Time.deltaTime));
				chdTransform.Rotate(new Vector3(0f, 0f, -10f * degree * Time.deltaTime));
			}
		}
		else
		{
			chdTransform.Rotate(new Vector3(0f, 0f, -10f * degree * Time.deltaTime));
		}
	}
}
