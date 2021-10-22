using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonGunController : MonoBehaviour
{
        //変数宣言
    public enum ShootMode { AUTO, SEMIAUTO }

    public bool shootEnabled = true;

    //使うもの
    [SerializeField]
    ShootMode shootMode = ShootMode.AUTO;
    [SerializeField]
    int maxAmmo = 100;
    [SerializeField]
    int maxSupplyValue = 100;
    [SerializeField]
    int damage = 1;
    [SerializeField]
    float shootInterval = 0.1f;
    [SerializeField]
    float shootRange = 50;
    [SerializeField]
    float supplyInterval = 0.1f;
    [SerializeField]
    Vector3 muzzleFlashScale;
    [SerializeField]
    GameObject muzzleFlashPrefab;
    [SerializeField]
    GameObject hitEffectPrefab;
    [SerializeField]
    Image ammoGauge;
    [SerializeField]
    Text ammoText;
    [SerializeField]
    Image supplyGauge;
    
    bool shooting = false;
    bool supplying = false;
    int ammo = 0;
    int supplyValue = 0;
    GameObject muzzleFlash;
    GameObject hitEffect;

    public int Ammo
    {
        set
        {
            ammo = Mathf.Clamp(value, 0, maxAmmo);
            //UIの表示を操作
            //テキスト
            ammoText.text = ammo.ToString("D3");
            //ゲージ
            float scaleX = (float)ammo / maxAmmo;
            ammoGauge.rectTransform.localScale = new Vector3(scaleX, 1, 1);
        }
        get
        {
            return ammo;
        }
    }

    public int SupplyValue
    {
        set
        {
            supplyValue = Mathf.Clamp(value, 0, maxSupplyValue);

            if (SupplyValue >= maxSupplyValue)
            {
                Ammo = maxAmmo;
                supplyValue = 0;
            }

            float scaleX = (float)supplyValue / maxSupplyValue;
            supplyGauge.rectTransform.localScale = new Vector3(scaleX, 1, 1);
        }
        get
        {
            return supplyValue;
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        InitGun();
    }

    // Update is called once per frame
    void Update()
    {
        if (shootEnabled & ammo > 0 & GetInput())
        {
            StartCoroutine(ShootTimer());
        }

        if (shootEnabled)
        {
            StartCoroutine(SupplyTimer());
        }
    }

    //ボタン入力
    void InitGun()
    {
        Ammo = maxAmmo;
        SupplyValue = 0;
    }

    bool GetInput()
    {
        switch (shootMode)
        {
            case ShootMode.AUTO:
              return Input.GetMouseButton(0);

            case ShootMode.SEMIAUTO:
              return Input.GetMouseButtonDown(0);
        }

        return false;
    }

    //演出
    IEnumerator ShootTimer()
    {
        if (!shooting)
        {
            shooting = true;

            //マズルフラッシュON
            if (muzzleFlashPrefab != null)
            {
                if (muzzleFlash != null)
                {
                    muzzleFlash.SetActive(true);
                }
                else
                {
                    muzzleFlash = Instantiate(muzzleFlashPrefab, transform.position, transform.rotation);
                    muzzleFlash.transform.SetParent(gameObject.transform);
                    muzzleFlash.transform.localScale = muzzleFlashScale;
                }
            }

            Shoot();

            yield return new WaitForSeconds(shootInterval);

            //マズルフラッシュOFF
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(false);
            }

            //ヒットエフェクトOFF
            if (hitEffect != null)
            {
                if (hitEffect.activeSelf)
                {
                    hitEffect.SetActive(false);
                }
            }

            shooting = false;
        }
        else
        {
            yield return null;
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //レイを飛ばしてヒットしたオブジェクトの情報を得る
        if (Physics.Raycast(ray, out hit, shootRange))
        {
            //ヒットエフェクトON
            if (hitEffectPrefab != null)
            {
                if (hitEffect != null)
                {
                    hitEffect.transform.position = hit.point;
                    hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    hitEffect.SetActive(true);
                }
                else
                {
                    hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
                }
            }
            //ダメージ処理
        }
        Ammo--;
    }
    IEnumerator SupplyTimer()
    {
        if (!supplying)
        {
            supplying = true;
            SupplyValue++;
            yield return new WaitForSeconds(supplyInterval);
            supplying = false;
        }
    }
}
