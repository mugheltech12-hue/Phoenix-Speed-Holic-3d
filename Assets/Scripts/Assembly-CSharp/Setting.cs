using System;
using UnityEngine;

public class Setting : MonoBehaviour
{
	public Transform mTransform;

	public Material mat1;

	public Material mat2;

	public Material mat3;

	public Mesh quad;

	public float startDegree;

	public float parentDegree;

	public float degree;

	public int cutCount;

	public float offsetZ;

	public float scaleX;

	public float scaleY;

	public float scaleZ;

	public float roadPosRate;

	public float roadScaleX;

	public float roadScaleY;

	public float roadScaleZ;

	public bool rotX;

	public float startX;

	public float startY;

	public float circleRad;

	private Transform[] cTransform;

	private GameManager GameManager;

	private float x;

	private float y;

	public void AddScriptIndexOfDegree()
	{
		cTransform = new Transform[mTransform.childCount];
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i] = mTransform.GetChild(i);
			if (!cTransform[i].GetComponent<RingSetting>())
			{
				cTransform[i].gameObject.AddComponent<RingSetting>();
			}
		}
	}

	public void SetIntroPosition()
	{
		cTransform = new Transform[mTransform.childCount];
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i] = mTransform.GetChild(i);
			cTransform[i].localRotation = Quaternion.identity;
			if (i < 15)
			{
				cTransform[i].Rotate(0f, 0f, 70 + i * 10);
			}
			else
			{
				cTransform[i].Rotate(0f, 0f, 210 - (i - 15) * 10);
			}
		}
	}

	public void AddBoxCollider()
	{
		RemoveCollider();
		cTransform = new Transform[mTransform.childCount];
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i] = mTransform.GetChild(i);
			for (int j = 0; j < cTransform[i].childCount; j++)
			{
				if (j == cutCount)
				{
					continue;
				}
				if (j == 0 || j == cutCount - 2)
				{
					cTransform[i].GetChild(j).gameObject.AddComponent<BoxCollider>();
					cTransform[i].GetChild(j).gameObject.GetComponent<BoxCollider>().isTrigger = true;
					if (j == 0)
					{
						cTransform[i].GetChild(j).gameObject.GetComponent<BoxCollider>().center = new Vector3(2f, 0f, 0f);
						cTransform[i].GetChild(j).gameObject.GetComponent<BoxCollider>().size = new Vector3(4f, 4f, 2.5f);
					}
					else
					{
						cTransform[i].GetChild(j).gameObject.GetComponent<BoxCollider>().center = new Vector3(-2f, 0f, 0f);
						cTransform[i].GetChild(j).gameObject.GetComponent<BoxCollider>().size = new Vector3(4f, 4f, 2.5f);
					}
					cTransform[i].GetChild(j).gameObject.layer = 9;
				}
				else
				{
					cTransform[i].GetChild(j).gameObject.layer = 0;
				}
			}
		}
	}

	public void AddCapsuleCollider()
	{
		RemoveCollider();
		cTransform = new Transform[mTransform.childCount];
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i] = mTransform.GetChild(i);
			for (int j = 0; j < cTransform[i].childCount; j++)
			{
				if (j != cutCount)
				{
					if (j == 0 || j == cutCount - 2)
					{
						cTransform[i].GetChild(j).gameObject.AddComponent<CapsuleCollider>();
						cTransform[i].GetChild(j).gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
						cTransform[i].GetChild(j).gameObject.GetComponent<CapsuleCollider>().radius = circleRad;
						cTransform[i].GetChild(j).gameObject.GetComponent<CapsuleCollider>().direction = 2;
						cTransform[i].GetChild(j).gameObject.GetComponent<CapsuleCollider>().height = 1.5f;
						cTransform[i].GetChild(j).gameObject.layer = 9;
					}
					else
					{
						cTransform[i].GetChild(j).gameObject.layer = 0;
					}
				}
			}
		}
	}

	public void AddSphereCollider()
	{
		RemoveCollider();
		cTransform = new Transform[mTransform.childCount];
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i] = mTransform.GetChild(i);
			for (int j = 0; j < cTransform[i].childCount; j++)
			{
				if (j != cutCount && (j == 0 || j == cutCount - 2))
				{
					cTransform[i].GetChild(j).gameObject.AddComponent<SphereCollider>();
					cTransform[i].GetChild(j).gameObject.GetComponent<SphereCollider>().isTrigger = true;
					if (j == 0)
					{
						cTransform[i].GetChild(j).gameObject.GetComponent<SphereCollider>().center = new Vector3(1.3f, 0f, 0f);
					}
					else
					{
						cTransform[i].GetChild(j).gameObject.GetComponent<SphereCollider>().center = new Vector3(-1.3f, 0f, 0f);
					}
					cTransform[i].GetChild(j).gameObject.GetComponent<SphereCollider>().radius = 1.3f;
					cTransform[i].GetChild(j).gameObject.layer = 9;
				}
			}
		}
	}

	public void RemoveCollider()
	{
		cTransform = new Transform[mTransform.childCount];
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i] = mTransform.GetChild(i);
			for (int j = 0; j < cTransform[i].childCount; j++)
			{
				if (cTransform[i].GetChild(j).gameObject.GetComponent<Collider>() != null)
				{
					UnityEngine.Object.DestroyImmediate(cTransform[i].GetChild(j).gameObject.GetComponent<Collider>());
				}
			}
		}
	}

	public void Set(int type)
	{
		GameManager = GameObject.Find("Zero").GetComponent<GameManager>();
		GameManager.degree = parentDegree;
		GameManager.offsetZ = offsetZ;
		switch (type)
		{
		case 1:
		{
			cTransform = new Transform[mTransform.childCount];
			for (int k = 0; k < mTransform.childCount; k++)
			{
				cTransform[k] = mTransform.GetChild(k);
				cTransform[k].localPosition = new Vector3(0f, 0f, (float)k * offsetZ);
				cTransform[k].localScale = new Vector3(1f, 1f, 1f);
				cTransform[k].rotation = Quaternion.identity;
				cTransform[k].Rotate(new Vector3(0f, 0f, (float)k * parentDegree));
				for (int l = 0; l < cTransform[k].childCount; l++)
				{
					cTransform[k].GetChild(l).GetComponent<MeshFilter>().sharedMesh = quad;
					if (l >= cutCount)
					{
						cTransform[k].GetChild(l).gameObject.SetActive(false);
						continue;
					}
					cTransform[k].GetChild(l).gameObject.SetActive(true);
					if (l < 4)
					{
						cTransform[k].GetChild(l).GetComponent<Renderer>().sharedMaterial = mat1;
					}
					else
					{
						cTransform[k].GetChild(l).GetComponent<Renderer>().sharedMaterial = mat2;
					}
					cTransform[k].GetChild(l).localRotation = Quaternion.identity;
					cTransform[k].GetChild(l).Rotate(new Vector3(0f, 0f, degree * (float)l + startDegree));
					cTransform[k].GetChild(l).localPosition = new Vector3(0f, 0f, 0f);
					cTransform[k].GetChild(l).localScale = new Vector3(scaleX, scaleY, scaleZ);
					x = startX * Mathf.Cos(degree * (float)l * ((float)Math.PI / 180f)) + (0f - startY) * Mathf.Sin(degree * (float)l * ((float)Math.PI / 180f));
					y = startX * Mathf.Sin(degree * (float)l * ((float)Math.PI / 180f)) + startY * Mathf.Cos(degree * (float)l * ((float)Math.PI / 180f));
					cTransform[k].GetChild(l).localPosition = new Vector3(x, y, cTransform[k].GetChild(l).localPosition.z);
				}
			}
			break;
		}
		case 2:
		{
			GameManager = GameObject.Find("Zero").GetComponent<GameManager>();
			cTransform = new Transform[mTransform.childCount];
			for (int num = 0; num < mTransform.childCount; num++)
			{
				cTransform[num] = mTransform.GetChild(num);
				cTransform[num].localPosition = new Vector3(0f, 0f, (float)num * offsetZ);
				cTransform[num].localScale = new Vector3(1f, 1f, 1f);
				cTransform[num].rotation = Quaternion.identity;
				cTransform[num].Rotate(new Vector3(0f, 0f, (float)num * parentDegree));
				for (int num2 = 0; num2 < cTransform[num].childCount; num2++)
				{
					cTransform[num].GetChild(num2).GetComponent<MeshFilter>().sharedMesh = quad;
					if (num2 >= cutCount)
					{
						cTransform[num].GetChild(num2).gameObject.SetActive(false);
						continue;
					}
					cTransform[num].GetChild(num2).gameObject.SetActive(true);
					if (num2 < 4)
					{
						cTransform[num].GetChild(num2).GetComponent<Renderer>().sharedMaterial = mat1;
					}
					else
					{
						cTransform[num].GetChild(num2).GetComponent<Renderer>().sharedMaterial = mat2;
					}
					cTransform[num].GetChild(num2).localRotation = Quaternion.identity;
					cTransform[num].GetChild(num2).Rotate(new Vector3(0f, 0f, degree * (float)num2 + startDegree));
					cTransform[num].GetChild(num2).Rotate(new Vector3(90f, 0f, 0f));
					cTransform[num].GetChild(num2).localPosition = new Vector3(0f, 0f, 0f);
					cTransform[num].GetChild(num2).localScale = new Vector3(scaleX, scaleY, scaleZ);
					x = startX * Mathf.Cos(degree * (float)num2 * ((float)Math.PI / 180f)) + (0f - startY) * Mathf.Sin(degree * (float)num2 * ((float)Math.PI / 180f));
					y = startX * Mathf.Sin(degree * (float)num2 * ((float)Math.PI / 180f)) + startY * Mathf.Cos(degree * (float)num2 * ((float)Math.PI / 180f));
					cTransform[num].GetChild(num2).localPosition = new Vector3(x, y, cTransform[num].GetChild(num2).localPosition.z);
				}
			}
			break;
		}
		case 3:
		{
			GameManager = GameObject.Find("Zero").GetComponent<GameManager>();
			cTransform = new Transform[mTransform.childCount];
			for (int num3 = 0; num3 < mTransform.childCount; num3++)
			{
				cTransform[num3] = mTransform.GetChild(num3);
				cTransform[num3].localPosition = new Vector3(0f, 0f, (float)num3 * offsetZ);
				cTransform[num3].localScale = new Vector3(1f, 1f, 1f);
				cTransform[num3].rotation = Quaternion.identity;
				cTransform[num3].Rotate(new Vector3(0f, 0f, 130f));
				for (int num4 = 0; num4 < cTransform[num3].childCount; num4++)
				{
					cTransform[num3].GetChild(num4).GetComponent<MeshFilter>().sharedMesh = quad;
					if (num4 >= cutCount)
					{
						if (num4 == cutCount)
						{
							cTransform[num3].GetChild(num4).localRotation = Quaternion.identity;
							cTransform[num3].GetChild(num4).Rotate(new Vector3(0f, 0f, degree * (float)num4 + degree / 2f + startDegree));
							if (rotX)
							{
								cTransform[num3].GetChild(num4).Rotate(new Vector3(90f, 0f, 0f));
							}
							x = startX * roadPosRate * Mathf.Cos((degree * (float)num4 + degree / 2f) * ((float)Math.PI / 180f)) + (0f - startY * roadPosRate) * Mathf.Sin((degree * (float)num4 + degree / 2f) * ((float)Math.PI / 180f));
							y = startX * roadPosRate * Mathf.Sin((degree * (float)num4 + degree / 2f) * ((float)Math.PI / 180f)) + startY * roadPosRate * Mathf.Cos((degree * (float)num4 + degree / 2f) * ((float)Math.PI / 180f));
							cTransform[num3].GetChild(num4).localPosition = new Vector3(x, y, cTransform[num3].GetChild(num4).localPosition.z);
							cTransform[num3].GetChild(num4).localScale = new Vector3(roadScaleX, roadScaleY, roadScaleZ);
							cTransform[num3].GetChild(num4).GetComponent<Renderer>().sharedMaterial = mat3;
						}
					}
					else
					{
						cTransform[num3].GetChild(num4).gameObject.SetActive(true);
						cTransform[num3].GetChild(num4).localRotation = Quaternion.identity;
						cTransform[num3].GetChild(num4).Rotate(new Vector3(0f, 0f, degree * (float)num4 + startDegree));
						cTransform[num3].GetChild(num4).localPosition = new Vector3(0f, 0f, 0f);
						cTransform[num3].GetChild(num4).localScale = new Vector3(scaleX, scaleY, scaleZ);
						x = startX * Mathf.Cos(degree * (float)num4 * ((float)Math.PI / 180f)) + (0f - startY) * Mathf.Sin(degree * (float)num4 * ((float)Math.PI / 180f));
						y = startX * Mathf.Sin(degree * (float)num4 * ((float)Math.PI / 180f)) + startY * Mathf.Cos(degree * (float)num4 * ((float)Math.PI / 180f));
						cTransform[num3].GetChild(num4).localPosition = new Vector3(x, y, cTransform[num3].GetChild(num4).localPosition.z);
						if (num4 == 0 || num4 == cutCount - 1)
						{
							cTransform[num3].GetChild(num4).GetComponent<Renderer>().sharedMaterial = mat1;
						}
						else
						{
							cTransform[num3].GetChild(num4).GetComponent<Renderer>().sharedMaterial = mat2;
						}
					}
				}
			}
			break;
		}
		case 4:
		{
			GameManager = GameObject.Find("Zero").GetComponent<GameManager>();
			cTransform = new Transform[mTransform.childCount];
			for (int m = 0; m < mTransform.childCount; m++)
			{
				cTransform[m] = mTransform.GetChild(m);
				cTransform[m].localPosition = new Vector3(0f, 0f, (float)m * offsetZ);
				cTransform[m].localScale = new Vector3(1f, 1f, 1f);
				cTransform[m].rotation = Quaternion.identity;
				cTransform[m].Rotate(new Vector3(0f, 0f, 130f));
				for (int n = 0; n < cTransform[m].childCount; n++)
				{
					cTransform[m].GetChild(n).GetComponent<MeshFilter>().sharedMesh = quad;
					if (n >= cutCount)
					{
						if (n == cutCount)
						{
							cTransform[m].GetChild(n).localRotation = Quaternion.identity;
							cTransform[m].GetChild(n).Rotate(new Vector3(0f, 0f, degree * (float)n + degree / 2f + startDegree));
							x = startX * roadPosRate * Mathf.Cos((degree * (float)n + degree / 2f) * ((float)Math.PI / 180f)) + (0f - startY * roadPosRate) * Mathf.Sin((degree * (float)n + degree / 2f) * ((float)Math.PI / 180f));
							y = startX * roadPosRate * Mathf.Sin((degree * (float)n + degree / 2f) * ((float)Math.PI / 180f)) + startY * roadPosRate * Mathf.Cos((degree * (float)n + degree / 2f) * ((float)Math.PI / 180f));
							cTransform[m].GetChild(n).localPosition = new Vector3(x, y, cTransform[m].GetChild(n).localPosition.z);
							cTransform[m].GetChild(n).localScale = new Vector3(roadScaleX, roadScaleY, roadScaleZ);
							cTransform[m].GetChild(n).GetComponent<Renderer>().sharedMaterial = mat3;
						}
						continue;
					}
					cTransform[m].GetChild(n).gameObject.SetActive(true);
					cTransform[m].GetChild(n).localRotation = Quaternion.identity;
					cTransform[m].GetChild(n).Rotate(new Vector3(0f, 0f, degree * (float)n + startDegree));
					cTransform[m].GetChild(n).localPosition = new Vector3(0f, 0f, 0f);
					cTransform[m].GetChild(n).localScale = new Vector3(scaleX, scaleY, scaleZ);
					x = startX * Mathf.Cos(degree * (float)n * ((float)Math.PI / 180f)) + (0f - startY) * Mathf.Sin(degree * (float)n * ((float)Math.PI / 180f));
					y = startX * Mathf.Sin(degree * (float)n * ((float)Math.PI / 180f)) + startY * Mathf.Cos(degree * (float)n * ((float)Math.PI / 180f));
					cTransform[m].GetChild(n).localPosition = new Vector3(x, y, cTransform[m].GetChild(n).localPosition.z);
					if (n == 0 || n == cutCount - 1)
					{
						cTransform[m].GetChild(n).GetComponent<Renderer>().sharedMaterial = mat1;
					}
					else
					{
						cTransform[m].GetChild(n).gameObject.SetActive(false);
					}
				}
			}
			break;
		}
		case 5:
		{
			GameManager = GameObject.Find("Zero").GetComponent<GameManager>();
			cTransform = new Transform[mTransform.childCount];
			for (int i = 0; i < mTransform.childCount; i++)
			{
				cTransform[i] = mTransform.GetChild(i);
				cTransform[i].localPosition = new Vector3(0f, 0f, (float)i * offsetZ);
				cTransform[i].localScale = new Vector3(1f, 1f, 1f);
				cTransform[i].rotation = Quaternion.identity;
				cTransform[i].Rotate(new Vector3(0f, 0f, 130f));
				for (int j = 0; j < cTransform[i].childCount; j++)
				{
					cTransform[i].GetChild(j).GetComponent<MeshFilter>().sharedMesh = quad;
					if (j >= cutCount)
					{
						if (j == cutCount)
						{
							cTransform[i].GetChild(j).localRotation = Quaternion.identity;
							cTransform[i].GetChild(j).Rotate(new Vector3(0f, 0f, degree * (float)j + degree / 2f + startDegree));
							x = startX * roadPosRate * Mathf.Cos((degree * (float)j + degree / 2f) * ((float)Math.PI / 180f)) + (0f - startY * roadPosRate) * Mathf.Sin((degree * (float)j + degree / 2f) * ((float)Math.PI / 180f));
							y = startX * roadPosRate * Mathf.Sin((degree * (float)j + degree / 2f) * ((float)Math.PI / 180f)) + startY * roadPosRate * Mathf.Cos((degree * (float)j + degree / 2f) * ((float)Math.PI / 180f));
							cTransform[i].GetChild(j).localPosition = new Vector3(x, y, cTransform[i].GetChild(j).localPosition.z);
							cTransform[i].GetChild(j).localScale = new Vector3(roadScaleX, roadScaleY, roadScaleZ);
							cTransform[i].GetChild(j).GetComponent<Renderer>().sharedMaterial = mat3;
						}
						continue;
					}
					cTransform[i].GetChild(j).gameObject.SetActive(true);
					cTransform[i].GetChild(j).localRotation = Quaternion.identity;
					cTransform[i].GetChild(j).Rotate(new Vector3(0f, 0f, degree * (float)j + startDegree));
					cTransform[i].GetChild(j).localPosition = new Vector3(0f, 0f, 0f);
					if (j == 0 || j == cutCount - 1)
					{
						cTransform[i].GetChild(j).localScale = new Vector3(scaleX, scaleY, scaleZ);
					}
					else
					{
						if (rotX)
						{
							cTransform[i].GetChild(j).Rotate(new Vector3(-30f, 0f, 0f));
						}
						cTransform[i].GetChild(j).localScale = new Vector3(1.5f * scaleX, 1.5f * scaleY, scaleZ);
					}
					x = startX * Mathf.Cos(degree * (float)j * ((float)Math.PI / 180f)) + (0f - startY) * Mathf.Sin(degree * (float)j * ((float)Math.PI / 180f));
					y = startX * Mathf.Sin(degree * (float)j * ((float)Math.PI / 180f)) + startY * Mathf.Cos(degree * (float)j * ((float)Math.PI / 180f));
					cTransform[i].GetChild(j).localPosition = new Vector3(x, y, cTransform[i].GetChild(j).localPosition.z);
					if (j == 0 || j == cutCount - 1)
					{
						cTransform[i].GetChild(j).GetComponent<Renderer>().sharedMaterial = mat1;
					}
					else
					{
						cTransform[i].GetChild(j).GetComponent<Renderer>().sharedMaterial = mat2;
					}
				}
			}
			break;
		}
		}
	}
}
