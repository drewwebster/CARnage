using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour {

    
    List<GameObject> buildingParts;
    int partCountInitially;

    private void Start()
    {
        buildingParts = new List<GameObject>();
        foreach(Transform trans in transform.GetComponentInChildren<Transform>())
        {
            buildingParts.Add(trans.gameObject);
        }
        partCountInitially = buildingParts.Count;
    }

    public List<GameObject> getBuildingParts()
    {
        return buildingParts;
    }

    public float getPercentage()
    {
        return (float)100 * buildingParts.Count/ partCountInitially;
    }

    public void removePart(GameObject part)
    {
        buildingParts.Remove(part);
        transform.parent.GetComponentInChildren<Text>().text = (int)getPercentage() + "%";
    }
}
