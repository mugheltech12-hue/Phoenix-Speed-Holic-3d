using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    private float CENTER = (float)Screen.width / 2f;

    public Transform mTransform;
    public Transform chdTransform;
    public Transform birdTransform;
    public float degree;

    // ── Naye UI Button fields ──
    public Button leftButton;
    public Button rightButton;

    private Transform camTransform;
    private bool isClockWise;
    private bool isRotate;
    private bool lastDownIsLeft;
    private bool lDown;
    private bool rDown;

    private Quaternion targetBirdRotation;
    private bool isBirdRotating = false;
    private float birdRotationSpeed = 500f;

    public float fixedX = -20f;
    private float fixedZ = 0f;
    private float originalBirdYRotation = 0f;

    private void Awake()
    {
        camTransform = Camera.main.transform;
        mTransform = transform;

        birdTransform.localEulerAngles = new Vector3(fixedX, 0f, 0f);
        targetBirdRotation = birdTransform.rotation;

        // ── Button listeners attach karo ──
        if (leftButton != null)
        {
            var l = leftButton.gameObject.AddComponent<UIButtonListener>();
            l.onDown = () => { lastDownIsLeft = true; isClockWise = true; isRotate = true; lDown = true; RotateBirdRelative(-45f); };
            l.onUp = () => { lDown = false; isRotate = rDown; ResetBirdRotation(); };
        }

        if (rightButton != null)
        {
            var r = rightButton.gameObject.AddComponent<UIButtonListener>();
            r.onDown = () => { lastDownIsLeft = false; isClockWise = false; isRotate = true; rDown = true; RotateBirdRelative(45f); };
            r.onUp = () => { rDown = false; isRotate = lDown; ResetBirdRotation(); };
        }
    }

    private void Update()
    {
        CheckTouch();
        CheckKeyboard();
        CheckRotate();
        SmoothBirdRotation();
    }

    private void CheckTouch()
    {
        if (Input.touchCount == 0) return;

        Touch[] touches = Input.touches;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (touches[i].phase == TouchPhase.Began)
            {
                if (touches[i].position.x > CENTER)
                {
                    lastDownIsLeft = false; isClockWise = false; isRotate = true; rDown = true;
                    RotateBirdRelative(45f);
                }
                else
                {
                    lastDownIsLeft = true; isClockWise = true; isRotate = true; lDown = true;
                    RotateBirdRelative(-45f);
                }
            }
            else if (touches[i].phase == TouchPhase.Ended || touches[i].phase == TouchPhase.Canceled)
            {
                if (touches[i].position.x > CENTER) { rDown = false; isRotate = lDown; }
                else { lDown = false; isRotate = rDown; }
                ResetBirdRotation();
            }
        }
    }

    private void CheckKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) { lastDownIsLeft = false; isClockWise = false; isRotate = true; rDown = true; RotateBirdRelative(45f); }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) { lastDownIsLeft = true; isClockWise = true; isRotate = true; lDown = true; RotateBirdRelative(-45f); }

        if (Input.GetKeyUp(KeyCode.RightArrow)) { rDown = false; isRotate = lDown; }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) { lDown = false; isRotate = rDown; }

        if (!lDown && !rDown) ResetBirdRotation();
    }


    private void CheckRotate()
    {
        if (isRotate)
        {
            float rotationAngle = degree * Time.deltaTime;
            if (isClockWise)
            {
                mTransform.Rotate(0f, 0f, -rotationAngle);
                camTransform.Rotate(0f, 0f, rotationAngle);
                chdTransform.Rotate(0f, 0f, 10f * rotationAngle);
            }
            else
            {
                mTransform.Rotate(0f, 0f, rotationAngle);
                camTransform.Rotate(0f, 0f, -rotationAngle);
                chdTransform.Rotate(0f, 0f, -10f * rotationAngle);
            }
        }
        // ✅ HATA DO ye wali lines jo bina input ke rotate karti thi:
        // else if (lastDownIsLeft) { chdTransform.Rotate(...); }
        // else { chdTransform.Rotate(...); }
        //
        // Ab jab koi button nahi daba to kuch nahi hoga
    }
    //private void CheckRotate()
    //{
    //    if (isRotate)
    //    {
    //        float rotationAngle = degree * Time.deltaTime;
    //        if (isClockWise)
    //        {
    //            mTransform.Rotate(0f, 0f, -rotationAngle);
    //            camTransform.Rotate(0f, 0f, rotationAngle);
    //            chdTransform.Rotate(0f, 0f, 10f * rotationAngle);
    //        }
    //        else
    //        {
    //            mTransform.Rotate(0f, 0f, rotationAngle);
    //            camTransform.Rotate(0f, 0f, -rotationAngle);
    //            chdTransform.Rotate(0f, 0f, -10f * rotationAngle);
    //        }
    //    }
    //    else if (lastDownIsLeft) { chdTransform.Rotate(0f, 0f, 10f * degree * Time.deltaTime); }
    //    else { chdTransform.Rotate(0f, 0f, -10f * degree * Time.deltaTime); }
    //}

    float newYRotation = -90;

    private void RotateBirdRelative(float yRotationOffset)
    {
        newYRotation = originalBirdYRotation + yRotationOffset;
        targetBirdRotation = Quaternion.Euler(fixedX, newYRotation, fixedZ);
        isBirdRotating = true;
    }

    private void ResetBirdRotation()
    {
        newYRotation = 0f;
        isBirdRotating = true;
    }

    private void SmoothBirdRotation()
    {
        if (isBirdRotating)
        {
            float smoothY = Mathf.Lerp(birdTransform.eulerAngles.y, newYRotation, Time.deltaTime * birdRotationSpeed);
            birdTransform.localEulerAngles = new Vector3(fixedX, smoothY, 0f);

            if (Mathf.Abs(transform.eulerAngles.y - newYRotation) < 1)
                isBirdRotating = false;
        }
    }


    public void ResetRotationState()
    {
        isRotate = false;
        lDown = false;
        rDown = false;
        isClockWise = false;
        lastDownIsLeft = false;
        isBirdRotating = false;
        newYRotation = 0f;

        if (camTransform != null)
            camTransform.localRotation = Quaternion.identity;

        if (chdTransform != null)
            chdTransform.localRotation = Quaternion.identity;

        birdTransform.localPosition = new Vector3(0f, 0.232f, 0f);
        birdTransform.localRotation = Quaternion.identity;
        targetBirdRotation = Quaternion.identity;
    }

    public void ResetState()
    {
        lDown = false;
        rDown = false;
        isRotate = false;
        lastDownIsLeft = false;  // neutral position

        // Bird rotation bhi reset
        newYRotation = 0f;
        isBirdRotating = true;
        targetBirdRotation = Quaternion.Euler(fixedX, 0f, fixedZ);

        // chdTransform rotation reset (trail/camera child)
        chdTransform.localRotation = Quaternion.identity;
    }

    // Sirf input reset — camera/rotation touch nahi hoga
    public void ResetInputState()
    {
        isRotate = false;
        lDown = false;
        rDown = false;
        isClockWise = false;
        lastDownIsLeft = false;
        isBirdRotating = false;
        newYRotation = 0f;

        birdTransform.localPosition = new Vector3(0f, 0.232f, 0f);
        birdTransform.localRotation = Quaternion.identity;
        targetBirdRotation = Quaternion.identity;
    }

    // chdTransform ka euler return karo
    public Vector3 GetChdLocalEuler()
    {
        return chdTransform.localEulerAngles;
    }

    // chdTransform ko saved euler se restore karo
    public void RestoreChdEuler(Vector3 euler)
    {
        chdTransform.localEulerAngles = euler;
    }



    public void ResetToCenter(float centeredZ)
    {
        isRotate = false;
        lDown = false;
        rDown = false;
        isClockWise = false;
        lastDownIsLeft = false;
        isBirdRotating = false;
        newYRotation = 0f;

        // mTransform centered rotation
        mTransform.eulerAngles = new Vector3(0f, 0f, centeredZ);

        // Camera opposite
        if (camTransform != null)
            camTransform.localEulerAngles = new Vector3(5f, 0f, -centeredZ);

        // chdTransform bhi camera ke saath
        if (chdTransform != null)
            chdTransform.localEulerAngles = new Vector3(0f, 0f, -centeredZ * 10f);

        birdTransform.localPosition = new Vector3(0f, 0.232f, 0f);
        birdTransform.localRotation = Quaternion.identity;
        targetBirdRotation = Quaternion.identity;
    }
}