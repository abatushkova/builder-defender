using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float orthographicSize;
    private float targetOrthographicSize;
    private bool edgeScrolling;

    private void Awake()
    {
        Instance = this;

        edgeScrolling = PlayerPrefs.GetInt("edgeScrolling", 1) == 1; // 1 - enabled by default
    }

    private void Start()
    {
        orthographicSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        targetOrthographicSize = orthographicSize;
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (edgeScrolling)
        {
            // mouse near screen edge - move camera in that direction
            float edgeScrollingSize = 25f; // in pixels
            if (Input.mousePosition.x > Screen.width - edgeScrollingSize)
            {
                x = +1f;
            }
            if (Input.mousePosition.x < edgeScrollingSize)
            {
                x = -1f;
            }
            if (Input.mousePosition.y > Screen.height - edgeScrollingSize)
            {
                y = +1f;
            }
            if (Input.mousePosition.y < edgeScrollingSize)
            {
                y = -1f;
            }
        }

        float moveSpeed = 30f;
        Vector3 moveDir = new Vector3(x, y).normalized;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomAmount = 2f;
        targetOrthographicSize += -Input.mouseScrollDelta.y * zoomAmount;

        float minOrthographicSize = 10f;
        float maxOrthographicSize = 30f;
        targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minOrthographicSize, maxOrthographicSize);

        float zoomSpeed = 5f;
        orthographicSize = Mathf.Lerp(orthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);

        cinemachineVirtualCamera.m_Lens.OrthographicSize = orthographicSize;
    }

    public void SetEdgeScrolling(bool edgeScrolling)
    {
        this.edgeScrolling = edgeScrolling;
        PlayerPrefs.SetInt("edgeScrolling", edgeScrolling ? 1 : 0);
    }

    public bool GetEdgeScrolling()
    {
        return edgeScrolling;
    }
}
