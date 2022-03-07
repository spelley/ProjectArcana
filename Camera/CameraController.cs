using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    CinemachineBrain brain;
    CinemachineVirtualCamera vcam;
    [SerializeField]
    Camera minimapCamera;
    [SerializeField]
    Transform minimapFocusPoint;

    [SerializeField]
    Transform focusPoint;
    [SerializeField]
    Transform followTarget;

    [SerializeField]
    float maxSize = 10f;

    [SerializeField]
    float minSize = 4f;

    [SerializeField]
    float zoomSpeed = 5f;

    [SerializeField]
    float rotationSpeed = 5f;
    [SerializeField]
    float scrollSpeed = 5f;
    [SerializeField]
    float maxPanDistance = 50f;

    [SerializeField]
    List<float> rotations = new List<float>();

    int curRotation = 0;

    Vector3 movePosition;

    void Awake()
    {
        FindCamera();
        curRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(vcam == null)
        {
            FindCamera();
        }
        else
        {
            if(followTarget != null)
            {
                focusPoint.position = followTarget.position + movePosition;
                minimapFocusPoint.position = followTarget.position + new Vector3(0, 10f, 0);
            }
            float curSize = vcam.m_Lens.OrthographicSize;
            if((Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.Plus)) && curSize > minSize)
            {
                vcam.m_Lens.OrthographicSize = Mathf.Clamp(curSize - (zoomSpeed * Time.deltaTime), minSize, maxSize);
            }
            else if(Input.GetKey(KeyCode.Minus) && curSize < maxSize)
            {
                vcam.m_Lens.OrthographicSize = Mathf.Clamp(curSize + (zoomSpeed * Time.deltaTime), minSize, maxSize);
            }
            
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                SetFocus(followTarget.gameObject);
            }
        }
    }

    void LateUpdate()
    {
        //HandleLockedRotations();
        HandleSmoothedRotations();
        HandleScroll();
    }

    void HandleScroll()
    {
        float scrollHorizontal = 0;
        float scrollVertical = 0;
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            scrollHorizontal = -1;
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            scrollHorizontal = 1;
        }

        if(Input.GetKey(KeyCode.DownArrow))
        {
            scrollVertical = -1;
        }
        else if(Input.GetKey(KeyCode.UpArrow))
        {
            scrollVertical = 1;
        }

        if(scrollVertical != 0 || scrollHorizontal != 0)
        {
            Vector3 input = Quaternion.Euler(0, vcam.transform.eulerAngles.y, 0) * new Vector3(scrollHorizontal, 0.0f, scrollVertical);
            Vector3 newMovePos = movePosition + input * scrollSpeed * Time.deltaTime;
            if(Mathf.Abs(newMovePos.x) < maxPanDistance && Mathf.Abs(newMovePos.z) < maxPanDistance)
            {
                movePosition = newMovePos;
            }
        }

        focusPoint.position += movePosition;
    }

    void HandleLockedRotations()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            curRotation = (curRotation == 0) ? (rotations.Count - 1) : (curRotation - 1);
            vcam.transform.localRotation = Quaternion.Euler(30f, rotations[curRotation], 0);
            minimapCamera.transform.localRotation = Quaternion.Euler(90f, rotations[curRotation], 0);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            curRotation = (curRotation == (rotations.Count - 1)) ? 0 : (curRotation + 1);
            vcam.transform.localRotation = Quaternion.Euler(30f, rotations[curRotation], 0);
            minimapCamera.transform.localRotation =  Quaternion.Euler(90f, rotations[curRotation], 0);
        }
    }

    void HandleSmoothedRotations()
    {
        int modifier = 0;
        if(Input.GetKey(KeyCode.E))
        {
            modifier = -1;
        }
        else if(Input.GetKey(KeyCode.Q))
        {
            modifier = 1;
        }
        if(modifier != 0)
        {
            vcam.transform.localRotation = Quaternion.Euler(30f, (vcam.transform.eulerAngles.y + (modifier * rotationSpeed * Time.deltaTime)), 0);
            minimapCamera.transform.localRotation = Quaternion.Euler(90f, (vcam.transform.eulerAngles.y + (modifier * rotationSpeed * Time.deltaTime)), 0);
        }
    }

    void FindCamera()
    {
        cam = Camera.main;
        brain = (GetComponent<Camera>() == null) ? null : cam.GetComponent<CinemachineBrain>();
        vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    public void SetFocus(GameObject target)
    {
        movePosition = Vector3.zero;
        followTarget = target.transform;
    }
}
