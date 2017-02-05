using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sand : Material
{
    void Start()
    {
        Friction = 40.0f;
        Bounciness = 1.0f;
    }
}