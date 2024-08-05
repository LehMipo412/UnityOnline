using UnityEngine;
using Unity.Cinemachine;

public class PanAndTlitClamper : MonoBehaviour
{
    [SerializeField] CinemachinePanTilt cameraRotationIndexr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var horizonAngle = cameraRotationIndexr.PanAxis;
        var verticalAngle = cameraRotationIndexr.TiltAxis;

      //  horizonAngle.ClampValue
        if (horizonAngle.Value > 180 && horizonAngle.Value < 340)
        {
            horizonAngle.Value = 340;

        }
        else if (horizonAngle.Value < 180 && horizonAngle.Value > 40)
        {
            horizonAngle.Value = 40;
        }
        if (verticalAngle.Value > 180 && verticalAngle.Value < 340)
        {
            verticalAngle.Value = 340;

        }
        else if (verticalAngle.Value < 180 && verticalAngle.Value > 40)
        {
            verticalAngle.Value = 40;
        }
    }
}
