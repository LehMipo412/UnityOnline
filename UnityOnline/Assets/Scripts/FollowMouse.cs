
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //  transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) );//* Time.deltaTime
        transform.eulerAngles = Input.mousePosition;

        if (transform.eulerAngles.x > 180)
        {
            transform.eulerAngles = new Vector3(180f, transform.eulerAngles.y, 0);
        }
        if (transform.eulerAngles.y > 180)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180f, 0);
        }
        if (transform.eulerAngles.x < 0)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        if (transform.eulerAngles.y < 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 0);
        }
    }
}
