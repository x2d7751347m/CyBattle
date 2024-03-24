using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using Cinemachine;
using FishNet;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;

public class LookAt : NetworkBehaviour
{
    private Vector3 _worldPosition;
    private Vector3 _screenPosition;
    [SerializeField]
    private GameObject _crosshair;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            Cursor.visible = false;
        }
        else
        {
            gameObject.GetComponent<LookAt>().enabled = false;
            _crosshair.GetComponent<Image>().enabled = false;
        }
    }

    private void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _screenPosition = Input.mousePosition;
        _screenPosition.z = 3f;

        _worldPosition = Camera.main.ScreenToWorldPoint(_screenPosition);
        transform.position = _worldPosition;

        _crosshair.transform.position = Input.mousePosition;
    }
}
