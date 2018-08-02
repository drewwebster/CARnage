using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARnageWeapon : MonoBehaviour {

    public string weaponName;
    public WeaponModel weaponModel;
    public DamageType damageType;
    public float Damage;
    public float shotDelay;
    [HideInInspector]
    public bool meleeDMGdelay;
    public int magazineSize;
    public float reloadTime;
    public bool automatic;

    public float projectileSpeed = 1000;
    
    public WeaponState weaponState = WeaponState.COLLECTABLE;
    public GameObject Projectile;
    public GameObject meleeHitbox;
    public GameObject Projectile_Bulletcase;
    public GameObject Magazine;
    public GameObject shootFX;
    public GameObject collectableFX;
    bool firing = false;
    bool reloading = false;

    public AudioClip ShootSound;
    public AudioClip ReloadSound;

    public GameObject WeaponGameObject;
    public GameObject WeaponModelGameObject;
    public GameObject WeaponModelWhenCollectableGameObject;
    public GameObject BulletSpawnGO;
    public GameObject BulletcaseSpawnGO;
    public GameObject shootFXSpawnGO;

    public GameObject upgrade_MagazineGO;
    public GameObject upgrade_ScopeGO;
    public GameObject upgrade_SilencerGO;
    public GameObject upgrade_AutomaticGO;
    public GameObject upgrade_DamageGO;
    public GameObject upgrade_LightGO;
    [HideInInspector]
    public bool upgraded_magazine;
    [HideInInspector]
    public bool upgraded_scope;
    [HideInInspector]
    public bool upgraded_silencer;
    [HideInInspector]
    public bool upgraded_automatic;
    [HideInInspector]
    public bool upgraded_damage;
    [HideInInspector]
    public bool upgraded_light;

    [HideInInspector]
    public int addAngle = 90;
    CARnageCar rel_car;
    GameObject rel_camera;
    int magazineLoaded;

    public enum WeaponState
    {
        EQUIPPED_LEFT,
        EQUIPPED_RIGHT,
        STASHED,
        COLLECTABLE
    }
    
    public void onObtained()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;

        rel_car = GetComponentInParent<CARnageCar>();
        //rel_camera = Camera.main.gameObject;
        rel_camera = rel_car.GetComponent<CARnageCar>().observingCamera;
        resetReloadingDelay();
        if (upgraded_magazine)
            upgrade_MagazineGO.SetActive(true);
        if (upgraded_scope)
            upgrade_ScopeGO.SetActive(true);
        if (upgraded_silencer)
            upgrade_SilencerGO.SetActive(true);
        if (upgraded_automatic)
            upgrade_AutomaticGO.SetActive(true);
        if (upgraded_damage)
            upgrade_DamageGO.SetActive(true);
        if (upgraded_light)
            upgrade_LightGO.SetActive(true);
        if(WeaponModelWhenCollectableGameObject != null)
        {
            WeaponModelWhenCollectableGameObject.SetActive(false);
            WeaponModelGameObject.SetActive(true);
        }
    }

    // left:
    float minAngleL = 90;
    float maxAngleL = 270;
    float minAngleR = -90;
    float maxAngleR = 90;
    // Update is called once per frame
    void Update ()
    {
        if(weaponState == WeaponState.COLLECTABLE)
        {
            transform.Rotate(0, 100 * Time.deltaTime, 0);
            collectableFX.SetActive(true);
            if (WeaponModelWhenCollectableGameObject != null)
            {
                WeaponModelWhenCollectableGameObject.SetActive(true);
                WeaponModelGameObject.SetActive(false);
            }
            return;
        }
        bool leftFiring = Input.GetMouseButtonDown(0) || ((automatic || getWeaponMod_automatic()) && Input.GetMouseButton(0));
        bool rightFiring = Input.GetMouseButtonDown(1) || ((automatic || getWeaponMod_automatic()) && Input.GetMouseButton(1));

        if (rel_car.GetComponent<CARnageCar>().controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard && ((weaponState == WeaponState.EQUIPPED_LEFT && leftFiring) || (weaponState == WeaponState.EQUIPPED_RIGHT && rightFiring)))
        {
            switch(damageType)
            {
                case DamageType.PROJECTILE:
                    shoot();
                    break;
                case DamageType.MELEE:
                    swingMe();
                    break;
            }
        }

        calcWeaponAngle();
    }

    public void swingMe()
    {
        if (firing)
            return;

        firing = true;
        //GetComponent<Animator>().enabled = true;
        //transform.localRotation = Quaternion.Euler(0, -180, 0);
        //if(weaponState == WeaponState.EQUIPPED_LEFT)
        //    CARnageAuxiliary.playAnimationTimeScaled(gameObject, "SwingLeft", getModdedShotDelay());
        //if(weaponState == WeaponState.EQUIPPED_RIGHT)
        //    CARnageAuxiliary.playAnimationTimeScaled(gameObject, "SwingRight", getModdedShotDelay());
        
        CARnageAuxiliary.playAnimationTimeScaled(gameObject, "SwingTowardsFront", getModdedShotDelay());

        onSwing();
        Invoke("onSwingEnd", getModdedShotDelay());
        Invoke("resetFiringDelay", getModdedShotDelay());
    }

    public void onSwing()
    {
        if (automatic)
            meleeHitbox.SetActive(false); // flicker; dont know if this is necessary
        meleeHitbox.SetActive(true);
    }
    public void onSwingEnd()
    {
        //GetComponent<Animator>().enabled = false;
        meleeHitbox.SetActive(false);
        if (automatic)
            meleeHitbox.SetActive(true); // flicker to re-collide
    }

    public void onHit()
    {
        meleeDMGdelay = true;
        Invoke("endMeleeDMGdelay", getModdedShotDelay());
    }
    public void endMeleeDMGdelay()
    {
        meleeDMGdelay = false;
        if(automatic)
        {
            meleeHitbox.SetActive(false);
            meleeHitbox.SetActive(true);
        }
    }

    public float getModdedShotDelay()
    {
        return shotDelay * getWeaponMod_shotDelay_multiplier() * getCar().getModController().getShotDelay_Multiplier();
    }

    public void shoot()
    {
        if (firing)
            return;
        if (reloading)
            return;

        if (magazineLoaded == 0)
        {
            reload();
            return;
        }

        firing = true;
        CARnageAuxiliary.playAnimationTimeScaled(gameObject, "Shoot", getModdedShotDelay());

        ProjectileTrajectory proj = shootProjectile();
        getCar().getModController().onProjectileShot(proj);

        Invoke("resetFiringDelay", getModdedShotDelay());
        magazineLoaded--;
    }

    public ProjectileTrajectory shootProjectile()
    {
        GameObject go = Instantiate(Projectile, BulletSpawnGO.transform); // parent transform for intialisation
        GameObject goBC = Instantiate(Projectile_Bulletcase, BulletcaseSpawnGO.transform); // parent transform for intialisation
        GameObject goFX = Instantiate(shootFX, shootFXSpawnGO.transform);
        go.transform.parent = null;
        goBC.transform.parent = null;
        go.GetComponent<Rigidbody>().velocity = GetComponentInParent<CARnageCar>().GetComponent<Rigidbody>().velocity;
        go.GetComponent<Rigidbody>().AddForce(transform.forward * projectileSpeed * getWeaponMod_projectileSpeed_multiplier());
        go.GetComponent<ProjectileTrajectory>().rel_car = getCar();
        go.GetComponent<ProjectileTrajectory>().rel_weapon = this;
        
        Destroy(go, CARnageAuxiliary.destroyAfterSec);
        Destroy(goBC, CARnageAuxiliary.destroyAfterSec);
        GetComponent<AudioSource>().PlayOneShot(ShootSound);
        return go.GetComponent<ProjectileTrajectory>();
    }

    public float calcDamage(CARnageCar damagedCar)
    {
        return Damage * getWeaponMod_damage_multiplier();
    }

    public float calcDamage(buildingCollision building)
    {
        return Damage * getWeaponMod_damage_multiplier();
    }

    public CARnageCar getCar()
    {
        return rel_car.GetComponent<CARnageCar>();
    }

    public void resetFiringDelay()
    {
        firing = false;
    }

    public void reload()
    {
        reloading = true;
        float reloadTimeModded = reloadTime * getCar().getModController().getWeaponReloadTime_Multiplier() * GlobalModifiers.getWeaponReloadTime_Multiplier_GLOBAL();
        Invoke("resetReloadingDelay", reloadTimeModded);
        CARnageAuxiliary.playAnimationTimeScaled(gameObject, "Reload", reloadTimeModded);

        GameObject go = Instantiate(Magazine, transform);
        go.transform.parent = null;
        Destroy(go, CARnageAuxiliary.destroyAfterSec);
        GetComponent<AudioSource>().PlayOneShot(ReloadSound);
    }
    public void resetReloadingDelay()
    {
        magazineLoaded = (int)(magazineSize * getWeaponMod_magazine_multiplier() * getCar().getModController().getWeaponMagazine_Multiplier() * GlobalModifiers.getWeaponMagazine_Multiplier_GLOBAL(this));
        reloading = false;
    }

    public void calcWeaponAngle()
    {
        if (rel_car.GetComponent<CARnageCar>().controlledBy != CARnageAuxiliary.ControllerType.MouseKeyboard)
            return;

        //if (damageType == DamageType.MELEE && GetComponentInChildren<Animator>().enabled) // dont aim for mouse when swinging
        //    return;

        Vector2 mousePos = Input.mousePosition;
        Vector2 relativeToPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        // consider splitscreen
        if(GetComponentInParent<CARnageCar>().observingCamera != null)
        {
            Rect cameraRect = GetComponentInParent<CARnageCar>().observingCamera.GetComponentInChildren<Camera>().rect;
            if(cameraRect.width == 0.5f || cameraRect.height == 0.5f)
            {
                if(cameraRect.x == 0) // left side
                {
                    if(cameraRect.y == 0) // lower side
                        relativeToPoint = new Vector2(Screen.width / 4, Screen.height / 4); // TODO: not final?
                    else // upper side
                        relativeToPoint = new Vector2(Screen.width / 4, Screen.height * 3 / 4);
                }
                else // right side
                {
                    if (cameraRect.y == 0) // lower side
                        relativeToPoint = new Vector2(Screen.width * 3 / 4, Screen.height / 4); // TODO: not final?
                    else // upper side
                        relativeToPoint = new Vector2(Screen.width * 3 / 4, Screen.height * 3/ 4);
                }
            }
            
        }


        Vector2 v = mousePos - relativeToPoint;
        
        float angleRadians = Mathf.Atan2(v.y, v.x);
        var angleDegrees = angleRadians * Mathf.Rad2Deg;
        
        if (angleDegrees < 0)
            angleDegrees += 360;

        // if: camera not y-rotation-Normalized
        if(rel_camera != null && !rel_camera.GetComponent<RCC_Camera>().lockZ)
        {
            angleDegrees += rel_car.transform.localRotation.eulerAngles.y;
            //angleDegrees -= rel_camera.transform.localRotation.eulerAngles.y;
            angleDegrees -= rel_camera.GetComponentInChildren<Camera>().transform.localRotation.eulerAngles.y;
        }

        transform.localRotation = Quaternion.Euler(new Vector3(0, -angleDegrees + addAngle, 0));
    }   

    public void addUpgrade(UpgradeTypes upgrade)
    {
        switch(upgrade)
        {
            case UpgradeTypes.AUTO:
                upgraded_automatic = true;
                upgrade_AutomaticGO.SetActive(true);
                break;
            case UpgradeTypes.COMPENSATOR:
                upgraded_damage = true;
                upgrade_DamageGO.SetActive(true);
                break;
            case UpgradeTypes.LIGHT:
                upgraded_light = true;
                upgrade_LightGO.SetActive(true);
                break;
            case UpgradeTypes.MAGAZINE:
                upgraded_magazine = true;
                upgrade_MagazineGO.SetActive(true);
                break;
            case UpgradeTypes.SCOPE:
                upgraded_scope = true;
                upgrade_ScopeGO.SetActive(true);
                break;
            case UpgradeTypes.SILENCER:
                upgraded_silencer = true;
                upgrade_SilencerGO.SetActive(true);
                break;
        }
    }

    public void addRandomUpgrade()
    {
        List<UpgradeTypes> possibleUpgrades = new List<UpgradeTypes>();
        if (upgrade_AutomaticGO != null && !upgraded_automatic)
            possibleUpgrades.Add(UpgradeTypes.AUTO);
        if (upgrade_DamageGO != null && !upgraded_damage)
            possibleUpgrades.Add(UpgradeTypes.COMPENSATOR);
        if (upgrade_LightGO != null && !upgraded_light)
            possibleUpgrades.Add(UpgradeTypes.LIGHT);
        if (upgrade_MagazineGO != null && !upgraded_magazine)
            possibleUpgrades.Add(UpgradeTypes.MAGAZINE);
        if (upgrade_ScopeGO != null && !upgraded_scope)
            possibleUpgrades.Add(UpgradeTypes.SCOPE);
        if (upgrade_SilencerGO != null && !upgraded_silencer)
            possibleUpgrades.Add(UpgradeTypes.SILENCER);
        if (possibleUpgrades.Count == 0)
            return;
        
        addUpgrade(possibleUpgrades[UnityEngine.Random.Range(0,possibleUpgrades.Count)]);
    }
    
    

    public enum UpgradeTypes
    {
        MAGAZINE,
        SCOPE,
        SILENCER,
        COMPENSATOR,
        LIGHT,
        AUTO
    }

    public enum WeaponModel
    {
        MY_LITTLE_PISTOL,
        GREAT_VENGEANCE_FURIOUS_ANGER,
        BREAKFAST_CLUB,
        CONCEPT,
        DOWNFALL,
        PUNCH,
        MERCURY,
        TOXIC_WASTE,
        PUSHER,
        PROPHET,
        BEACH_BUOY,
        COLD_CASE,
        SAWED,
        SAW,
        BIG_GAME,
        TORCH,
        DRILL,
        WHACK,
        SILLY_BILLY,
        FILTH,
        FORTUNE,
        GRACE,
        SAVIOR,
        MASS_DESTRUCTION,
        TOOL,
        OUTLAW,
        CABRON,
        ESE,
        THE_BRIDE,
        SMOKING_BARREL,
        WITNESS,
        BOOMSTICK,
        ESCAPE,
        RELIEF,
        PANSY,
        SAUCE_PLOX,
        ACE_OF_SPADES,
        FORKBUDDY,
        DENTIST,
        SURPRISE,
        KEVIN,
        MOTHER_THERESA,
        LIBERTY,
        MOMS_KNIFE,
        FEAR,
        MEANINGFUL_RELATIONSHIP,
        THE_NEWS,
        BARREL_ROLL,
        BUSTER,
        WRECKER,
        LORI,
        FLOWER_POWER,
        DRAGON,
        PHOENIX,
        CROSSIE,
        SLOTTIE
    }

    private void OnTriggerEnter(Collider other)
    {
        if(weaponState == WeaponState.COLLECTABLE)
        {
            CARnageCar car = other.GetComponentInParent<CARnageCar>();
            if (car != null)
            {
                collectableFX.SetActive(false);
                car.GetComponentInChildren<WeaponController>().obtainWeapon(gameObject);
            }
        }
    }

    public float getWeaponMod_magazine_multiplier()
    {
        if (upgraded_magazine)
            return 1.5f;
        return 1f;
    }
    public float getWeaponMod_projectileSpeed_multiplier()
    {
        if (upgraded_scope)
            return 1.5f;
        return 1f;
    }
    public float getWeaponMod_shotDelay_multiplier()
    {
        if (upgraded_silencer)
            return 0.8f;
        return 1f;
    }
    public float getWeaponMod_damage_multiplier()
    {
        if (upgraded_damage)
            return 1.2f;
        return 1f;
    }
    public bool getWeaponMod_automatic()
    {
        if (upgraded_automatic)
            return true;
        return false;
    }

    // ----- ----- ----- COMMON WEAPON BONUSES ----- ----- -----
    public void OnDMG_WeaponModelMod(CARnageCar damager, CARnageCar damagedCar)
    {
        switch (weaponModel)
        {
            case WeaponModel.BOOMSTICK:
                damagedCar.applyDebuff(CARnageCar.Debuff.Fire, damager);
                break;
        }
    }

}
