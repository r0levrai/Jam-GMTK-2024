using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class EndCard : MonoBehaviour
{
    private float scaleStart_ = 1, scaleEnd_ = 1, scaleOld_ = 1;
    private float rotationStart_ = 0, rotationEnd_ = 0;
    private Vector3 positionStart_ = new Vector3(0,0), positionEnd_ = new Vector3(0, 0), positionOld_ = new Vector3(0,0);
    private float alphaStart_ = 1, alphaEnd_ = 1;

    public TMPro.TextMeshProUGUI timeAgoText;
    public UnityEngine.UI.Image background;
    public Draw draw;

    public Canvas textCanvas;
    public PanelSettings panelSettings;

    //private CanvasGroup canvasGroup;

    public float animDuration_ = 1;

    public float time_ = 0;

    public bool easingout = true;
    private bool isHover = false;

    public int like_score = 0;
    public int funny_score = 0;
    public int bad_score = 0;
    public NetworkedDrawing networkedDrawing;

    private UIDocument mainDoc;

    public bool blocked_like = false;
    public bool blocked_funny = false;
    public bool blocked_bad = false;

    public bool zoomed = false;

    public GameObject likeButton, laughButton, badButton;

    public TMPro.TextMeshProUGUI likeCount, laughCount, perplexedCount; 


    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localPosition = new Vector3(-100, -100);
        //canvasGroup = GameObject.Find("UIDocument").GetComponent<CanvasGroup>();
        mainDoc = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        textCanvas.sortingOrder = 10;
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

        gameObject.transform.localScale = new Vector3(currentScale + (isHover ? 0.1f : 0), currentScale + (isHover ? 0.1f : 0), 1);
        //draw.transform.localScale = new Vector3(currentScale, currentScale, 1);
        //background.transform.localScale = new Vector3(currentScale, currentScale, 1);
        gameObject.transform.localEulerAngles =  new Vector3(0, 0, currentRotation + (isHover ? 5:0));
        gameObject.transform.localPosition = currenPos;
        setAlpha(currentAlpha);

        CheckClick();
        CheckHover();
    }


    private void CheckHover()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (zoomed)
        {
            if (likeButton.GetComponent<BoxCollider2D>().OverlapPoint(point))
            {
                likeButton.transform.localEulerAngles = new Vector3(0, 0, 5);
                likeButton.transform.localScale = new Vector3(1.1f, 1.1f, 1);
            }
            else
            {
                likeButton.transform.localEulerAngles = new Vector3(0, 0, 0);
                likeButton.transform.localScale = new Vector3(1, 1, 1);
            }

            if (laughButton.GetComponent<BoxCollider2D>().OverlapPoint(point))
            {
                laughButton.transform.localEulerAngles = new Vector3(0, 0, 5);
                laughButton.transform.localScale = new Vector3(1.1f, 1.1f, 1);
            }
            else
            {
                laughButton.transform.localEulerAngles = new Vector3(0, 0, 0);
                laughButton.transform.localScale = new Vector3(1, 1, 1);
            }

            if (badButton.GetComponent<BoxCollider2D>().OverlapPoint(point))
            {
                badButton.transform.localEulerAngles = new Vector3(0, 0, 5);
                badButton.transform.localScale = new Vector3(1.1f, 1.1f, 1);
            }
            else
            {
                badButton.transform.localEulerAngles = new Vector3(0, 0, 0);
                badButton.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else if(!Constants.Instance.pauseTitleAnimation)
        {
            if(GetComponent<BoxCollider2D>().OverlapPoint(point))
            {
                isHover = true;
                textCanvas.sortingOrder = 100;
            }
            else
            {
                isHover = false;
                textCanvas.sortingOrder = 10;
            }
        }
    }


    public void Set(NetworkedDrawing drawing, string customText = null)
    {
        timeAgoText.text = customText != null ? customText : drawing.GetTimeDifference();
        int iBg = int.TryParse(drawing.data.background, out iBg) ? iBg : 0;
        background.sprite = Constants.Instance.GetCurrentImage(iBg);
        draw.SetDrawingData(drawing.GetDrawingData());
        like_score = drawing.data.like;
        funny_score = drawing.data.funny;
        bad_score = drawing.data.bad;
        networkedDrawing = drawing;

        likeCount.text = like_score.ToString();
        laughCount.text = like_score.ToString();
        perplexedCount.text = like_score.ToString();

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

    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!Constants.Instance.pauseTitleAnimation)
            {
                if(GetComponent<BoxCollider2D>().OverlapPoint(point))
                {
                    zoomed = true;
                    Constants.Instance.pauseTitleAnimation = true;
                    isHover = false;
                    positionOld_ = positionEnd_;
                    scaleOld_ = scaleEnd_;

                    //canvasGroup.interactable = false;
                    //canvasGroup.blocksRaycasts = false;
                    //canvasGroup.alpha = 0;

                    mainDoc.rootVisualElement.visible = false;


                    move(new Vector3(0, 0, -1));
                    rotate(0);
                    scale(1.3f);
                    time_ = 0;

                    textCanvas.sortingOrder = 100;
                }
            }
            else
            {
                if(zoomed)
                {
                    if (likeButton.GetComponent<BoxCollider2D>().OverlapPoint(point))
                    {
                        ClickLike();
                    }
                    else if (laughButton.GetComponent<BoxCollider2D>().OverlapPoint(point))
                    {
                        ClickLaugh();
                    }
                    else if (badButton.GetComponent<BoxCollider2D>().OverlapPoint(point))
                    {
                        ClickPerplexed();
                    }
                    else if (!GetComponent<BoxCollider2D>().OverlapPoint(point))
                    {
                        SetPreviousCard();
                    }
                }
            }

        }
        
        
    }
    private void ClickLike()
    {
        if(!blocked_like)
        {
            like_score++;
            likeCount.text = like_score.ToString();
            blocked_like = true;

            _ = networkedDrawing.SendReaction("like");
        }
    }

    private void ClickLaugh()
    {
        if (!blocked_funny)
        {
            funny_score++;
            laughCount.text = funny_score.ToString();

            blocked_funny = true;
            _ = networkedDrawing.SendReaction("funny");
        }
    }

    private void ClickPerplexed()
    {
        if (!blocked_bad)
        {
            bad_score++;
            perplexedCount.text = bad_score.ToString();

            blocked_bad = true;
            _ = networkedDrawing.SendReaction("bad");
        }
    }
    private void SetPreviousCard()
    {
        Constants.Instance.pauseTitleAnimation = false;
        zoomed = false;

        //canvasGroup.interactable = true;
        //canvasGroup.blocksRaycasts = true;
        //canvasGroup.alpha = 1;

        mainDoc.rootVisualElement.visible = true;

        positionEnd_ = positionOld_;
        scaleEnd_ = scaleOld_;
        move(positionOld_);
        rotate(0);
        scale(scaleOld_);
        time_ = 0;
        textCanvas.sortingOrder = 10;
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

    public void scale(float target)
    {
        scaleStart_ = scaleEnd_;
        scaleEnd_ = target;
    }
}



