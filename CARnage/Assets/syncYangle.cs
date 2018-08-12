using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class syncYangle : MonoBehaviour {

    public GameObject target;
    public bool syncX;
    public bool syncY;
    public bool syncZ;
    public bool xTOz;

    // Update is called once per frame
    void Update () {
        float x = 0;
        float y = 0;
        float z = 0;
        if (syncX)
            x = target.transform.localRotation.eulerAngles.x;
        if (syncY)
            y = target.transform.localRotation.eulerAngles.y;
        if (syncZ)
            z = target.transform.localRotation.eulerAngles.z;
        if(xTOz)
            z = target.transform.localRotation.eulerAngles.x;
        transform.localRotation = Quaternion.Euler(x, y, z);
	}
}
