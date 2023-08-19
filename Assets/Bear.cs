using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
    public int health = 100;
    public int speed = 10;
    public int attackSpeed = 10;
    public int attackRange = 10;
    public int attackDamage = 10;
    public int detectionRange = 15000;
    public float neckLength = 1.0f;
    public Transform neck;

    void Start() {
        // Transform neckTransform = transform.Find("spine.001");
if (neck != null)
        {
            // Scale the neck transform according to the neckLength value
            Vector3 scale = neck.localScale;
            scale.y = neckLength;
            neck.localScale = scale;
        }
        else
        {
            Debug.LogError("Neck transform not found!");
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            Debug.Log("Bear is dead!");
        }

        // Find if objects that have "player" or "deer" in the name is within the detection range of the bear

        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) <= detectionRange)
        {
            // Move towards player and turn to face player
            transform.LookAt(GameObject.Find("Player").transform);
            // only move on the x and z axis
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            transform.position += transform.forward * speed * Time.deltaTime;

        }


        // Attack player

        // Die

    }
}
