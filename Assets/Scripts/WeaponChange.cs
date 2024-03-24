using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;
using TMPro;
using UnityEngine.UI;

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
    private Image _weaponIcon;
    private TMP_Text _ammoAmtText;
    [SerializeField]
    private Sprite[] _weaponIcons;
    [SerializeField]
    private int[] _ammoAmts;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            _weaponIcon = GameObject.Find("WeaponUI").GetComponent<Image>();
            _ammoAmtText = GameObject.Find("AmmoAmt").GetComponent<TMP_Text>();
            _camObject = GameObject.Find("PlayerCam");
            _cam = _camObject.GetComponent<CinemachineVirtualCamera>();
            var o = gameObject;
            _cam.Follow = o.transform;
            _cam.LookAt = o.transform;
            _ammoAmtText.text = _ammoAmts[0].ToString();
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
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UpdateWeaponChangeServer();
        }
    }

    [ServerRpc]
    private void UpdateWeaponChangeServer()
    {
        UpdateWeaponChange();
    }

    [ObserversRpc(BufferLast = true)]
    private void UpdateWeaponChange()
    {
        _weaponNumber++;
        if (_weaponNumber > _weapons.Length - 1)
        {
            _weaponIcon.GetComponent<Image>().sprite = _weaponIcons[0];
            _ammoAmtText.text = _ammoAmts[0].ToString();
            _weaponNumber = 0;
        }
        foreach (var t in _weapons)
        {
            t.SetActive(false);
        }
        _weapons[_weaponNumber].SetActive(true);
        _weaponIcon.GetComponent<Image>().sprite = _weaponIcons[_weaponNumber];
        _ammoAmtText.text = _ammoAmts[_weaponNumber].ToString();
        _leftHand.data.target = _leftTargets[_weaponNumber];
        _rightHand.data.target = _rightTargets[_weaponNumber];
        _leftThumb.data.target = _thumbTargets[_weaponNumber];
        _rig.Build();
    }
}
