using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objetive;
    public float cameraSpeed = 0.025f;
    [SerializeField] public Vector3 movement;


    private void LateUpdate()
    {
      Vector3 desirePosition = objetive.position + movement ;
      Vector3 smoothPosition = Vector3.Lerp(transform.position, desirePosition, cameraSpeed ) ;
      transform.position = smoothPosition ;
    }

}
