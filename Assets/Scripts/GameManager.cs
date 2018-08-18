﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    [SerializeField] Transform bulletParent;
    [SerializeField] int bulletInstantiationCount = 100;

    [SerializeField] GameObject rifleBullet;
    Stack<GameObject> freeRifleBullets = new Stack<GameObject>();

    [SerializeField] GameObject shotgunBullet;
    Stack<GameObject> freeShotgunBullets = new Stack<GameObject>();

    [SerializeField] Transform particleSystemParent;
    [SerializeField] int maxParticles = 30;

    [SerializeField] GameObject bulletImpact;
    Stack<GameObject> freeBulletImpacts = new Stack<GameObject>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < bulletInstantiationCount; i++)
        {
            // Instantiate Rifle Bullet
            GameObject newRifleBullet = Instantiate(rifleBullet, Vector3.zero, Quaternion.Euler(Vector3.zero), bulletParent);
            newRifleBullet.SetActive(false);
            freeRifleBullets.Push(newRifleBullet);

            // Instantiate Shotung Bullet
            GameObject newShotgunBullet = Instantiate(shotgunBullet, Vector3.zero, Quaternion.Euler(Vector3.zero), bulletParent);
            newShotgunBullet.SetActive(false);
            freeShotgunBullets.Push(newShotgunBullet);
        }
        for (int i = 0; i < maxParticles; i++)
        {
            GameObject newBulletImpact = Instantiate(bulletImpact, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            freeBulletImpacts.Push(newBulletImpact);
        }
    }

    public void GetBulletImpact(Vector3 pos, Vector3 upDir)
    {
        GameObject ps = freeBulletImpacts.Pop();
        ps.transform.position = pos;
        ps.transform.up = upDir;
        ps.GetComponent<ParticleSystem>().Play();
        StartCoroutine(GetBulletImpactBack(ps.GetComponent<ParticleSystem>().main, ps));
    }

    IEnumerator GetBulletImpactBack(ParticleSystem.MainModule main, GameObject ps)
    {
        yield return new WaitForSeconds(main.duration);
        freeBulletImpacts.Push(ps);
    }

    public GameObject GetRifleBullet(Vector3 pos)
    {
        GameObject bull = freeRifleBullets.Pop();
        bull.transform.position = pos;
        bull.SetActive(true);
        StartCoroutine(GetBulletBackAfterSeconds(bull, freeRifleBullets, bull.GetComponent<BulletController>().Lifetime));
        return bull;
    }

    public GameObject GetShotgunBullet(Vector3 pos)
    {
        GameObject bull = freeShotgunBullets.Pop();
        bull.transform.position = pos;
        bull.SetActive(true);
        StartCoroutine(GetBulletBackAfterSeconds(bull, freeShotgunBullets, bull.GetComponent<BulletController>().Lifetime));
        return bull;
    }

    IEnumerator GetBulletBackAfterSeconds(GameObject bull, Stack<GameObject> stack, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        stack.Push(bull);
        bull.SetActive(false);
    }
}