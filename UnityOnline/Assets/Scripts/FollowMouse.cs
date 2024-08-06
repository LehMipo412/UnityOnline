
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
   [SerializeField] PlayerController myPlayerController;
   
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(Input.GetAxis("Mouse Y")*-1, Input.GetAxis("Mouse X") , 0) * -1 );
        //transform.localEulerAngles = Input.mousePosition;
        var horizonAngle = transform.localEulerAngles.x;
        var verticalAngle = transform.localEulerAngles.y;

        if (horizonAngle > 180 && horizonAngle < 340)
        {
            horizonAngle = 340;

        }
        else if (horizonAngle < 180 && horizonAngle > 40)
        {
            horizonAngle = 40;
        }
        if (verticalAngle > 180 && verticalAngle < 340)
        {
            verticalAngle = 340;

        }
        else if (verticalAngle < 180 && verticalAngle > 40)
        {
            verticalAngle = 40;
        }

        transform.localEulerAngles = new Vector3(horizonAngle, verticalAngle, 0);


        //if (transform.eulerAngles.x > 180)
        //{
        //    transform.eulerAngles = new Vector3(180f, transform.eulerAngles.y, 0);
        //}
        //if (transform.eulerAngles.y > 180)
        //{
        //    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180f, 0);
        //}
        //if (transform.eulerAngles.x < 0)
        //{
        //    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        //}
        //if (transform.eulerAngles.y < 0)
        //{
        //    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 0);
        //}
    }
}
