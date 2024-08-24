using UnityEngine;
using FishNet.Object;
using UnityEngine.UI;

public class LookAt : NetworkBehaviour
{
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private GameObject _lookAt;
    [SerializeField] private float _lookAtDistance = 3f;
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _mouseMovementThreshold = 0.1f;
    
    private Camera _mainCamera;
    private Vector3 _targetLookAtPosition;
    private Vector3 _lastMousePosition;
    private Vector3 _lastCameraPosition;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            Cursor.visible = false;
            _mainCamera = Camera.main;
            _lastMousePosition = Input.mousePosition;
            _lastCameraPosition = _mainCamera.transform.position;
        }
        else
        {
            enabled = false;
            _crosshair.GetComponent<Image>().enabled = false;
        }
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        enabled = false;
        _crosshair.GetComponent<Image>().enabled = false;
    }

    void Update()
    {
        if (!IsOwner || _mainCamera == null) return;

        bool mouseMovement = Vector3.Distance(_lastMousePosition, Input.mousePosition) > _mouseMovementThreshold;
        bool cameraMovement = Vector3.Distance(_lastCameraPosition, _mainCamera.transform.position) > 0.001f;

        if (mouseMovement || cameraMovement)
        {
            UpdateAimDirection();
            if (mouseMovement)
            {
                UpdateCrosshairPosition();
                _lastMousePosition = Input.mousePosition;
            }
            _lastCameraPosition = _mainCamera.transform.position;
        }

        UpdateLookAtPosition();
    }

    void UpdateAimDirection()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _lookAtDistance, _wallLayer))
        {
            _targetLookAtPosition = hit.point;
        }
        else
        {
            _targetLookAtPosition = _mainCamera.transform.position + ray.direction * _lookAtDistance;
        }
    }

    void UpdateLookAtPosition()
    {
        _lookAt.transform.position = Vector3.Lerp(_lookAt.transform.position, _targetLookAtPosition, Time.deltaTime * _smoothSpeed);
    }

    void UpdateCrosshairPosition()
    {
        _crosshair.transform.position = Input.mousePosition;
    }
}