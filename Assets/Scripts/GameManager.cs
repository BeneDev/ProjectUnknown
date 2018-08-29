using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    [SerializeField] Transform bulletParent;
    [SerializeField] int bulletInstantiationCount = 100;

    [SerializeField] GameObject rifleBullet;
    Stack<GameObject> freeRifleBullets = new Stack<GameObject>();

    [SerializeField] GameObject heavyBullet;
    Stack<GameObject> freeHeavyBullets = new Stack<GameObject>();

    [SerializeField] GameObject sniperBullet;
    Stack<GameObject> freeSniperBullets = new Stack<GameObject>();

    [SerializeField] Transform particleSystemParent;
    [SerializeField] int maxParticles = 30;

    [SerializeField] GameObject bulletImpact;
    Stack<GameObject> freeBulletImpacts = new Stack<GameObject>();

    [SerializeField] GameObject dustWave;
    Stack<GameObject> freeDustWaves = new Stack<GameObject>();

    [SerializeField] GameObject critImpact;
    Stack<GameObject> freeCritImpacts = new Stack<GameObject>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < bulletInstantiationCount; i++)
        {
            // Instantiate Rifle Bullet
            GameObject newRifleBullet = Instantiate(rifleBullet, Vector3.zero, Quaternion.Euler(Vector3.zero), bulletParent);
            newRifleBullet.SetActive(false);
            freeRifleBullets.Push(newRifleBullet);

            // Instantiate Shotgun Bullet
            GameObject newShotgunBullet = Instantiate(heavyBullet, Vector3.zero, Quaternion.Euler(Vector3.zero), bulletParent);
            newShotgunBullet.SetActive(false);
            freeHeavyBullets.Push(newShotgunBullet);

            // Instantiate Sniper Bullet
            GameObject newSniperBullet = Instantiate(sniperBullet, Vector3.zero, Quaternion.Euler(Vector3.zero), bulletParent);
            newSniperBullet.SetActive(false);
            freeSniperBullets.Push(newSniperBullet);
        }
        for (int i = 0; i < maxParticles; i++)
        {
            GameObject newBulletImpact = Instantiate(bulletImpact, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            freeBulletImpacts.Push(newBulletImpact);

            GameObject newDustWave = Instantiate(dustWave, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            freeDustWaves.Push(newDustWave);

            GameObject newCritImpact = Instantiate(critImpact, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            freeCritImpacts.Push(newCritImpact);
        }
    }

    public void GetBulletImpact(Vector3 pos, Vector3 upDir, int damage)
    {
        GameObject ps = freeBulletImpacts.Pop();
        if(damage <= 10)
        {
            ps.transform.localScale = new Vector3(0.9f + (damage * 0.1f), 0.9f + (damage * 0.1f), 0.9f + (damage * 0.1f));
        }
        else
        {
            ps.transform.localScale = new Vector3(2f, 2f, 2f);
        }
        ps.transform.position = pos;
        ps.transform.up = upDir;
        ps.GetComponent<ParticleSystem>().Play();
        StartCoroutine(GetParticleSystemBack(ps.GetComponent<ParticleSystem>().main, ps, freeBulletImpacts));
    }

    public void GetDustWave(Vector3 pos)
    {
        ParticleSystem ps = freeDustWaves.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.transform.position = pos;
        ps.Play();
        StartCoroutine(GetParticleSystemBack(ps.main, ps.gameObject, freeDustWaves));
    }

    public void GetCritImpact(Vector3 pos)
    {
        ParticleSystem ps = freeCritImpacts.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.transform.position = pos;
        ps.Play();
        StartCoroutine(GetParticleSystemBack(ps.main, ps.gameObject, freeCritImpacts));
    }

    IEnumerator GetParticleSystemBack(ParticleSystem.MainModule main, GameObject ps, Stack<GameObject> stackToPush)
    {
        yield return new WaitForSeconds(main.duration);
        stackToPush.Push(ps);
    }

    public GameObject GetRifleBullet(Vector3 pos)
    {
        GameObject bull = freeRifleBullets.Pop();
        bull.transform.position = pos;
        bull.SetActive(true);
        StartCoroutine(GetBulletBackAfterSeconds(bull, freeRifleBullets, bull.GetComponent<BulletController>().Lifetime));
        return bull;
    }

    public GameObject GetSniperBullet(Vector3 pos)
    {
        GameObject bull = freeSniperBullets.Pop();
        bull.transform.position = pos;
        bull.SetActive(true);
        StartCoroutine(GetBulletBackAfterSeconds(bull, freeSniperBullets, bull.GetComponent<BulletController>().Lifetime));
        return bull;
    }

    public GameObject GetHeavyBullet(Vector3 pos)
    {
        GameObject bull = freeHeavyBullets.Pop();
        bull.transform.position = pos;
        bull.SetActive(true);
        StartCoroutine(GetBulletBackAfterSeconds(bull, freeHeavyBullets, bull.GetComponent<BulletController>().Lifetime));
        return bull;
    }

    IEnumerator GetBulletBackAfterSeconds(GameObject bull, Stack<GameObject> stack, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        stack.Push(bull);
        bull.SetActive(false);
    }
}
