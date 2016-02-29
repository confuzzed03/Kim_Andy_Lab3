using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float swing_angle;

    public float elevate_angle;

    public float rotationSpeed = 50.0F;

    private bool e = false;
    private bool f = false;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        //transform.localRotation = Quaternion.identity;

        if (Input.GetMouseButton(0))
        {
            swing_angle = Input.GetAxis("Mouse X");
            elevate_angle = Input.GetAxis("Mouse Y") * -1;
            transform.Rotate(elevate_angle, swing_angle, 0.0f);
            if (swing_angle > 0)
            {
                e = true;
            }
            if (elevate_angle > 0)
            {
                f = true;
            }
        }
        else
        {
            transform.localRotation = Quaternion.identity;
            //bool xNearZero = transform.eulerAngles.x <= 0.5f && transform.eulerAngles.x >= -0.5f;
            //bool yNearZero = transform.eulerAngles.y <= 0.5f && transform.eulerAngles.y >= -0.5f;
            //if (xNearZero)
            //{
            //    transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //}
            //if (yNearZero)
            //{
            //    transform.localEulerAngles = new Vector3(transform.eulerAngles.x, 0, 0);
            //}
            //else
            //{
            //    float x = transform.eulerAngles.x;
            //    float y = transform.eulerAngles.y;
            //    float c = 0.1f;
            //    float d = 0.1f;
            //    if (e)
            //    {
            //        d = d * -1f;
            //    }
            //    if (f)
            //    {
            //        c = c * -1f;
            //    }
            //    transform.Rotate(c, d, 0);
            //}
        }
	}
}
