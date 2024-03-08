using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;
using UnityEngine.UIElements;
using System.Threading;

public class PlayerMovemont : NetworkBehaviour
{
    [SerializeField]
    private long _userId;
    [SerializeField]
    private float _moveSpeed = 7f;
    [SerializeField]
    private float _rotateSpeed = 100.0f;
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private Animator _anim;
    private bool _canJump = true;
    private bool _isJumpPressed = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
            gameObject.GetComponent<PlayerMovemont>().enabled = false;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
        }
        else
        {
            gameObject.GetComponent<PlayerMovemont>().enabled = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        if (_isJumpPressed && _canJump == true)
        {

            Jump();
            StartCoroutine(JumpAgain());
        }
        if (Input.GetAxis("Mouse X") != 0 || movement != Vector3.zero)
        {
            UpdateMovementLocal(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("Mouse X"), Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * _rotateSpeed * Time.deltaTime, 0)));
        }
        if (movement != Vector3.zero)
        {
            UpdateAnimationLocal(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _canJump)
        {
            _isJumpPressed = true;
        }
    }

    IEnumerator JumpAgain()
    {
        yield return new WaitForSeconds(2);
        _canJump = true;
    }

    [ServerRpc]
    public void UpdateMovementServer(float vertical, float horizontal, float mouseX, Quaternion rotation)
    {
        UpdateMovement(vertical, horizontal, mouseX, rotation);
    }

    [ObserversRpc]
    public void UpdateMovement(float vertical, float horizontal, float mouseX, Quaternion rotation)
    {
        _rb.MoveRotation(_rb.rotation * rotation);
        _rb.MovePosition(_rb.position + transform.forward * vertical * _moveSpeed * Time.deltaTime + transform.right * horizontal * _moveSpeed * Time.deltaTime);
        _anim.SetFloat("BlendV", vertical);
        _anim.SetFloat("BlendH", horizontal);
    }

    [ServerRpc]
    public void UpdateAnimationServer(float vertical, float horizontal)
    {
        _canJump = false;
        UpdateAnimation(vertical, horizontal);
    }

    [ObserversRpc]
    public void UpdateAnimation(float vertical, float horizontal)
    {
        _anim.SetFloat("BlendV", vertical);
        _anim.SetFloat("BlendH", horizontal);
    }

    public void UpdateAnimationLocal(float vertical, float horizontal)
    {
        _anim.SetFloat("BlendV", vertical);
        _anim.SetFloat("BlendH", horizontal);
    }

    public void UpdateMovementLocal(float vertical, float horizontal, float mouseX, Quaternion rotation)
    {
        _rb.MoveRotation(_rb.rotation * rotation);
        _rb.MovePosition(_rb.position + transform.forward * vertical * _moveSpeed * Time.deltaTime + transform.right * horizontal * _moveSpeed * Time.deltaTime);
        //anim.SetFloat("BlendV", vertical);
        //anim.SetFloat("BlendH", horizontal);
    }

    [ServerRpc]
    public void UpdateJumpServer(bool jump, bool canJump)
    {
        UpdateJump(jump, canJump);
    }

    [ObserversRpc]
    public void UpdateJump(bool jump, bool canJump)
    {
        if (jump && canJump == true)
        {
            Jump();
            StartCoroutine(JumpAgain());
        }
    }

    void Jump()
    {
        _isJumpPressed = false;
        _canJump = false;
        _rb.AddForce(Vector3.up * 500 * Time.deltaTime, ForceMode.VelocityChange);
    }
    public void SetUserId(long userId)
    {
        _userId = userId;
    }
}
