using UnityEngine;

public class HandDebugCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hand collided with: " + collision.collider.gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hand triggered with: " + other.gameObject.name);
    }
}
