using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonM : MonoBehaviour
{
    public GameObject mantis;

    public void OnBossRushPull()
    {
        mantis.GetComponent<Boss>().RushPull();
    }

    public void OnBossRushPull_L()
    {
        mantis.GetComponent<Boss>().RushPull_L();
    }

    public void OnBossFlyingPull()
    {
        mantis.GetComponent<Boss>().BossFlyingPull();
    }

    public void OnBossWallPull()
    {
        mantis.GetComponent<Boss>().BossWallPull();
    }

    public void OnBossWallPull_L()
    {
        mantis.GetComponent<Boss>().BossWallPull_L();
    }
    public void OnBossDiePull()
    {
        mantis.GetComponent<Boss>().bossHP = 0;
        mantis.GetComponent<Boss>().DiePull();
    }

    public void OnBossLoserPull()
    {
        mantis.GetComponent<Boss>().LoserPull();
    }

    public void OnBossDamage()
    {
        mantis.GetComponent<Boss>().Damage();
    }

    public void OnBossDamage7()
    {
        mantis.GetComponent<Boss>().Damage7();
    }

    


}
