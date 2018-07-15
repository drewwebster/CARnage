using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARnageWeapon : MonoBehaviour {

    public float Damage;
    public float shotDelay;
    public int magazineSize;
    public float reloadTime;
    public bool automatic;

    public float projectileSpeed = 1000;

    public WeaponSide weaponSide = WeaponSide.LEFT;
    public GameObject Projectile;
    public GameObject Projectile_Bulletcase;
    public GameObject Magazine;
    public GameObject shootFX;
    public float destroyAfterSec = 10;
    bool firing = false;
    bool reloading = false;
    int magazineLoaded;

    public AudioClip ShootSound;
    public AudioClip ReloadSound;

    public int addAngle;
    GameObject rel_car;
    GameObject rel_camera;

    public enum WeaponSide
    {
        LEFT,
        RIGHT
    }

    private void Start()
    {
        rel_car = transform.parent.parent.parent.gameObject;
        rel_camera = Camera.main.gameObject;
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
        bool leftFiring = Input.GetMouseButtonDown(0) || (automatic && Input.GetMouseButton(0));
        bool rightFiring = Input.GetMouseButtonDown(1) || (automatic && Input.GetMouseButton(1));

        if ((weaponSide == WeaponSide.LEFT && leftFiring) || (weaponSide == WeaponSide.RIGHT && rightFiring))
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
        Destroy(go, destroyAfterSec);
        Destroy(goBC, destroyAfterSec);

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
        Destroy(go, destroyAfterSec);
        GetComponent<AudioSource>().PlayOneShot(ReloadSound);
    }
    public void resetReloadingDelay()
    {
        magazineLoaded = magazineSize;
        reloading = false;
    }

    public void calcWeaponAngle()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 relativeToPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 v = mousePos - relativeToPoint;
        
        float angleRadians = Mathf.Atan2(v.y, v.x);
        var angleDegrees = angleRadians * Mathf.Rad2Deg;
        
        if (angleDegrees < 0)
            angleDegrees += 360;

        // if: camera not y-rotation-Normalized
        if(!rel_camera.transform.parent.parent.GetComponent<RCC_Camera>().lockZ)
        {
            angleDegrees += rel_car.transform.localRotation.eulerAngles.y;
            angleDegrees -= rel_camera.transform.localRotation.eulerAngles.y;
        }

        transform.localRotation = Quaternion.Euler(new Vector3(0, -angleDegrees + addAngle, 0));
    }   
}
