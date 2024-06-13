using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Bullet_Behaviour : MonoBehaviour
{
    public void Start()
    {
        VisualEffect visualEffect = GetComponent<VisualEffect>();
        visualEffect.Play();
        Invoke(nameof(DestroyGameObject), 2);
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
