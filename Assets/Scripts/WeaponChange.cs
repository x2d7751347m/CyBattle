using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WeaponChange : NetworkBehaviour
{
    [SerializeField] private TwoBoneIKConstraint _leftHand;
    [SerializeField] private TwoBoneIKConstraint _rightHand;
    [SerializeField] private TwoBoneIKConstraint _leftThumb;

    private CinemachineVirtualCamera _cam;
    private GameObject _camObject;

    [SerializeField] private RigBuilder _rig;
    [SerializeField] private Transform[] _leftTargets;
    [SerializeField] private Transform[] _rightTargets;
    [SerializeField] private Transform[] _thumbTargets;
    [SerializeField] private GameObject[] _weapons;
    [SyncVar]private int _weaponNumber = 0;
    private Image _weaponIcon;
    private TMP_Text _ammoAmtText;
    [SerializeField] private Sprite[] _weaponIcons;
    [SerializeField] private int[] _ammoAmts;

    [SerializeField] private GameObject[] _muzzleFlash;

    [SerializeField]
    private string _shooterName;
    private string _gotShotName;
    public float[] DamageAmts;
    public bool IsDead;

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

            _shooterName = InstanceFinder.NetworkManager.GetComponent<GameManager>().PlayerNickName;
            SetShooterNameServer(_shooterName);
        }
        else
        {
            _leftHand.data.target = _leftTargets[_weaponNumber];
            _rightHand.data.target = _rightTargets[_weaponNumber];
            _leftThumb.data.target = _thumbTargets[_weaponNumber];
            _rig.Build();
            gameObject.GetComponent<WeaponChange>().enabled = false;
        }
    }

    [ServerRpc]
    private void SetShooterNameServer(string shooterName)
    {
        _shooterName = shooterName;
        SetShooterNameObserver(shooterName);
    }

    [ObserversRpc(BufferLast = true, ExcludeOwner = true, ExcludeServer = true)]
    private void SetShooterNameObserver(string shooterName)
    {
        _shooterName = shooterName;
    }

    void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsDead) return;
        if (Input.GetMouseButtonDown(0) && _ammoAmts[_weaponNumber] > 0)
        {
            _ammoAmts[_weaponNumber]--;
            _ammoAmtText.text = _ammoAmts[_weaponNumber].ToString();
            GetComponent<DisplayColor>().PlayGunShot(_weaponNumber);
            GunMuzzleFlash(_weaponNumber);
            GetComponent<DisplayColor>().PlayGunShotServer(_weaponNumber);
            GunMuzzleFlashServer(_weaponNumber);
            Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (Physics.Raycast(ray, out var hit, 500))
            {
                if (hit.transform.gameObject.GetComponent<WeaponChange>() != null)
                {
                    _gotShotName = hit.transform.gameObject.GetComponent<WeaponChange>()._shooterName;
                    hit.transform.gameObject.GetComponent<DisplayColor>()
                        .DeliverDamage(_gotShotName, DamageAmts[_weaponNumber]);
                }
            }

            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        if (Input.GetMouseButtonDown(1))
        {
            UpdateWeaponChangeServer();
        }
    }

    private void GunMuzzleFlash(int weaponNumber)
    {
        _muzzleFlash[weaponNumber].SetActive(true);
        StartCoroutine(MuzzleOff());
    }

    [ServerRpc]
    private void GunMuzzleFlashServer(int weaponNumber)
    {
        GunMuzzleFlashObserver(weaponNumber);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void GunMuzzleFlashObserver(int weaponNumber)
    {
        GunMuzzleFlash(weaponNumber);
    }

    [ServerRpc]
    private void UpdateWeaponChangeServer()
    {
        UpdateWeaponChange();
    }

    [ObserversRpc()]
    private void UpdateWeaponChange()
    {
        _weaponNumber++;
        if (_weaponNumber > _weapons.Length - 1)
        {
            if (IsOwner)
            {
                _weaponIcon.GetComponent<Image>().sprite = _weaponIcons[0];
                _ammoAmtText.text = _ammoAmts[0].ToString();
            }
            _weaponNumber = 0;
        }

        foreach (var t in _weapons)
        {
            t.SetActive(false);
        }

        _weapons[_weaponNumber].SetActive(true);
        
        if (IsOwner)
        {
            _weaponIcon.GetComponent<Image>().sprite = _weaponIcons[_weaponNumber];
            _ammoAmtText.text = _ammoAmts[_weaponNumber].ToString();
        }
        _leftHand.data.target = _leftTargets[_weaponNumber];
        _rightHand.data.target = _rightTargets[_weaponNumber];
        _leftThumb.data.target = _thumbTargets[_weaponNumber];
        _rig.Build();
    }

    IEnumerator MuzzleOff()
    {
        yield return new WaitForSeconds(0.03f);
        _muzzleFlash[_weaponNumber].SetActive(false);
    }

    public void AddAmmo(int weaponNumber, int ammo)
    {
        _ammoAmts[weaponNumber] += ammo;
        if (_weaponNumber == weaponNumber)
        {
            _ammoAmtText.text = _ammoAmts[_weaponNumber].ToString();
        }
    }
}