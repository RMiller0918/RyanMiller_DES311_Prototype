using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _lights;
    [SerializeField] private const string LIGHTTAG = "Light";

    private void Awake()
    {
        _lights = GameObject.FindGameObjectsWithTag(LIGHTTAG).ToList();
    }
}
