using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour {


    List<GameObject> buildingParts;
    public List<GameObject> fundamentalBuildingParts;
    int partCountInitially;
    public int gearsDropped;
    public bool explodesAtDestroy;
    bool destroyed;
    public CARnageCar lastDamager;

    private void Start()
    {
        buildingParts = new List<GameObject>();
        foreach (Transform trans in transform.GetComponentInChildren<Transform>())
            buildingParts.Add(trans.gameObject);
        partCountInitially = fundamentalBuildingParts.Count;
    }

    public float getPercentage()
    {
        if (partCountInitially == 0)
            return 0;
        return (float)100 * fundamentalBuildingParts.Count/ partCountInitially;
    }

    public void removePart(GameObject part)
    {
        buildingParts.Remove(part);
        fundamentalBuildingParts.Remove(part);
        //transform.parent.GetComponentInChildren<Text>().text = (int)getPercentage() + "%";
        Debug.Log(getPercentage());
    }

    public void destroyMe()
    {
        destroyed = true;
        if(explodesAtDestroy)
            Instantiate(Resources.Load<GameObject>("FX_Building_Explosion"), transform);
        foreach (GameObject part in buildingParts)
            if(part.GetComponent<buildingCollision>() && !part.GetComponent<buildingCollision>().destroyed)
                part.GetComponent<buildingCollision>().destroyMe(-1, DamageType.DIRECT_DAMAGE, lastDamager);
        Gear.spawnGears(gearsDropped, this, CARnageModifier.GearSource.ENVIRONMENT);
    }

    public void checkIfDestroyed()
    {
        if (destroyed)
            return;

        if (fundamentalBuildingParts.Count == 0)
            // Destroy entirely
            destroyMe();
    }

    public void calcAdditionalDamage(float residualForce, DamageType damageType)
    {
        if (destroyed)
            return;

        //find random building part
        //Transform[] parts = transform.parent.GetComponentsInChildren<Transform>();
        //Transform part = parts[Random.Range(0, parts.Length)];
        if (buildingParts.Count == 0)
            return;

        GameObject go = buildingParts[Random.Range(0, buildingParts.Count)];

        //Debug.Log("residual damage: " + residualForce);
        //transform.parent.GetComponent<Building>().buildingParts.Remove(go);
        go.GetComponent<buildingCollision>().damageMe(residualForce, false, damageType, null);
    }
}
