using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Shade : MonoBehaviour
{
    public enum State
    {
        IDLE,
        AWARE,
        RIGHT_TURN,
        LEFT_TURN,
        RIGHT_TRACE,
        LEFT_TRACE,
        RIGHT_ATTACK,
        LEFT_ATTACK,
        RIGHT_SKILL,
        LEFT_SKILL,
        RETURN,
        AWAKE,
        DEATH,
    }
    public State state;


    public ParticleSystem skill;
    public GameObject Attack;
    public LayerMask Wall;
    public LayerMask Ground;
    Transform player;
    Vector3 defaultPos;
    Animator anim;

    public float traceDist;
    public float attackDist;
    public float traceSpeed;
    public float attackDelay = 3f;
    float attackDelayTimer;
    public Vector3 traceOffset;

    int hp = 10;

    bool isDie;
    bool isAware;
    bool isAttack;
    bool isUseSkill;
    bool usingSkill;
    bool isTracing;
    bool isReturning;
    bool stopMove;

    readonly int hashAware = Animator.StringToHash("AWARE");
    readonly int hashRightTurn = Animator.StringToHash("RIGHT_TURN");
    readonly int hashLeftTurn = Animator.StringToHash("LEFT_TURN");
    readonly int hashRightTrace = Animator.StringToHash("RIGHT_TRACE");
    readonly int hashLeftTrace = Animator.StringToHash("LEFT_TRACE");
    readonly int hashRightAttack = Animator.StringToHash("RIGHT_ATTACK");
    readonly int hashLeftAttack = Animator.StringToHash("LEFT_ATTACK");
    readonly int hashRightSkill = Animator.StringToHash("RIGHT_SKILL");
    readonly int hashLeftSkill = Animator.StringToHash("LEFT_SKILL");
    readonly int hashReturn = Animator.StringToHash("RETURN");
    readonly int hashAwake = Animator.StringToHash("AWAKE");
    readonly int hashDeath = Animator.StringToHash("DEATH");


    void Awake()
    {
        var playerobj = GameObject.FindGameObjectWithTag("Player");
        if (playerobj != null)
        {
            player = playerobj.GetComponent<Transform>();
        }
        anim = GetComponent<Animator>();
        defaultPos = gameObject.transform.position;
        isDie = false;
        isAware = false;
        isAttack = false;
        isUseSkill = false;
        isTracing = false;
        attackDelayTimer = attackDelay;
    }


    void OnEnable()
    {
        hp = 10;
        isDie = false;
        isAware = false;
        isAttack = false;
        isUseSkill = false;
        state = State.IDLE;
        StartCoroutine(setState());
        StartCoroutine(Animation());
        StartCoroutine(Fly());
    }

    private void Update()
    {
        if (hp == 0)
        {
            isDie = true;
        }
        Flip();
        AttackDelay();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }

    private void LateUpdate()
    {
        Move();
    }

    IEnumerator Fly()
    {
        while (true)
        {
            float y = Mathf.Sin(Time.time * 5) * 0.005f;
            transform.position += new Vector3(0, y, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }

    void Flip()
    {
        if (state == State.LEFT_TRACE)
        {
            isTracing = true;
            skill.transform.rotation = Quaternion.Euler(0, 180, 0);
            skill.startRotation3D = new Vector3(0, 0, 180 * Mathf.Deg2Rad);
            ParticleSystem[] subs = skill.GetComponentsInChildren<ParticleSystem>();
            for (int b = 1; b < subs.Length; b++)
            {
                subs[b].startRotation3D = new Vector3(0, 0, 180 * Mathf.Deg2Rad);
            }
        }
        else if (state == State.RIGHT_TRACE)
        {
            isTracing = true;
            skill.transform.rotation = Quaternion.Euler(0, 0, 0);
            skill.startRotation3D = new Vector3(0, 0 , 0);
            ParticleSystem[] subs = skill.GetComponentsInChildren<ParticleSystem>();
            for (int b = 1; b < subs.Length; b++)
            {
                subs[b].startRotation3D = new Vector3(0, 0, 0);
            }
        }
    }

    void AttackDelay()
    {
        if (isAttack || isUseSkill)
        {
            attackDelayTimer -= Time.deltaTime;
        }
        if (attackDelayTimer <= 0)
        {
            isAttack = false;
            isUseSkill = false;
        }
    }

    void Move()
    {
        Vector3 dir = (player.position - transform.position);

        stopMove = Physics2D.OverlapCircle(transform.position, 0.3f, Wall);

        if (isTracing && !stopMove && !usingSkill)
        {
            if (transform.position.x < player.position.x)
                transform.position += (dir - traceOffset).normalized * traceSpeed * Time.deltaTime;
            else if (transform.position.x > player.position.x)
                transform.position += (dir + traceOffset).normalized * traceSpeed * Time.deltaTime;
        }
        else if (isReturning)
            transform.position = Vector3.Lerp(transform.position, defaultPos, 0.02f);
    }

    IEnumerator setState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            if (state == State.DEATH)
            {
                yield break;
            }

            float dist = Vector3.Distance(player.position, transform.position);


            if (isTracing && (stopMove || dist > traceDist))
            {
                state = State.RETURN;
                isAware = false;
                isAttack = false;
                isUseSkill = false;
                isTracing = false;
                attackDelayTimer = attackDelay;
                yield return new WaitForSeconds(0.6f);
                isReturning = true;
            }
            else if (isReturning && Vector3.Distance(transform.position, defaultPos) < 0.5f)
            {
                state = State.AWAKE;
                isReturning = false;
            }
            else if (state == State.AWAKE)
            {
                state = State.IDLE;
            }
            else if (dist <= attackDist  && !isAttack && !isUseSkill && !isReturning && attackDelayTimer > 0)
            {
                int random = Random.Range(0, 201);
                if (random < 200)
                {
                    isTracing = false;
                    isAttack = true;
                    if (state==State.LEFT_TRACE) //??????????????? ????????? ?????? ???
                    {
                        Attack.GetComponent<SpriteRenderer>().flipX = true;
                        Attack.transform.localPosition = new Vector3(-0.265f, -0.201f, 0.001f);
                        state = State.LEFT_ATTACK;
                    }
                    else if (state==State.RIGHT_TRACE)   //??????????????? ???????????? ?????? ???
                    { 
                        Attack.GetComponent<SpriteRenderer>().flipX = false;
                        Attack.transform.localPosition = new Vector3(0.321f, -0.201f, 0.001f);
                        state = State.RIGHT_ATTACK;
                    }
                }
                else if (random >= 100)
                {
                    isTracing = false;
                    isUseSkill = true;
                    if (transform.position.x > player.position.x) //??????????????? ????????? ?????? ???
                        state = State.LEFT_SKILL;
                    else if (transform.position.x < player.position.x)   //??????????????? ???????????? ?????? ???
                        state = State.RIGHT_SKILL;
                }
            }
            else if (dist <= traceDist &&!isReturning)
            {
                if (!isAware)
                {
                    state = State.AWARE;
                    yield return new WaitForSeconds(0.4f);
                    isAware = true;
                    isTracing = false;
                }
                else if (isAware)
                {
                    if (transform.position.x > player.position.x)    //??????????????? ????????? ?????? ???
                    {
                        if (state == State.AWARE||state == State.RIGHT_TRACE|| state == State.RIGHT_ATTACK || state == State.RIGHT_SKILL)
                            state = State.LEFT_TURN;
                        else if (state == State.LEFT_TURN|| state == State.LEFT_ATTACK || state == State.LEFT_SKILL)
                            state = State.LEFT_TRACE;

                        isTracing = true;
                    }
                    else if (transform.position.x < player.position.x)   //??????????????? ???????????? ?????? ???
                    {
                        if (state == State.LEFT_TRACE|| state == State.LEFT_ATTACK || state == State.LEFT_SKILL)
                            state = State.RIGHT_TURN;
                        else if (state == State.AWARE||state == State.RIGHT_TURN||state == State.RIGHT_ATTACK || state == State.RIGHT_SKILL)
                            state = State.RIGHT_TRACE;

                        isTracing = true;
                    }

                    if (attackDelayTimer <= 0)
                        attackDelayTimer = attackDelay;
                }
            }
            else if (dist > attackDist && dist <= traceDist)
            {
                isAttack = false;
                isUseSkill = false;
                attackDelayTimer = attackDelay;
            }
            else if (isDie)
            {
                state = State.DEATH;
                isAware = false;
                isAttack = false;
                isUseSkill = false;
                isTracing = false;
                isReturning = false;
            }

        }
    }

    IEnumerator Animation()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            switch (state)
            {
                case State.IDLE:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.AWARE:
                    anim.SetBool(hashAware, true);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.RIGHT_TURN:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, true);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.LEFT_TURN:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, true);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.RIGHT_TRACE:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, true);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.LEFT_TRACE:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, true);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.RIGHT_ATTACK:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, true);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.LEFT_ATTACK:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, true);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.LEFT_SKILL:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, true);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.RIGHT_SKILL:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, true);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.RETURN:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, true);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashDeath, false);
                    break;

                case State.AWAKE:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashAwake, true);
                    anim.SetBool(hashDeath, false);
                    break;


                case State.DEATH:
                    anim.SetBool(hashAware, false);
                    anim.SetBool(hashRightTurn, false);
                    anim.SetBool(hashLeftTurn, false);
                    anim.SetBool(hashRightTrace, false);
                    anim.SetBool(hashLeftTrace, false);
                    anim.SetBool(hashRightAttack, false);
                    anim.SetBool(hashLeftAttack, false);
                    anim.SetBool(hashRightSkill, false);
                    anim.SetBool(hashLeftSkill, false);
                    anim.SetBool(hashAwake, false);
                    anim.SetBool(hashReturn, false);
                    anim.SetBool(hashDeath, true);
                    break;
            }
        }
    }

    public void ableAttack()
    {
        Attack.gameObject.SetActive(true);
    }

    public void disableAttack()
    {
        Attack.gameObject.SetActive(false);
    }

    void ShootSkill()
    {
        skill.Play();
    }

    void usingSkillTrue()
    {
        usingSkill = true;
    }

    void usingSkillFalse()
    {
        usingSkill = false;
    }
}
