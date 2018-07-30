using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARnageWeapon : MonoBehaviour {

    public string weaponName;
    public WeaponModel weaponModel;
    public DamageType damageType;
    public float Damage;
    public float shotDelay;
    public int magazineSize;
    public float reloadTime;
    public bool automatic;

    public float projectileSpeed = 1000;
    
    public WeaponState weaponState = WeaponState.COLLECTABLE;
    public GameObject Projectile;
    public GameObject Projectile_Bulletcase;
    public GameObject Magazine;
    public GameObject shootFX;
    public GameObject collectableFX;
    bool firing = false;
    bool reloading = false;

    public AudioClip ShootSound;
    public AudioClip ReloadSound;

    public GameObject WeaponGameObject;

    public int addAngle;
    GameObject rel_car;
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
        rel_car = GetComponentInParent<CARnageCar>().gameObject;
        //rel_camera = Camera.main.gameObject;
        rel_camera = rel_car.GetComponent<CARnageCar>().observingCamera;
        magazineLoaded = magazineSize;
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
            return;
        }
        bool leftFiring = Input.GetMouseButtonDown(0) || (automatic && Input.GetMouseButton(0));
        bool rightFiring = Input.GetMouseButtonDown(1) || (automatic && Input.GetMouseButton(1));

        if (rel_car.GetComponent<CARnageCar>().controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard && ((weaponState == WeaponState.EQUIPPED_LEFT && leftFiring) || (weaponState == WeaponState.EQUIPPED_RIGHT && rightFiring)))
            shoot();

        calcWeaponAngle();
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
        CARnageAuxiliary.playAnimationTimeScaled(gameObject, "Shoot", shotDelay);

        GameObject go = Instantiate(Projectile, transform); // parent transform for intialisation
        GameObject goBC = Instantiate(Projectile_Bulletcase, transform); // parent transform for intialisation
        GameObject goFX = Instantiate(shootFX, transform);
        go.transform.parent = null;
        goBC.transform.parent = null;
        go.GetComponent<Rigidbody>().velocity = transform.parent.parent.parent.GetComponentInChildren<Rigidbody>().velocity;
        go.GetComponent<Rigidbody>().AddForce(transform.forward * projectileSpeed);
        go.GetComponent<ProjectileTrajectory>().rel_car = rel_car;
        go.GetComponent<ProjectileTrajectory>().rel_weapon = this;
        Destroy(go, CARnageAuxiliary.destroyAfterSec);
        Destroy(goBC, CARnageAuxiliary.destroyAfterSec);

        //foreach(ParticleSystem ps in shootFX.GetComponentsInChildren<ParticleSystem>())
        //{
        //    ps.Stop();
        //    ps.Play();
        //}

        Invoke("resetFiringDelay", shotDelay);
        GetComponent<AudioSource>().PlayOneShot(ShootSound);
        magazineLoaded--;
    }



    public void resetFiringDelay()
    {
        firing = false;
    }

    public void reload()
    {
        reloading = true;
        Invoke("resetReloadingDelay", reloadTime);
        CARnageAuxiliary.playAnimationTimeScaled(gameObject, "Reload", reloadTime);
        GameObject go = Instantiate(Magazine, transform);
        go.transform.parent = null;
        Destroy(go, CARnageAuxiliary.destroyAfterSec);
        GetComponent<AudioSource>().PlayOneShot(ReloadSound);
    }
    public void resetReloadingDelay()
    {
        magazineLoaded = magazineSize;
        reloading = false;
    }

    public void calcWeaponAngle()
    {
        if (rel_car.GetComponent<CARnageCar>().controlledBy != CARnageAuxiliary.ControllerType.MouseKeyboard)
            return;

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

    }

    public void addRandomUpgrade()
    {

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
}
