using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    PlayerController player;

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

    [SerializeField] GameObject bulletTrail;
    Stack<GameObject> freeBulletTrails= new Stack<GameObject>();

    [SerializeField] GameObject muzzleFlash;
    Stack<GameObject> freeMuzzleFlashs = new Stack<GameObject>();

    [SerializeField] GameObject itemLight;
    Stack<GameObject> freeItemLights = new Stack<GameObject>();

    Vector3 respawnPosition;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.OnCheckpointSet += SetRespawnPosition;
        player.OnPlayerDied += RespawnPlayer;
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

            GameObject newMuzzleFlash = Instantiate(muzzleFlash, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            newMuzzleFlash.SetActive(false);
            freeMuzzleFlashs.Push(newMuzzleFlash);
        }
        for (int i = 0; i < maxParticles; i++)
        {
            GameObject newBulletImpact = Instantiate(bulletImpact, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            newBulletImpact.SetActive(false);
            freeBulletImpacts.Push(newBulletImpact);

            GameObject newDustWave = Instantiate(dustWave, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            newDustWave.SetActive(false);
            freeDustWaves.Push(newDustWave);

            GameObject newCritImpact = Instantiate(critImpact, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            newCritImpact.SetActive(false);
            freeCritImpacts.Push(newCritImpact);

            GameObject newBulletTrail = Instantiate(bulletTrail, Vector3.zero, Quaternion.Euler(Vector3.zero), particleSystemParent);
            newBulletTrail.SetActive(false);
            freeBulletTrails.Push(newBulletTrail);

            GameObject newItemLight = Instantiate(itemLight, Vector3.zero, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), particleSystemParent);
            newItemLight.SetActive(false);
            freeItemLights.Push(newItemLight);
        }
    }

    void SetRespawnPosition(Vector3 pos)
    {
        respawnPosition = pos;
    }

    void RespawnPlayer()
    {
        player.gameObject.transform.position = respawnPosition;
        // TODO play animation and/or particle system and shit
    }

    public void GetBulletImpact(Vector3 pos, Vector3 upDir, int damage)
    {
        GameObject ps = freeBulletImpacts.Pop();
        ps.SetActive(true);
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

    public void GetBulletTrail(GameObject followObject)
    {
        ParticleSystem ps = freeBulletTrails.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.SetActive(true);
        ps.Play();
        StartCoroutine(GetFollowingParticleSystemBack(ps.main, ps.gameObject, freeBulletTrails, followObject));
    }

    public GameObject GetItemLight(Vector3 pos)
    {
        ParticleSystem ps = freeItemLights.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.transform.position = new Vector3(pos.x, pos.y + 1f, -1f);
        ps.gameObject.SetActive(true);
        ps.Play();
        return ps.gameObject;
    }

    public void GiveItemLightBack(GameObject ps)
    {
        ps.SetActive(false);
        freeItemLights.Push(ps);
    }

    public void GetDustWave(Vector3 pos)
    {
        ParticleSystem ps = freeDustWaves.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.SetActive(true);
        ps.gameObject.transform.position = pos;
        ps.Play();
        StartCoroutine(GetParticleSystemBack(ps.main, ps.gameObject, freeDustWaves));
    }

    public void GetCritImpact(Vector3 pos)
    {
        ParticleSystem ps = freeCritImpacts.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.SetActive(true);
        ps.gameObject.transform.position = pos;
        ps.Play();
        StartCoroutine(GetParticleSystemBack(ps.main, ps.gameObject, freeCritImpacts));
    }

    IEnumerator GetFollowingParticleSystemBack(ParticleSystem.MainModule main, GameObject ps, Stack<GameObject> stackToPush, GameObject objectToFollow)
    {
        for (float t = 0f; t < main.duration; t += Time.deltaTime)
        {
            ps.transform.position = objectToFollow.transform.position;
            yield return new WaitForEndOfFrame();
        }
        ps.SetActive(false);
        stackToPush.Push(ps);
    }

    public void GetMuzzleFlash(Vector3 pos)
    {
        ParticleSystem ps = freeMuzzleFlashs.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.SetActive(true);
        ps.gameObject.transform.position = pos;
        ps.Play();
        StartCoroutine(GetParticleSystemBack(ps.main, ps.gameObject, freeMuzzleFlashs));
    }

    IEnumerator GetParticleSystemBack(ParticleSystem.MainModule main, GameObject ps, Stack<GameObject> stackToPush, bool staysActive = false)
    {
        yield return new WaitForSeconds(main.duration);
        ps.SetActive(staysActive);
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
