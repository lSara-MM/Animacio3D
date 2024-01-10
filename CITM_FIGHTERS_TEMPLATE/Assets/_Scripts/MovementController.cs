using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    Vector2 MoveLimits = new Vector2(-5, 5);

    [SerializeField]
    float SafetyDistance = 0.5f;
    // Start is called before the first frame update

    private bool _isColliding = false;

    #region AnimationParamNames
    const string SPEED = "Speed";
    const string ATTACK_HIGH_QUICK = "AttackHighQuick";
    const string DIE = "Die";
    const string WIN = "Win";

    #endregion

    Rigidbody m_Rigidbody;

    private Animator _animator;
    private Transform _otherPlayer;

    public static int _playercount;
    int _id;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        _id = _playercount++;
    }

    public void SetOtherPlayer(Transform other)
    {
        _otherPlayer = other;
    }


    public void TryMove(float speed)
    {
        if (CanMove(speed) && speed != 0f)
        {
            float directionSpeed = _id == 1 ? -speed : speed;

            _animator.SetFloat(SPEED, speed > 0f ? 1 : -1);

            float deltaSpeed = directionSpeed * Time.deltaTime;

            Vector3 pos = transform.position;
            pos.x += deltaSpeed;
            transform.position = pos;
        }
        else
        {
            _animator.SetFloat(SPEED, 0);
        }
    }

    private void FixedUpdate()
    {
        //Store user input as a movement vector
        Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition
        m_Rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime * 1);
    }

    public void LateUpdate()
    {
    }

    bool CanMove(float speed)
    {
        if (speed < 0)
            return CanMoveLeft();
        if (speed > 0)
            return CanMoveRight();

        return true;
    }

    bool CanMoveLeft()
    {
        if (transform.position.x <= MoveLimits.x)
            return false;

        if (_otherPlayer == null)
            return true;

        if (_id == 1 && transform.position.x <= (_otherPlayer.position.x + SafetyDistance))
            return false;

        return true;
    }
    bool CanMoveRight()
    {
        if (transform.position.x >= MoveLimits.y)
            return false;

        if (_otherPlayer == null)
            return true;

        if (_id == 0 && transform.position.x >= (_otherPlayer.position.x - SafetyDistance))
            return false;

        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            _isColliding = true;
            Debug.Log("OnCollisionEnter Player");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            _isColliding = false;
            Debug.Log("OnCollisionExit Player");
        }
    }
}
