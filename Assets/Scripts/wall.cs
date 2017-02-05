using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Trumped
public class wall : MonoBehaviour
{

    public Material material;

    public float Bounciness;
    public float Friction;

    private Vector3 Reflected;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.Bounciness = material.getBounciness();

    }
    private void OnCollisionEnter(Collision col)
    {
        //col.rigidbody.angularDrag = Friction;
        Reflected = Vector3.Reflect(col.relativeVelocity, col.contacts[0].normal);
        col.rigidbody.AddForce(Reflected.normalized * Bounciness * col.relativeVelocity.magnitude * 10);
    }
    void OnCollisionStay(Collision col)
    {
        //col.rigidbody.angularDrag = Friction;
    }
}
