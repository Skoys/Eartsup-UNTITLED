using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimGif : MonoBehaviour
{

    [SerializeField] private Material[] frames;
    [SerializeField] private float fps = 10.0f;

    public Renderer mat;
    private int index = 0;

    void Start()
    {
        mat = GetComponent<Renderer>();
        GIFUpdate();
    }

    void GIFUpdate()
    {
        mat.material = frames[index];
        index++;
        if (index == frames.Length)
        {
            index = 0;
        }
        Invoke("GIFUpdate", fps * 0.01f);
    }
}
