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
    private GameObject _lookAtObject;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
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
