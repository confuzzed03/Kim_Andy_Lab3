using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float turnSpeed = 70f;

    public float moveSpeed = 2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        float radius = GameObject.Find("Cylinder").GetComponent<CapsuleCollider>().radius + 0.32f;

        Vector3 cylinderPos = GameObject.Find("Cylinder").transform.position;

        float distance = Mathf.Pow(transform.position.x - cylinderPos.x, 2f) + Mathf.Pow(transform.position.z - cylinderPos.z, 2f);

        if (distance < Mathf.Pow(radius, 2f))
        {
            float newPosX = cylinderPos.x + radius * (transform.localPosition.x - cylinderPos.x) / distance;
            float newPosZ = cylinderPos.z + radius * (transform.localPosition.z - cylinderPos.z) / distance;
            transform.position = new Vector3(newPosX, transform.localPosition.y, newPosZ);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down * turnSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
        }

	}
}
