using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPulse : MonoBehaviour
{
    [SerializeField] private float growSpeed = 100f;
    [SerializeField] private float lifeTime = 1f;

    float life;
    private Material mat;

    float speed;
    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        life = lifeTime;
        speed = growSpeed;
    }
    void Update()
    {
        //transform.localScale += new Vector3(growSpeed * Time.deltaTime, growSpeed * Time.deltaTime, growSpeed * Time.deltaTime);


        float transparency = Mathf.Lerp(1, 500, 1 - (life/lifeTime));
        mat.SetFloat("_Transparency", transparency);

        //growSpeed = Mathf.Lerp(speed, 0, 1 - (life / lifeTime));

        //life -= Time.deltaTime;
        //if (life <= 0)
            //Destroy(gameObject);
    }
}
