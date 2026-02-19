using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = -5.0f;

    void Update()
    {
        // Moves the object along its own Z-axis (forward) at a continuous speed
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}