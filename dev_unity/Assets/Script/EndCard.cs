using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EndCard : MonoBehaviour
{
    private float scaleStart_ = 1, scaleEnd_ = 1;
    private float rotationStart_ = 0, rotationEnd_ = 0;
    private Vector2 positionStart_ = new Vector2(0,0), positionEnd_ = new Vector2(0, 0);
    private float alphaStart_ = 1, alphaEnd_ = 1;

    public TMPro.TextMeshProUGUI timeAgoText;
    public SpriteRenderer background;
    public Draw draw;

    public float animDuration_ = 1;

    public float time_ = 0;

    public bool easingout = true;

    // Start is called before the first frame update
    void Start()
    {
        //setupScale(0.5f, 1);
        //setupRotation(2, -2);
        //setupAlpha(0.4f, 1);
        //setupPosition(new Vector3(-1, -1, 0), new Vector3(0, 0, 0));

        //time_ = -2;
    }

    // Update is called once per frame
    void Update()
    {
        time_ += Time.deltaTime;
        float currentT = time_ / animDuration_;

        float currentScale = getCurrentVal(scaleStart_, scaleEnd_, currentT);
        float currentRotation = getCurrentVal(rotationStart_, rotationEnd_, currentT);
        float currentAlpha = getCurrentVal(alphaStart_, alphaEnd_, currentT);
        Vector3 currenPos = getCurrentVal(positionStart_, positionEnd_, currentT);

        gameObject.transform.localScale = new Vector3(currentScale, currentScale, 1);
        draw.transform.localScale = new Vector3(currentScale, currentScale, 1);
        background.transform.localScale = new Vector3(currentScale, currentScale, 1);
        gameObject.transform.localEulerAngles =  new Vector3(0, 0, currentRotation);
        gameObject.transform.localPosition = currenPos;
        setAlpha(currentAlpha);
    }

    public void setAlpha(float alpha)
    {
        SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
        Color newColor;
        foreach (SpriteRenderer child in children)
        {
            newColor = child.color;
            newColor.a = alpha;
            child.color = newColor;
        }

        TMPro.TextMeshProUGUI[] texts = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (TMPro.TextMeshProUGUI text in texts)
        {
            newColor = text.color;
            newColor.a = alpha;
            text.color = newColor;
        }

    }

    void OnMouseDown()
    {
        Debug.Log("Sprite Clicked");
    }

    void OnMouseOver()
    {
        Debug.Log("Sprite Hovered");
    }

    float getCurrentVal(float start, float end, float t)
    {
        return start + (end - start) * getEasing(t);
    }

    Vector3 getCurrentVal(Vector3 start, Vector3 end, float t)
    {
        return start + (end - start) * getEasing(t);
    }

    float getEasing(float t)
    {
        if(easingout) return t >= 1 ? 1 : t < 0 ? 0 : 1 - (float)System.Math.Pow(2, -10 * t);
        else return t <= 0 ? 0 : t > 1 ? 1 : (float)System.Math.Pow(2, 10 * t - 10);
    }

    public void setupScale(float start, float end)
    {
        scaleStart_ = start;
        scaleEnd_ = end;
    }

    public void setupRotation(float start, float end)
    {
        rotationStart_ = start;
        rotationEnd_ = end;
    }

    public void setupAlpha(float start, float end)
    {
        alphaStart_ = start;
        alphaEnd_ = end;
    }

    public void setupPosition(Vector3 start, Vector3 end)
    {
        positionStart_ = start;
        positionEnd_ = end;
    }

    public void move(Vector3 target)
    {
        positionStart_ = positionEnd_;
        positionEnd_ = target;
    }

    public void rotate(float target)
    {
        rotationStart_ = rotationEnd_;
        rotationEnd_ = rotationStart_ + target;
    }
}



