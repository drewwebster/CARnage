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
    public int projectilesPerShot = 1;
    public float projectileScatterAngle = 0;
    public float projectileSpeedRNDRange = 0;
    public bool automatic;
    public float timeDelayAfterShooting = 0f;

    public float projectileSpeed = 1000;
    public float knockbackMult = 1f;
    
    public WeaponState weaponState = WeaponState.COLLECTABLE;
    public bool isRoofWeapon;
    public bool isFrontWeapon;
    public bool isSprayWeapon;
    public GameObject Projectile;
    public GameObject meleeHitbox;
    public GameObject Projectile_Bulletcase;
    public GameObject Magazine;
    public GameObject shootFX;
    public GameObject shootFXinsta;
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
    public GameObject loadedProjectileGO;

    public GameObject upgrade_MagazineGO;
    public GameObject upgrade_ScopeGO;
    public GameObject upgrade_SilencerGO;
    public GameObject upgrade_AutomaticGO;
    public GameObject upgrade_DamageGO;
    public GameObject upgrade_LightGO;

    GameObject throwable;
    public GameObject[] throwableVariants;
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
    
    public int magazineLoaded;

    public enum WeaponState
    {
        EQUIPPED_LEFT,
        EQUIPPED_RIGHT,
        STASHED,
        COLLECTABLE,
        AUTOFIRE
    }
    
    public void onObtained()
    {
        if (!GetComponent<Rigidbody>())
            return;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
        collectableFX.SetActive(false);

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
        if (CARnageAuxiliary.isPaused)
            return;

        if (weaponState == WeaponState.STASHED)
            return;

        if (getCar() != null && getCar().isLocked())
            return;

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
        bool firingRN = false;
        if (weaponState == WeaponState.AUTOFIRE || rel_car.GetComponent<CARnageCar>().controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard && ((weaponState == WeaponState.EQUIPPED_LEFT && leftFiring) || (weaponState == WeaponState.EQUIPPED_RIGHT && rightFiring)))
        {
            firingRN = true;
            switch (damageType)
            {
                case DamageType.PROJECTILE:
                    Invoke("shoot", timeDelayAfterShooting);
                    break;
                case DamageType.MELEE:
                    swingMe();
                    break;
                case DamageType.EXPLOSION:
                    gatherThrowForce();
                    break;
            }
        }

        // SPRAY WEAPONS:
        if (isSprayWeapon)
            if (firingRN)
            {
                //meleeHitbox.SetActive(true);
                if(shootFXinsta == null)
                {
                    shootFXinsta = Instantiate(shootFX,shootFX.transform.parent);
                    shootFXinsta.SetActive(true);
                }
                //shootFX.SetActive(true);
                //foreach(var ps in shootFX.GetComponentsInChildren<ParticleSystem>())
                //    if(!ps.isPlaying)
                //        ps.Play();                
            }
            else
            {
                if(shootFXinsta != null)
                {
                    shootFXinsta.transform.parent = null;
                    foreach (var ps in shootFXinsta.GetComponentsInChildren<ParticleSystem>())
                            ps.Stop();
                    Destroy(shootFXinsta, 3);
                    shootFXinsta = null;
                }
                //meleeHitbox.SetActive(false);
                //shootFX.SetActive(false);
                //foreach (var ps in shootFX.GetComponentsInChildren<ParticleSystem>())
                //    if(!ps.isStopped)
                //        ps.Stop();
            }

        // THROWABLE WEAPONS:
        if (isGatheringThrowForce)
            throwForceTime += Time.deltaTime;

        if (damageType == DamageType.EXPLOSION && rel_car.GetComponent<CARnageCar>().controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard && ((weaponState == WeaponState.EQUIPPED_LEFT && Input.GetMouseButtonUp(0)) || (weaponState == WeaponState.EQUIPPED_RIGHT && Input.GetMouseButtonUp(1))))
            throwMe();

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
        
        switch(weaponModel)
        {
            case WeaponModel.ESCAPE:
                CARnageAuxiliary.playAnimationTimeScaled(gameObject, "ChopTowardsFront", getModdedShotDelay());
                break;
            case WeaponModel.RELIEF:
            case WeaponModel.WRECKER:
            case WeaponModel.DRILL:
            case WeaponModel.TORCH:
            case WeaponModel.SURPRISE:
                break;
            default:
                CARnageAuxiliary.playAnimationTimeScaled(gameObject, "SwingTowardsFront", getModdedShotDelay());
                break;
        }


        onSwing();
        Invoke("onSwingEnd", getModdedShotDelay());
        Invoke("resetFiringDelay", getModdedShotDelay());
    }

    public void onSwing()
    {
        if(meleeHitbox != null)
        {
            if (automatic)
                meleeHitbox.SetActive(false); // flicker; dont know if this is necessary
            meleeHitbox.SetActive(true);
        }

        switch(weaponModel)
        {
            case WeaponModel.TOOL:
                getCar().repair(1);
                break;
        }
    }
    public void onSwingEnd()
    {
        if (meleeHitbox != null)
        {
            //GetComponent<Animator>().enabled = false;
            meleeHitbox.SetActive(false);
            if (automatic)
                meleeHitbox.SetActive(true); // flicker to re-collide
        }
    }

    public void onHit()
    {
        meleeDMGdelay = true;
        Invoke("endMeleeDMGdelay", getModdedShotDelay());
    }
    public void endMeleeDMGdelay()
    {
        meleeDMGdelay = false;
        if(automatic && meleeHitbox != null)
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

        switch(weaponModel)
        {
            case WeaponModel.MOTHER_THERESA:
                CARnageAuxiliary.playAnimationTimeScaled(gameObject, "BowShoot", getModdedShotDelay());
                break;
            default:
                CARnageAuxiliary.playAnimationTimeScaled(gameObject, "Shoot", getModdedShotDelay());
                break;
        }

        for(int i = 0; i < projectilesPerShot; i++)
        {
            ProjectileTrajectory proj = shootProjectile();
            if (loadedProjectileGO)
                loadedProjectileGO.SetActive(false);
            getCar().getModController().onProjectileShot(proj);
        }

        // Bulletcase / FX
        if (Projectile_Bulletcase != null)
        {
            GameObject goBC = Instantiate(Projectile_Bulletcase, BulletcaseSpawnGO.transform); // parent transform for intialisation
            goBC.transform.parent = null;
            Destroy(goBC, CARnageAuxiliary.destroyAfterSec);
        }

        if (shootFX != null)
            Instantiate(shootFX, shootFXSpawnGO.transform);


        Invoke("resetFiringDelay", getModdedShotDelay());
        magazineLoaded--;
    }

    public ProjectileTrajectory shootProjectile()
    {
        GameObject go = Instantiate(Projectile, BulletSpawnGO.transform); // parent transform for intialisation
        go.transform.parent = null;
        if(weaponState != WeaponState.AUTOFIRE)
            go.GetComponentInChildren<Rigidbody>().velocity = getCar().GetComponent<Rigidbody>().velocity;

        go.transform.Rotate(new Vector3(0, 0, UnityEngine.Random.Range(-projectileScatterAngle, projectileScatterAngle)));
        go.GetComponentInChildren<Rigidbody>().AddForce(go.transform.up * projectileSpeed * (1-UnityEngine.Random.Range(-projectileSpeedRNDRange, projectileSpeedRNDRange)) * getWeaponMod_projectileSpeed_multiplier());
        go.GetComponentInChildren<ProjectileTrajectory>().rel_car = getCar();
        go.GetComponentInChildren<ProjectileTrajectory>().rel_weapon = this;
        if (go.GetComponentInChildren<explodableHitbox>())
            go.GetComponentInChildren<explodableHitbox>().rel_weapon = this;
        
        Destroy(go, CARnageAuxiliary.destroyAfterSec);
        GetComponent<AudioSource>().PlayOneShot(ShootSound);
        return go.GetComponent<ProjectileTrajectory>();
    }

    bool isGatheringThrowForce = false;
    float throwForceTime = 0;
    //float throwTimeMax = 3;
    void gatherThrowForce()
    {
        isGatheringThrowForce = true;
        CARnageAuxiliary.playAnimationTimeScaled(gameObject, "Gather", reloadTime);
    }

    public void throwMe()
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
        CARnageAuxiliary.playAnimationTimeScaled(gameObject, "Gather", 0.001f);
        isGatheringThrowForce = false;

        GameObject thrown = Instantiate(throwable, throwable.transform.parent);
        throwable.SetActive(false);
        thrown.transform.parent = null;
        thrown.GetComponentInChildren<Collider>().enabled = true;
        thrown.GetComponentInChildren<explodableHitbox>().enabled = true;
        thrown.GetComponentInChildren<explodableHitbox>().rel_weapon = this;
        thrown.GetComponentInChildren<explodableHitbox>().explodeTime = shotDelay;
        thrown.AddComponent<Rigidbody>();
        if (weaponModel == WeaponModel.OUTLAW)
        {
            thrown.GetComponent<Rigidbody>().freezeRotation = true;
            thrown.GetComponentInChildren<AutoTurretFire>().GetComponentInChildren<CARnageWeapon>().rel_car = rel_car;
            thrown.GetComponentInChildren<AutoTurretFire>().GetComponentInChildren<CARnageWeapon>().weaponState = WeaponState.AUTOFIRE;
            thrown.GetComponentInChildren<AutoTurretFire>().GetComponentInChildren<CARnageWeapon>().magazineLoaded = 7777;
            thrown.GetComponentInChildren<AutoTurretFire>().gameObject.SetActive(true);

        }

        Vector3 throwForce = new Vector3(thrown.transform.up.x, thrown.transform.up.y + 0.2f, thrown.transform.up.z);
        if (isRoofWeapon)
            throwForce = new Vector3(-thrown.transform.up.x, thrown.transform.up.y + 1f, -thrown.transform.up.z);
        throwForceTime = Mathf.Min(throwForceTime, reloadTime);
        float throwMult = 1000 * throwForceTime / reloadTime;
        throwForceTime = 0;
        throwForce = throwForce * throwMult;
        throwForce.y = Mathf.Clamp(throwForce.y, 0, 250);

        thrown.GetComponent<Rigidbody>().velocity = getCar().GetComponent<Rigidbody>().velocity;
        thrown.GetComponent<Rigidbody>().AddForce(throwForce);

        thrown.GetComponent<Rigidbody>().AddTorque(transform.right * throwMult);

        //Invoke("resetFiringDelay", getModdedShotDelay());
        //Invoke("reload", getModdedShotDelay());
        resetFiringDelay();
        reload();
        magazineLoaded--;
    }

    public float calcDamage(CARnageCar damagedCar)
    {
        return Damage * getWeaponMod_damage_multiplier();
    }

    public float calcDamage(buildingCollision building)
    {
        return Damage * getWeaponMod_damage_multiplier() * buildingDMG_WeaponModelMod();
    }

    public CARnageCar getCar()
    {
        if (rel_car == null)
            rel_car = GetComponentInParent<CARnageCar>();

        return rel_car;
    }

    public void resetFiringDelay()
    {
        firing = false;
        if (magazineLoaded == 0)
            reload();
    }

    public void reload()
    {
        reloading = true;
        float reloadTimeModded = reloadTime * getCar().getModController().getWeaponReloadTime_Multiplier() * GlobalModifiers.getWeaponReloadTime_Multiplier_GLOBAL();
        Invoke("resetReloadingDelay", reloadTimeModded);

        switch (weaponModel)
        {
            case WeaponModel.MOTHER_THERESA:
                CARnageAuxiliary.playAnimationTimeScaled(gameObject, "BowReload", reloadTimeModded);
                break;
            default:
                CARnageAuxiliary.playAnimationTimeScaled(gameObject, "Reload", reloadTimeModded);
                break;
        }

        if (Magazine != null)
        {
            GameObject go = Instantiate(Magazine, transform);
            go.transform.parent = null;
            Destroy(go, CARnageAuxiliary.destroyAfterSec);
        }
        GetComponent<AudioSource>().PlayOneShot(ReloadSound);
    }
    public void resetReloadingDelay()
    {
        magazineLoaded = (int)(magazineSize * getWeaponMod_magazine_multiplier() * getCar().getModController().getWeaponMagazine_Multiplier() * GlobalModifiers.getWeaponMagazine_Multiplier_GLOBAL(this));
        reloading = false;
        if (loadedProjectileGO)
            loadedProjectileGO.SetActive(true);

        if (damageType == DamageType.EXPLOSION)
        {
            throwable = throwableVariants[UnityEngine.Random.Range(0, throwableVariants.Length)];
            throwable.SetActive(true);
            throwForceTime = 0f;
        }
    }
    
    public void calcWeaponAngle()
    {
        if (weaponState == WeaponState.AUTOFIRE || weaponModel == WeaponModel.GREED_WEAPON)
            return;
        
        if(isRoofWeapon)
        {
            transform.position = getCar().getWeaponController().RoofWeaponSpot.transform.position;
            transform.localRotation = Quaternion.identity;
            return;
        }
        if(isFrontWeapon)
        {
            transform.position = getCar().getWeaponController().FrontWeaponSpot.transform.position;
            transform.localRotation = Quaternion.identity;
            return;
        }
        
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

        if (weaponModel == WeaponModel.GLUTTONY_WEAPON) // that doesnt make any sense but ... well.
            transform.localRotation = Quaternion.identity;
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

    public void addAllUpgrades()
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

        foreach (UpgradeTypes ut in possibleUpgrades)
            addUpgrade(ut);
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
        INFERNO,
        PHOENIX,
        CROSSIE,
        SLOTTIE,
        WRATH_WEAPON,
        GLUTTONY_WEAPON,
        LUST_WEAPON,
        GREED_WEAPON,
        ENVY_WEAPON,
        DETONATION_WEAPON,
        TRAILER_THRASH_WEAPON
    }

    private void OnTriggerEnter(Collider other)
    {
        if(weaponState == WeaponState.COLLECTABLE)
        {
            CARnageCar car = other.GetComponentInParent<CARnageCar>();
            if (other.GetComponent<damageCar>()) // only bounding box of car model
            {
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
        // melee knockback effect:
        if(damageType == DamageType.MELEE)
        {
            Vector3 knockbackDirection = Vector3.zero;
            switch (weaponModel)
            {
                case WeaponModel.PUNCH:
                case WeaponModel.WHACK:
                case WeaponModel.LUST_WEAPON:
                    knockbackDirection = transform.forward * 50000;
                    break;
                case WeaponModel.SILLY_BILLY:
                case WeaponModel.GLUTTONY_WEAPON:
                    knockbackDirection = transform.forward * 25000;
                    break;
                case WeaponModel.BOOMSTICK:
                case WeaponModel.RELIEF:
                    knockbackDirection = transform.forward * 10000;
                    break;
                default:
                    if(!automatic)
                        knockbackDirection = transform.forward * 10000;
                    break;
            }
            if(knockbackDirection != Vector3.zero)
                damagedCar.GetComponent<Rigidbody>().AddForce(knockbackDirection, ForceMode.Impulse);
        }


        // additional weapon effects:
        switch (weaponModel)
        {
            case WeaponModel.BOOMSTICK:
            case WeaponModel.TORCH:
            case WeaponModel.BARREL_ROLL:
            case WeaponModel.FLOWER_POWER:
            case WeaponModel.INFERNO:
                damagedCar.applyDebuff(CARnageCar.Debuff.Fire, damager);
                break;
            case WeaponModel.PHOENIX:
                damagedCar.applyDebuff(CARnageCar.Debuff.Fire, damager, 2);
                break;
            case WeaponModel.SURPRISE:
                damagedCar.applyDebuff(CARnageCar.Debuff.Fire, damager, 0.4f);
                break;
            case WeaponModel.THE_BRIDE:
            case WeaponModel.ESE:
            case WeaponModel.GRACE:
            case WeaponModel.SAW:
            case WeaponModel.MOMS_KNIFE:
            case WeaponModel.FILTH:
            case WeaponModel.WITNESS:
            case WeaponModel.TRAILER_THRASH_WEAPON:
                damagedCar.applyDebuff(CARnageCar.Debuff.Drain, damager);
                break;
            case WeaponModel.MOTHER_THERESA:
            case WeaponModel.TOXIC_WASTE:
                damagedCar.applyDebuff(CARnageCar.Debuff.Acid, damager);
                break;
            case WeaponModel.SAUCE_PLOX:
                damagedCar.applyDebuff(CARnageCar.Debuff.Fire, damager);
                damagedCar.applyDebuff(CARnageCar.Debuff.Acid, damager);
                break;
            case WeaponModel.LORI:
                damagedCar.GetComponent<Rigidbody>().AddForce(damagedCar.transform.up * 10000, ForceMode.Impulse);
                damagedCar.GetComponent<Rigidbody>().AddTorque(transform.up * 50000,ForceMode.Impulse);
                break;
            case WeaponModel.CROSSIE:
                if (damagedCar.currentShield <= 0)
                    break;
                float vamp = Mathf.Min(damagedCar.currentShield, damagedCar.maxShield / 10f);
                damagedCar.damageMe(vamp,getCar(),DamageType.MELEE);
                getCar().replenishShield(vamp);
                break;
            case WeaponModel.SLOTTIE:
                float HPvamp = Mathf.Min(damagedCar.currentHP, damagedCar.maxHP / 10f);
                damagedCar.damageMe(HPvamp, getCar(), DamageType.DIRECT_DAMAGE);
                getCar().repair(HPvamp);
                break;
            case WeaponModel.RELIEF:
            case WeaponModel.BEACH_BUOY:
                if(damagedCar.isOnFire())
                {
                    damagedCar.fireTicks = -1;
                    damagedCar.endDebuff(CARnageCar.Debuff.Fire);
                }
                break;
        }
    }

    public void OnDMG_WeaponModelMod(CARnageCar damager, buildingCollision building)
    {
        switch(weaponModel)
        {
            case WeaponModel.ACE_OF_SPADES:
                Gear.spawnGears(1, building.getBuilding(), CARnageModifier.GearSource.ENVIRONMENT);
                break;
        }
    }

    public float buildingDMG_WeaponModelMod()
    {
        float mult = 1f;
        switch(weaponModel)
        {
            case WeaponModel.ESCAPE:
            case WeaponModel.THE_NEWS:
            case WeaponModel.DENTIST:
            case WeaponModel.KEVIN:
                mult *= 2;
                break;
        }
        return mult;
    }

    public void doParticleDMG(CARnageCar damagedCar)
    {
        if (meleeDMGdelay)
            return;
        if (damagedCar == getCar()) // no friendly fire
            return;
        if (weaponState == WeaponState.COLLECTABLE)
            return;
        if (damagedCar != null)
        {
            // damage to car
            float damage = calcDamage(damagedCar);
            damagedCar.damageMe(damage, rel_car, DamageType.MELEE);
            onHit();
            OnDMG_WeaponModelMod(rel_car, damagedCar);
        }
    }

    public string getExtraDescription()
    {
        switch(weaponModel)
        {
            case WeaponModel.BREAKFAST_CLUB:
            case WeaponModel.PANSY:
                return "Reflects Projectiles.";
            case WeaponModel.SILLY_BILLY:
                return "+100% Knock-back.";
            case WeaponModel.PUNCH:
            case WeaponModel.MERCURY:
                return "+400% Knock-back.";
            case WeaponModel.TOXIC_WASTE:
            case WeaponModel.MOTHER_THERESA:
                return "+Acid: 10<size=9>x</size> 2<size=9>DMG/s</size>, blocks repair.";
            case WeaponModel.SAW:
            case WeaponModel.FILTH:
            case WeaponModel.GRACE:
            case WeaponModel.ESE:
            case WeaponModel.THE_BRIDE:
            case WeaponModel.MOMS_KNIFE:
                return "+Leak: 5<size=9>x</size> 2<size=9>DMG/s</size>, -50% Speed.";
            case WeaponModel.TORCH:
            case WeaponModel.BOOMSTICK:
            case WeaponModel.BARREL_ROLL:
            case WeaponModel.FLOWER_POWER:
            case WeaponModel.INFERNO:
                return "+Fire: 5<size=9>x</size> 5<size=9>DMG/s</size>.";
            case WeaponModel.PHOENIX:
                return "+Fire: 10<size=9>x</size> 5<size=9>DMG/s</size>.";
            case WeaponModel.SURPRISE:
                return "+Fire: 2<size=9>x</size> 5<size=9>DMG/s</size>.";
            case WeaponModel.BEACH_BUOY:
            case WeaponModel.RELIEF:
                return "Extinguishes Fire.";
            case WeaponModel.TOOL:
                return "Repairs 1HP when swung.";
            case WeaponModel.OUTLAW:
                return "Throws sentry guns.";
            case WeaponModel.ESCAPE:
            case WeaponModel.DENTIST:
            case WeaponModel.KEVIN:
            case WeaponModel.THE_NEWS:
                return "+100% DMG on Buildings.";
            case WeaponModel.ACE_OF_SPADES:
                return "+1 Gear when hitting a building";
            case WeaponModel.LORI:
                return "Spins enemies around.";
            case WeaponModel.CROSSIE:
                return "Transfers 10% of enemy's Shield.";
            case WeaponModel.SLOTTIE:
                return "Transfers 10% of enemy's HP.";
            case WeaponModel.WRATH_WEAPON:
            case WeaponModel.FORTUNE:
            case WeaponModel.DOWNFALL:
                return "+100% Explosion DMG at detonation.";
            default:
                return "";
        }
    }
}
