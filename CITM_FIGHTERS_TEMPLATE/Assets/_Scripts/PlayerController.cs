using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject ImpactPrefab;

    // Start is called before the first frame update
    private bool _isAttacking;
    private bool _isBlocking;

    private UpDown UpOrDown;

    private bool CanAttack => !_isAttacking && !_isBlocking;
    private bool CanBlock => !_isAttacking && !_isBlocking;

    public bool Dead => _dead;

    public Camera cam;
    [SerializeField] bool camAnim = false;

    // Audios
    [SerializeField] AudioSource atk;
    [SerializeField] AudioSource dodgeLow;
    [SerializeField] AudioSource dodge;
    [SerializeField] AudioSource die;
    [SerializeField] AudioSource win;

    [SerializeField] GameObject music;

    bool dieAudio = false;

    #region AnimationParamNames
    const string SPEED = "Speed";
    const string ATTACK_HIGH_QUICK = "AttackHighQuick";
    const string ATTACK_HIGH_SLOW = "AttackHighSlow";
    const string ATTACK_LOW_QUICK = "AttackLowQuick";
    const string ATTACK_LOW_SLOW = "AttackLowSlow";

    const string BLOCK_HIGH = "BlockHigh";
    const string BLOCK_LOW = "BlockLow";


    const string DIE = "Die";
    const string WIN = "Win";



    #endregion

    private Animator _animator;
    private Transform _otherPlayer;

    static int _playercount;
    int _id;

    private bool _dead;
    private bool _win = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        music = GameObject.Find("Music");

        _id = _playercount++;
    }

    private void Update()
    {
        if ((_isAttacking || _isBlocking) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99)
        {
            if (_isAttacking)
            {
                _isAttacking = false;
                //Debug.Log("Attacking false");
            }

            if (_isBlocking)
            {
                _isBlocking = false;
                //Debug.Log("Blocking false");
            }
        }

        if (_win && camAnim)
        {
            Vector3 target = transform.position - new Vector3(0, -2, 3);
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, target, 5f * Time.deltaTime);
        }
    }

    public void SetOtherPlayer(Transform other)
    {
        _otherPlayer = other;
    }

    internal void SetAtacking(bool value, UpDown upDown)
    {
        _isAttacking = value;
        UpOrDown = upDown;

        if (atk != null) atk.Play();

        //Debug.Log(value);
    }



    internal void SetBlocking(bool value, UpDown upDown)
    {
        _isBlocking = value;
        UpOrDown = upDown;
    }

    public void TryHighQuickAttack()
    {
        if (CanAttack)
        {
            _animator.SetTrigger(ATTACK_HIGH_QUICK);
            SetAtacking(true, UpDown.Up);
        }
    }
    public void TryHighSlowAttack()
    {
        if (CanAttack)
        {
            _animator.SetTrigger(ATTACK_HIGH_SLOW);
            SetAtacking(true, UpDown.Up);
        }
    }
    public void TryLowQuickAttack()
    {
        if (CanAttack)
        {
            _animator.SetTrigger(ATTACK_LOW_QUICK);
            SetAtacking(true, UpDown.Down);
        }
    }
    public void TryLowSlowAttack()
    {
        if (CanAttack)
        {
            _animator.SetTrigger(ATTACK_LOW_SLOW);
            SetAtacking(true, UpDown.Down);
        }
    }

    internal void TryHighBlock()
    {
        if (CanBlock)
        {
            _animator.SetTrigger(BLOCK_HIGH);
            SetBlocking(true, UpDown.Up);

            if (dodge != null) dodge.Play();
        }
    }

    internal void TryLowBlock()
    {
        if (CanBlock)
        {
            _animator.SetTrigger(BLOCK_LOW);
            SetBlocking(true, UpDown.Down);

            if (dodgeLow != null) dodgeLow.Play();
        }
    }


    public void OnHit(Transform hit)
    {
        var hitBy = hit.root.GetComponent<PlayerController>();
        if (hitBy.transform == _otherPlayer && hitBy._isAttacking && !hitBy.Dead && !_dead)
        {
            if (!_isBlocking || hitBy.UpOrDown != this.UpOrDown || hitBy.Dead)
            {
                Die();
                hitBy.Win();
                Instantiate(ImpactPrefab, hit.position, Quaternion.identity);
            }
        }
    }

    private void Die()
    {
        _animator.SetTrigger(DIE);

        if (die != null && !dieAudio)
        {
            _dead = true;
            dieAudio = true;
            die.Play();
        }

        StartCoroutine(DieLater());
    }

    IEnumerator DieLater()
    {
        yield return new WaitForSeconds(0.1f);
        _dead = true;
        yield return new WaitForSeconds(5);
        PlayerStart.nPLayers = 0;
        _playercount = 0;
        MovementController._playercount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Win()
    {
        _animator.SetTrigger(WIN);
        _win = true;

        music.SetActive(false);

        if (win != null && !dieAudio)
        {
            dieAudio = true; 
            win.Play();
        }
    }
}

public enum UpDown
{
    Up,
    Down
}
