using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerTriggerCameraConfiner : MonoBehaviour
{
    
    [SerializeField] CinemachineStateDrivenCamera stateDrivenCamera;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
        {
            //stateDrivenCamera.AnimatedTarget = playerManager.ConfinePlayerCamera();
            stateDrivenCamera.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (other.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
            {
                stateDrivenCamera.gameObject.SetActive(false);
            }
        }
    }
}
