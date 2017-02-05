using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Material : MonoBehaviour {
    protected float Friction;
    protected float Bounciness;
	
    public float getFriction() { 
        return Friction;
    }
    public float getBounciness() { 
        return Bounciness;
    }
}
