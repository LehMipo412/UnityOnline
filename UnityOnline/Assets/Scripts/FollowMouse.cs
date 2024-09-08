using UnityEngine;

public class FollowMouse : MonoBehaviour
{
   [SerializeField] PlayerController myPlayerController;
   [SerializeField] Transform myPlayer;
    void Update()
    {
        myPlayer.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") , 0));

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
    }
}
