using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.UI;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;  // Drag your 3D prefab here
    public XROrigin xrOrigin;  // Use XROrigin instead of ARSessionOrigin

    private void Start()
    {
        ARSession.stateChanged += OnARSessionStateChanged;
        // Check if AR session has started
        if (ARSession.state == ARSessionState.SessionTracking)
        {
            SpawnObject();
        }
    }

    void Awake()
    {
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("This GameObject is not a root object and cannot use DontDestroyOnLoad.");
        }
    }
    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        if (args.state == ARSessionState.None || args.state == ARSessionState.Unsupported)
        {
            Debug.LogError("AR session is not supported or has been stopped.");
        }
    }
    void SpawnObject()
    {
        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            Vector3 spawnPosition = xrOrigin.Camera.transform.position + xrOrigin.Camera.transform.forward * 0.5f;
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            Debug.Log("Object spawned at: " + spawnPosition);
        }
        else
        {
            Debug.LogError("Camera or XROrigin is missing or destroyed!");
        }
    }
}
