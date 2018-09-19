using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour {

    public CARnageCar rel_car;
    public CARnageCar magnetCar;
    Building rel_building;
    CARnageModifier.GearSource source;
    public bool collectable;

    public static void spawnGears(int amount, CARnageCar car, CARnageModifier.GearSource source, CARnageCar collectingCar)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("GEAR"));
            go.GetComponent<Gear>().rel_car = car;
            go.GetComponent<Gear>().magnetCar = collectingCar;
            go.GetComponent<Gear>().source = source;
            go.GetComponent<Gear>().Invoke("appear", i * 0.1f);
        }
    }
    public static void spawnGears(int amount, Building building, CARnageModifier.GearSource source, CARnageCar collectingCar)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("GEAR"));
            go.GetComponent<Gear>().rel_building = building;
            go.GetComponent<Gear>().magnetCar = collectingCar;
            go.GetComponent<Gear>().source = source;
            go.GetComponent<Gear>().Invoke("appear", i * 0.1f);
        }
    }

    private void appear()
    {
        gameObject.SetActive(true);
        if(rel_car != null)
            transform.position = rel_car.transform.position + transform.up * 3;
        else if(rel_building != null)
            transform.position = rel_building.transform.position + transform.up * 3;
        GetComponent<Rigidbody>().AddForce(10 * transform.up + transform.right * Random.Range(-5f, 5f) + transform.forward * Random.Range(-5f, 5f), ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(transform.up * Random.Range(-1000, 1000));
        Invoke("startCollectable", 2);
        Destroy(gameObject, CARnageAuxiliary.destroyAfterSec * 2);
    }

    void startCollectable()
    {
        GetComponent<SphereCollider>().enabled = true;
        collectable = true;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    CARnageCar car = collision.gameObject.GetComponentInParent<CARnageCar>();
    //    if (collision.gameObject.GetComponent<damageCar>() && car.canCarryGears()) // only bounding box of car model
    //    {
    //        car.addGears(1, source);
    //        car.getModController().onGearCollected(1);
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!collectable || magnetCar)
            return;
        // magnet
        CARnageCar car = other.GetComponentInParent<CARnageCar>();
        if (other.GetComponent<damageCar>() && car.canCarryGears()) // only bounding box of car model
        {
            magnetCar = car;
        }
    }

    private void Update()
    {
        if (magnetCar != null)
        {
            transform.LookAt(magnetCar.transform);
            float dist = Vector3.Distance(transform.position, magnetCar.transform.position);
            float mult = (15 - dist) * (15 - dist);
            GetComponent<Rigidbody>().AddForce(transform.forward * mult);
            if (dist <= 2.5f)
            {
                magnetCar.addGears(1, source);
                magnetCar.getModController().onGearCollected(1);
                Destroy(gameObject);
            }
            //else if (dist >= 15)
            //    magnetCar = null;
        }
    }
}
