using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boss : MonoBehaviour
{
    public int bossHP = 210;

    public Animator anim;
    SpriteRenderer rend;

    Vector3 nextPos;
    float moveSpeed;

    public GameObject target;

    IEnumerator currCoroutine;
    IEnumerator dieCoroutine = null;

    public GameObject boomerang;

    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        target = GameObject.Find("Player");

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BossStageStart();
        }

        transform.Translate((nextPos - transform.position).normalized * Time.deltaTime * moveSpeed);

        if (bossHP <= 0 && dieCoroutine == null)
        {
            dieCoroutine = BossLoserDie();
            StartCoroutine(dieCoroutine);
        }

    }


    public void BossHead_Up()
    {
        anim.SetTrigger("1_Head_Up");
    } // 플레이어가 일정 거리에 들어오면 머리통을 들어올림

    public void BossStageStart()
    {
        anim.SetTrigger("2_1Stand_Start");
        StartCoroutine("BossStageDeley");
    } // 키 입력 시 의자에서 일어남

    public void BossStageStart2()
    {
        anim.SetTrigger("2_2Stand_End");
        nextPos = new Vector3(17.2f, 11.38f, 0);
        //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime*10);
        moveSpeed = 16f;
        // Start 함수>>시작지점으로 옮기고
        // nextPos = 목표 지점 
        // 애니메이션 길이에 맞게 moveSpeed 설정.
        // transform.position = Vector3.Lerp(transform.position, nextPos, Time.deltaTime*moveSpeed); >> update 함수에 적힐 부분.

        StartCoroutine(BossPattern());

    } // 몇 초있다가 뛰어 오르고 전투시작

    IEnumerator BossStageDeley()
    {
        yield return new WaitForSeconds(2f);
        BossStageStart2();
    } // 코루틴 함수로 시간 지연

    public void BossRushStart()
    {
        rend.flipX = false;
        anim.SetTrigger("3_1Rush_Start");
        //Vector3 pos = new Vector3(-5.38f, -1.87f, 0);
        transform.position = new Vector3(-5.38f, -1.87f, 0);
        nextPos = new Vector3(-4.64f, -2.94f, 0f);
        moveSpeed = 20f;
        //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10);
        //transform.position = pos;
    } // 보스 돌진 공격 시작

    public void BossRushpreparation()
    {
        anim.SetTrigger("3_2Rush_preparation");
        transform.position = new Vector3(-4.8f, -3.9f, 0);
        nextPos = transform.position;

    } // 보스 돌진 공격 준비

    public void BossRushing()
    {
        anim.SetTrigger("3_3Rushing");
        nextPos = new Vector3(3.1f, -3.9f, 0);
        moveSpeed = 20f;
    } // 보스 돌진 공격

    public void BossRushEnd1()
    {
        anim.SetTrigger("3_4RushEnd1");
        transform.position = new Vector3(3.1f, -3.9f, 0);
        nextPos = transform.position;
    } // 보스 돌진 공격 끝1

    public void BossRushEnd2()
    {
        anim.SetTrigger("3_5Rush_End2");
        transform.position = new Vector3(3.1f, -3.9f, 0);
        nextPos = new Vector3(11.5f, -0.3f, 0f);
        moveSpeed = 16f;
    } // 보스 돌진 후 날라가기

    public void BossRushStart_L()
    {
        rend.flipX = true;

        //transform.localScale = new Vector3(-1, 1, 1);

        anim.SetTrigger("3_1Rush_Start");
        transform.position = new Vector3(5.38f, -1.87f, 0);
        nextPos = new Vector3(4.64f, -2.94f, 0f);
        moveSpeed = 16f;
    }

    public void BossRushpreparation_L()
    {
        anim.SetTrigger("3_2Rush_preparation");
        transform.position = new Vector3(4.8f, -3.9f, 0);
        nextPos = transform.position;
    }

    public void BossRushing_L()
    {
        anim.SetTrigger("3_3Rushing");
        nextPos = new Vector3(-3.1f, -3.9f, 0);
        moveSpeed = 20f;
    }

    public void BossRushEnd1_L()
    {
        anim.SetTrigger("3_4RushEnd1");
        transform.position = new Vector3(-3.1f, -3.9f, 0);
        nextPos = transform.position;
    }

    public void BossRushEnd2_L()
    {
        anim.SetTrigger("3_5Rush_End2");
        transform.position = new Vector3(-3.1f, -3.9f, 0);
        nextPos = new Vector3(-11.5f, -0.3f, 0f);
        moveSpeed = 16f;
    }

    public void RushPull()
    {
        currCoroutine = BossRushPull();
        StartCoroutine(currCoroutine);
    } // 돌진 풀 

    public void RushPull_L()
    {
        currCoroutine = BossRushPull_L();
        StartCoroutine(currCoroutine);
    } // 돌진(왼) 풀

    IEnumerator BossRushPull()
    {
        rend.enabled = true;
        BossRushStart();
        yield return new WaitForSeconds(0.3f);
        BossRushpreparation();
        yield return new WaitForSeconds(0.3f);
        BossRushing();
        yield return new WaitForSeconds(0.3f);
        BossRushEnd1();
        yield return new WaitForSeconds(0.3f);
        BossRushEnd2();
        yield return new WaitForSeconds(0.3f);
    } // 돌진 풀 코루틴

    IEnumerator BossRushPull_L()
    {
        rend.enabled = true;
        BossRushStart_L();
        yield return new WaitForSeconds(0.3f);
        BossRushpreparation_L();
        yield return new WaitForSeconds(0.3f);
        BossRushing_L();
        yield return new WaitForSeconds(0.3f);
        BossRushEnd1_L();
        yield return new WaitForSeconds(0.3f);
        BossRushEnd2_L();
        yield return new WaitForSeconds(0.3f);
    } // 돌진(왼) 풀 코루틴

    public void FlyingStart()
    {
        rend.flipX = false;
        anim.SetTrigger("4_1Flying_Start");

        transform.position = new Vector3(target.transform.position.x, 2.88f, 0);
        nextPos = new Vector3(target.transform.position.x, 2.08f, 0);
        moveSpeed = 10f;
    } // 내려찍기 시작

    public void Flyingpreparation()
    {
        anim.SetTrigger("4_2Flying_preparation");

        transform.position = new Vector3(target.transform.position.x, 2.88f, 0);
        nextPos = transform.position;
    } // 내려찍기 준비

    public void Flying()
    {
        anim.SetTrigger("4_3Flying");
        nextPos = new Vector3(target.transform.position.x, -3.5f, 0);
        moveSpeed = 18f;
    } // 내려찍기

    public void FlyingEnd1()
    {
        anim.SetTrigger("4_4Flying_End1");
        transform.position = new Vector3(this.transform.position.x, -3.9f, 0);
        nextPos = transform.position;
    } // 내려찍기 끝1

    public void FlyingEnd2()
    {
        anim.SetTrigger("4_5Flying_End2");
        transform.position = new Vector3(this.transform.position.x, -3.9f, 0);
        nextPos = (new Vector3(2.33f, 1.8f, 0)) + target.transform.position;
        moveSpeed = 16f;

        currCoroutine = FlyEndDelay();
        StartCoroutine(currCoroutine);

    } // 내려찍기 끝2

    IEnumerator FlyEndDelay()
    {
        yield return new WaitForSeconds(0.3f);
        rend.enabled = false;
    } // 코루틴 함수로 시간 지연 후 보스 화면 밖으로 이탈


    public void BossFlyingPull()
    {
        currCoroutine = FlyingPull();
        StartCoroutine(currCoroutine);
    } // 보스 내려찍기 풀

    IEnumerator FlyingPull()
    {
        rend.enabled = true;
        FlyingStart();
        yield return new WaitForSeconds(0.2f);
        Flyingpreparation();
        yield return new WaitForSeconds(0.3f);
        Flying();
        yield return new WaitForSeconds(0.3f);
        FlyingEnd1();
        yield return new WaitForSeconds(0.3f);
        FlyingEnd2();
    } // Flying Pull 모음

    public void WallStart()
    {
        rend.flipX = false;
        anim.SetTrigger("5_1Wall_Start");

        transform.position = new Vector3(4.45f, -0.2f, 0);
        nextPos = new Vector3(6.36f, -0.57f, 0f);
        moveSpeed = 16f;
    } // 벽 공격 시작

    public void WallAttack()
    {
        anim.SetTrigger("5_2Wall_Attack");

        transform.position = new Vector3(7.8f, -0.3f, 0);
        nextPos = transform.position;
    } // 벽 공격 

    public void WallEnd()
    {
        anim.SetTrigger("5_3Wall_End");

        transform.position = new Vector3(7.8f, -0.3f, 0);
        nextPos = new Vector3(3.31f, 1.1f, 0);
        moveSpeed = 16f;

        currCoroutine = WallEndDeley();
        StartCoroutine(currCoroutine);
    } // 벽 공격 끝

    IEnumerator WallEndDeley()
    {
        yield return new WaitForSeconds(0.3f);
        rend.enabled = false;
    } // WallEnd 코루틴

    public void BossWallPull()
    {
        currCoroutine = WallPull();
        StartCoroutine(currCoroutine);
    } // 벽 공격 풀

    IEnumerator WallPull()
    {
        rend.enabled = true;
        WallStart();
        yield return new WaitForSeconds(0.3f);
        WallAttack();
        yield return new WaitForSeconds(2.5f);
        WallEnd();
    } // 벽공격 풀 코루틴

    public void WallStart_L()
    {
        rend.flipX = true;
        anim.SetTrigger("5_1Wall_Start");

        transform.position = new Vector3(-4.45f, -0.2f, 0);
        nextPos = new Vector3(-6.36f, -0.57f, 0f);
        moveSpeed = 16f;
    }

    public void WallAttack_L()
    {
        anim.SetTrigger("5_2Wall_Attack");

        transform.position = new Vector3(-7.8f, -0.3f, 0);
        nextPos = transform.position;
    }

    public void WallEnd_L()
    {
        anim.SetTrigger("5_3Wall_End");

        transform.position = new Vector3(-7.8f, -0.3f, 0);
        nextPos = new Vector3(-3.31f, 1.1f, 0);
        moveSpeed = 16f;

        currCoroutine = WallEndDeley();
        StartCoroutine(currCoroutine);
    }
    public void BossWallPull_L()
    {
        currCoroutine = WallPull_L();
        StartCoroutine(currCoroutine);
    } // 벽공격 풀(L)

    IEnumerator WallPull_L()
    {
        rend.enabled = true;
        WallStart_L();
        yield return new WaitForSeconds(0.3f);
        WallAttack_L();
        yield return new WaitForSeconds(2.5f);
        WallEnd_L();
    } // 벽공격 풀 코루틴(L)

    public void BossDie1()
    {

        anim.SetTrigger("6_1Dying_Start");
        //StopCoroutine(currCoroutine);

        transform.position = new Vector3(this.transform.position.x, -3.5f, 0);
        nextPos = transform.position;
    } // 보스 죽음 시작

    public void BossDie2()
    {
        anim.SetTrigger("6_2Dying_End");

        transform.position = new Vector3(this.transform.position.x, -3.5f, 0);
        nextPos = new Vector3(this.transform.position.x, -0.53f, 0);

        currCoroutine = BossDieDeley();
        StartCoroutine(currCoroutine);
    } // 보스 죽음 끝
    IEnumerator BossDieDeley()
    {
        yield return new WaitForSeconds(0.5f);
        rend.enabled = false;
    } // 보스 Die 딜레이

    public void DiePull()
    {
        StopCoroutine(currCoroutine);
        StartCoroutine("BossDiePull");
    } // 보스 죽음 풀

    IEnumerator BossDiePull()
    {
        rend.enabled = true;
        BossDie1();
        yield return new WaitForSeconds(1f);
        BossDie2();
        yield return new WaitForSeconds(1.5f);
    } // 보스죽음 풀 코루틴

    public void LoserStart()
    {
        transform.position = new Vector3(0, -0.55f, 0);
        nextPos = transform.position;

        rend.enabled = true;
        anim.SetTrigger("7_1Loser_Start");

        /*currCoroutine = BossLoserDeley();
        StartCoroutine(currCoroutine);*/
    } // 보스 패배자 시작

    public void LoserEnd()
    {
        anim.SetTrigger("7_2Loser_End");

        transform.position = new Vector3(0, -0.55f, 0);
        nextPos = transform.position;
    } // 보스 패배자 끝

    public void LoserPull()
    {
        StopCoroutine(currCoroutine);
        StartCoroutine("BossLoserPull");
    } // 보스 패배자 풀

    IEnumerator BossLoserPull()
    {
        LoserStart();
        yield return new WaitForSeconds(0.5f);
        LoserEnd();
    } // 보스 패배자 풀 코루틴

    IEnumerator BossLoserDie()
    {
        //DiePull();
        StopCoroutine(currCoroutine);
        yield return StartCoroutine(BossDiePull());
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(BossLoserPull());
        //LoserPull();

    } // 보스 다이 패배자 묶음

    public void Boomerang_Right()
    {
        float moveX = 2.5f;


        if (rend.flipX == false)
        {
            GameObject _boomerang = Instantiate(boomerang, new Vector3(transform.position.x - moveX, transform.position.y, transform.position.z), Quaternion.identity);
            _boomerang.GetComponent<Boomerang>().moveSpeed *= -1f;
        }
        else if (rend.flipX == true)
        {
            GameObject _boomerang = Instantiate(boomerang, new Vector3(transform.position.x + moveX, transform.position.y, transform.position.z), Quaternion.identity);

        }
    } //부메랑 (절대 건드리지 말 것!!)

    IEnumerator BossPattern()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            int ranAction = Random.Range(0, 5);
            switch (ranAction)
            {
                case 0:

                    currCoroutine = BossRushPull();
                    yield return StartCoroutine(currCoroutine);

                    break;
                case 1:

                    currCoroutine = BossRushPull_L();
                    yield return StartCoroutine(currCoroutine);
                    break;
                case 2:

                    currCoroutine = FlyingPull();
                    yield return StartCoroutine(currCoroutine);
                    break;
                case 3:

                    currCoroutine = WallPull();
                    yield return StartCoroutine(currCoroutine);
                    break;
                case 4:

                    currCoroutine = WallPull_L();
                    yield return StartCoroutine(currCoroutine);
                    break;
            }

            yield return new WaitForSeconds(2f);
        }

    } // 패턴모음 랜덤

    public void Damage()
    {
        bossHP = 0;
    }

    public void Damage7()
    {
        bossHP -= 10;
    }



}
