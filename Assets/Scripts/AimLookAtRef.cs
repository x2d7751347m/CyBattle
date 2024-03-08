using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class AimLookAtRef : NetworkBehaviour
{
    [SerializeField]
    private RigBuilder _rig;
    [SerializeField]
    private GameObject _lookAtObject;
    public override void OnStartServer()
    {
        base.OnStartServer();
        gameObject.GetComponent<AimLookAtRef>().enabled = false;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            _rig.Build();
        }
        else
        {
            gameObject.GetComponent<AimLookAtRef>().enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = _lookAtObject.transform.position;
    }
}
