using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Material {

    private void Start()
    {
        Friction = 0.20f;
        Bounciness = 50.0f;
    }
}
