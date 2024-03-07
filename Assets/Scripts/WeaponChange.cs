using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;

public class WeaponChange : NetworkBehaviour
{
    [SerializeField]
    private TwoBoneIKConstraint _leftHand;
    [SerializeField]
    private TwoBoneIKConstraint _rightHand;
    [SerializeField]
    private TwoBoneIKConstraint _leftThumb;

    private CinemachineVirtualCamera _cam;
    private GameObject _camObject;

    [SerializeField]
    private RigBuilder _rig;
    [SerializeField]
    private Transform[] _leftTargets;
    [SerializeField]
    private Transform[] _rightTargets;
    [SerializeField]
    private Transform[] _thumbTargets;
    [SerializeField]
    private GameObject[] _weapons;
    private int _weaponNumber = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            _camObject = GameObject.Find("PlayerCam");
            _cam = _camObject.GetComponent<CinemachineVirtualCamera>();
            _cam.Follow = this.gameObject.transform;
            _cam.LookAt = this.gameObject.transform;
        }
        else
        {
            gameObject.GetComponent<WeaponChange>().enabled = false;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UpdateWeaponChangeServer(Input.GetMouseButtonDown(1));
        }
    }

    [ServerRpc]
    public void UpdateWeaponChangeServer(bool mouseButtonDown1)
    {
        UpdateWeaponChange(mouseButtonDown1);
    }

    [ObserversRpc]
    public void UpdateWeaponChange(bool mouseButtonDown1)
    {
        if (mouseButtonDown1)
        {
            _weaponNumber++;
            if (_weaponNumber > _weapons.Length - 1)
            {
                _weaponNumber = 0;
            }
            for (int i = 0; i < _weapons.Length; i++)
            {
                _weapons[i].SetActive(false);
            }
            _weapons[_weaponNumber].SetActive(true);
            _leftHand.data.target = _leftTargets[_weaponNumber];
            _rightHand.data.target = _rightTargets[_weaponNumber];
            _leftThumb.data.target = _thumbTargets[_weaponNumber];
            _rig.Build();
        }
    }
}
