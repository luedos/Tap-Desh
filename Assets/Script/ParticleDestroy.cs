﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour {

    public float TimeForDestroy = 1f;

	// Use this for initialization
	void Start () {
        if(TimeForDestroy != 0f)
        Destroy(gameObject, TimeForDestroy);
	}

}
