using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTexManager : MonoBehaviour
{
    public GameObject monster;

    private void OnEnable()
    {
        monster.SetActive(true);
    }

    private void OnDisable()
    {
        monster.SetActive(false);
    }

    private void Update()
    {
        monster.transform.Rotate(Input.acceleration.y * 0.5f, Input.acceleration.x * 0.5f, Input.acceleration.z * 0.5f);
    }
}
