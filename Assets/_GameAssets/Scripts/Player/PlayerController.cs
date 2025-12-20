using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Referance")]
    [SerializeField] Transform _orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] KeyCode _movementKey;
    [SerializeField] float _movementSpeed;

    [Header("Jump Settings")]
    [SerializeField] KeyCode _jumpKey;
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpCooldown;
    [SerializeField] private bool _canJump;

    [Header("Sliding Settings")]
    [SerializeField] KeyCode _slideKey;
    [SerializeField] float _slideMultiplayer;
    [SerializeField] float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _playerHeight;
    [SerializeField] float _groundDrag;

    private Rigidbody _playerRigidbody;

    private float _horizontalInput,_verticalInput;

    private Vector3 movementDirection;

    private bool isSliding;

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        SetInput();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }
    private void FixedUpdate()
    {
        SetMovement();
    }

    private void SetInput()
    {
        _horizontalInput=Input.GetAxisRaw("Horizontal");
        _verticalInput=Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(_slideKey))
        {
            isSliding = true;
        } 

        else if(Input.GetKeyDown(_movementKey))
        {
            isSliding = false;
        }

        else if(Input.GetKey(_jumpKey) && _canJump && IsGrounded())
        {
            _canJump = false;
            setPlayerJumpMovement();
            Invoke(nameof(ResetJumping), _jumpCooldown);
        }
    }

    private void SetMovement()
    {
        movementDirection = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;

        if(isSliding)
        {
            transform.position += movementDirection.normalized * _movementSpeed * _slideMultiplayer * Time.fixedDeltaTime;
        }
        else
        {
            transform.position += movementDirection.normalized * _movementSpeed * Time.fixedDeltaTime;
        }
    }

    private void SetPlayerDrag()
    {
        if(isSliding)
        {
            _playerRigidbody.linearDamping = _slideDrag;
        }
        else
        {
            _playerRigidbody.linearDamping = _groundDrag;
        }
    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);

        if(flatVelocity.magnitude > _movementSpeed)
        {
            Vector3 limitVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigidbody.linearVelocity = new Vector3(limitVelocity.x, _playerRigidbody.linearVelocity.y, limitVelocity.z);
        }
    }
    private void setPlayerJumpMovement()
    {
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJumping()
    {
        _canJump = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);
    }
}
