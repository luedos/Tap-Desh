using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_EQS_Query : MonoBehaviour {

    public virtual Transform GetTransform()
    {
        return transform;
    }
    
    public virtual Vector3 GetPoint()
    {
        return transform.position;
    }
}
