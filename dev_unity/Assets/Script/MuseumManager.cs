using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MuseumManager : MonoBehaviour
{
    // Start is called before the first frame update

    public EndCard endcardPrefab;
    
    private List<EndCard> listCards = new List<EndCard>();

    private bool destroy = false;
    private int menuIndex = 0;
    private float time = 0;
    private int currentPage = 0, maxPage = 5, cardsPerPage = 8;

    public int nCards = 8;
    private bool request = false;

    NetworkedDrawing[] drawings;

    [Header("UI")]
    [SerializeField] private UIDocument uiTitleSceen;
    private Button lastButton, randomButton, mostButton, nextButton, prevButton;
    private Label pageLabel;

    private void Awake()
    {
        VisualElement root = uiTitleSceen.rootVisualElement;
        lastButton = root.Q<Button>("LastButton");
        randomButton = root.Q<Button>("RandomButton");
        mostButton = root.Q<Button>("MostButton");
        pageLabel = root.Q<Label>("PageLabel");
        nextButton = root.Q<Button>("NextButton");
        prevButton = root.Q<Button>("PrevButton");
    }

    async void Start()
    {
        nCards = cardsPerPage * maxPage;
        drawings = await NetworkedDrawing.ReceiveLasts(nCards);

        for (int i = 0; i < 8; i++)
        {
            EndCard ec = Instantiate(endcardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            listCards.Add(ec);
        }

        
        Populate(drawings);

        lastButton.clicked += () => LoadLast();
        lastButton.style.unityBackgroundImageTintColor = new Color(1, 0.65f, 0.65f);

        randomButton.clicked += () => LoadRandom();
        mostButton.clicked += () => LoadMost();

        pageLabel.text = (currentPage + 1).ToString() + "/" + maxPage.ToString();

        nextButton.clicked += () => Next();
        prevButton.clicked += () => Prev();

    }

    private void Populate(NetworkedDrawing[] drawings)
    {

        for (int i = 0; i < 4; i++)
        {
            if (drawings.Length <= i + 4) { return; }

            float rot2 = Random.Range(2.0f, 10.0f);
            if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
            listCards[i].setupRotation(rot2 * 3, -rot2);
            listCards[i].setupScale(0.5f, 0.5f);
            listCards[i].setupPosition(new Vector3(30 / 5.0f * (i + 1) - 15, -7.5f), new Vector3(18 / 5.0f * (i + 1) - 9, -5.0f / 3.0f));
            listCards[i].time_ = -0.25f - Random.Range(0.0f, 0.25f);
            listCards[i].Set(drawings[currentPage * cardsPerPage + i]);
            listCards[i].easingout = true;
        }
        for (int i = 0; i < 4; i++)
        {
            if (drawings.Length <= i) { return; }
            
            float rot2 = Random.Range(2.0f, 10.0f);
            if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
            listCards[i+ 4].setupRotation(rot2 * 3, -rot2);
            listCards[i+ 4].setupScale(0.5f, 0.5f);
            listCards[i+ 4].setupPosition(new Vector3(30 / 5.0f * (i + 1) - 15, 7.5f), new Vector3(18 / 5.0f * (i + 1) - 9, 5.0f/3.0f));
            listCards[i+ 4].time_ = -0.25f - Random.Range(0.0f, 0.25f);
            listCards[i+ 4].Set(drawings[currentPage * cardsPerPage + i + 4]);
            listCards[i+ 4].easingout = true;
        }

       
    }

    void Despawn()
    {
        time = 0;
        destroy = true;
        for (int i = 0; i < 4; i++)
        {
            if(i > listCards.Count) { return; }
            listCards[i].rotate(0);
            listCards[i].setupScale(0.5f, 0.5f);
            listCards[i].move(new Vector3(30 / 5.0f * (i + 1) - 15, 7.5f));
            listCards[i].time_ = Random.Range(0.0f, 0.2f);
            //listCards[i].easingout = false;
            listCards[i].blocked_bad = false;
            listCards[i].blocked_like = false;
            listCards[i].blocked_funny = false;
        }

        for (int i = 0; i < 4; i++)
        {
            if (i+4 > listCards.Count) { return; }
            listCards[i+4].rotate(0);
            listCards[i+4].setupScale(0.5f, 0.5f);
            listCards[i+4].move(new Vector3(30 / 5.0f * (i + 1) - 15, -7.5f));
            listCards[i+4].time_ = Random.Range(0.0f, 0.2f);
            //listCards[i+4].easingout = false;
            listCards[i+4].blocked_bad = false;
            listCards[i+4].blocked_like = false;
            listCards[i+4].blocked_funny = false;
        }
    }

    void LoadLast()
    {
        if(!destroy)
        {
            Despawn();
            request = true;
            menuIndex = 0;
            lastButton.style.unityBackgroundImageTintColor = new Color(1, 0.65f, 0.65f);
            randomButton.style.unityBackgroundImageTintColor = new Color(1, 1, 1);
            mostButton.style.unityBackgroundImageTintColor = new Color(1, 1, 1);
        }
    }

    void LoadMost()
    {
        if (!destroy)
        {
            Despawn();
            request = true;
            menuIndex = 1;
            mostButton.style.unityBackgroundImageTintColor = new Color(1, 0.65f, 0.65f);
            lastButton.style.unityBackgroundImageTintColor = new Color(1, 1, 1);
            randomButton.style.unityBackgroundImageTintColor = new Color(1, 1, 1);
        }
    }

    void LoadRandom()
    {
        if (!destroy)
        {
            Despawn();
            request = true;
            menuIndex = 2;
            randomButton.style.unityBackgroundImageTintColor = new Color(1, 0.65f, 0.65f);
            lastButton.style.unityBackgroundImageTintColor = new Color(1, 1, 1);
            mostButton.style.unityBackgroundImageTintColor = new Color(1, 1, 1);
        }
    }

    void Next()
    {
        if (!destroy)
        {
            if (currentPage + 1 >= maxPage) return;
            Despawn();
            currentPage += 1;
            pageLabel.text = (currentPage + 1).ToString() + "/" + maxPage.ToString();
        }
    }

    void Prev()
    {
        if (!destroy)
        {
            if (currentPage - 1 < 0) return;
            Despawn();
            currentPage -= 1;
            pageLabel.text = (currentPage + 1).ToString() + "/" + maxPage.ToString();
        }
    }


    // Update is called once per frame
    async void Update()
    {
        time += Time.deltaTime;
        if(destroy && time > 0.2f)
        {
            destroy = false;
            switch(menuIndex)
            {
                case 0:
                    if(request)
                    {
                        drawings = await NetworkedDrawing.ReceiveLasts(nCards);
                        request = false;
                    }
                    break;

                default:
                    break;
            }

            Populate(drawings);
        }
        pageLabel.visible = !Constants.Instance.pauseTitleAnimation;
    }
}
