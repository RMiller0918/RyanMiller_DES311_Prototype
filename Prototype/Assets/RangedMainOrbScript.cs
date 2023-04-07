using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMainOrbScript : MonoBehaviour
{
    [field: SerializeField] public Animator Animator { get; set; }
    [field: SerializeField] public int ShrinkHash { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        ShrinkHash = Animator.StringToHash("Shrink");
    }

    public void TriggerStart()
    {
        Animator.SetTrigger(ShrinkHash);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, 1f * Time.deltaTime);
    }

    void OnEnable()
    {

    }
}
