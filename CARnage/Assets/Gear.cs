using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour {

    CARnageCar rel_car;
    CARnageModifier.GearSource source;

    public static void spawnGears(int amount, CARnageCar car, CARnageModifier.GearSource source)
    {
        for(int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("GEAR"));
            go.GetComponent<Gear>().rel_car = car;
            go.GetComponent<Gear>().source = source;
            go.GetComponent<Gear>().Invoke("appear", i * 0.1f);
        }
    }

    private void appear()
    {
        gameObject.SetActive(true);
        transform.position = rel_car.transform.position + transform.up * 3;
        GetComponent<Rigidbody>().AddForce(10 * transform.up + transform.right * Random.Range(-5f, 5f) + transform.forward * Random.Range(-5f, 5f), ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(transform.up * Random.Range(-1000, 1000));
        Destroy(gameObject, CARnageAuxiliary.destroyAfterSec * 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        CARnageCar car = other.GetComponentInParent<CARnageCar>();
        if (other.GetComponent<damageCar>() && car.canCarryGears()) // only bounding box of car model
        {
            car.addGears(1,source);
            car.getModController().onGearCollected(1);
            Destroy(gameObject);
        }
    }
}
