using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class GunScript : MonoBehaviour
{
    [Header("Variables")]
    public bool discoverGun = false;
    public bool hasBullet = true;

    [Header("VFXs & others")]
    public VisualEffect muzzleVFX;
    public GameObject PREFAB_bullet;
    [SerializeField] private Animator animator;

    [Header("SFXs")]
    public AudioSource audioSource;
    public List<AudioClip> pistiolShots;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DiscoverGun()
    {
        animator.SetBool("discoverGun", true);
        animator.SetBool("munition", true);
    }

    public void GetAmmo()
    {
        hasBullet = true ;
        animator.SetBool("munition", true);
        animator.SetBool("shooting", false);
    }

    public void GunShot()
    {
        animator.SetBool("shooting", true);
        animator.SetBool("munition", false);
        animator.SetBool("haveGun", true);

        GameObject bullet = Instantiate(PREFAB_bullet);
        bullet.transform.position = gameObject.transform.position;
        bullet.transform.rotation = gameObject.transform.rotation;
        hasBullet = false;
    }
}
