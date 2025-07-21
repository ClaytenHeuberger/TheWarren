using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMarker : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Sprite onScreen;
    [SerializeField] private Sprite offScreen;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float sightAngle = 30f;
    [SerializeField] private float spriteSize = 60f;

    RectTransform rectTrans;

    Image this_image;

    bool targetOnScreen = false;

    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        this_image = GetComponent<Image>();
        audioManager = FindObjectOfType<AudioManager>();

        this_image.color = Color.red * 2f;

        Canvas canvas = FindObjectOfType<Canvas>();
        transform.SetParent(canvas.transform);
    }

    void FixedUpdate()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        bool inScreenBounds = screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height;

        float radius = Screen.height - offScreen.rect.width/2;

        Vector3 heading = (target.position - Camera.main.transform.position).normalized;




        if (inScreenBounds)
        {
            if(Vector3.Angle(heading, Camera.main.transform.forward) < sightAngle)
            {


                var tempColor = this_image.color;
                tempColor.a = 1f;
                this_image.color = tempColor;


                //Just came on screen
                if (targetOnScreen == false)
                {
                    StartCoroutine(AquireTarget(0.15f, 10f));
                }


                this_image.sprite = onScreen;

                rectTrans.position = screenPos;

                targetOnScreen = true;
            }
            else
            {

                var tempColor = this_image.color;
                tempColor.a = 0f;
                this_image.color = tempColor;
            }


        }
        else
        {
            this_image.sprite = offScreen;
            rectTrans.sizeDelta = new Vector2(spriteSize / 3, spriteSize / 3);



            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

            screenPos -= screenCenter;

            float angle = Mathf.Atan2(screenPos.x, screenPos.y);
            //angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            screenPos = screenCenter + new Vector3(sin*150, cos*150, 0);

            float m = cos / sin;

            Vector3 screenBounds = screenCenter * 0.9f;

            //Up and down

            if(cos > 0)
            {
                screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            }
            else
            {
                screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);

            }

            //Out of bounds

            if(screenPos.x > screenBounds.x)
            {
                screenPos = new Vector3(screenBounds.x,  screenBounds.x * m, 0);
            }else if(screenPos.x < -screenBounds.x)
            {
                screenPos = new Vector3(-screenBounds.x,  -screenBounds.x * m, 0);
            }

            screenPos += screenCenter;


            rectTrans.position = screenPos;
            rectTrans.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

            targetOnScreen = false;
        }

    }

    IEnumerator AquireTarget(float duration, float size)
    {
        float elapsed = 0.0f;
        Vector2 originalSize = new Vector2(spriteSize, spriteSize);

        rectTrans.sizeDelta *= size;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;

            rectTrans.sizeDelta = Vector2.Lerp(originalSize * size, originalSize, elapsed / duration);

            yield return null;
        }

        audioManager.Play("TargetAquired");

    }
}
