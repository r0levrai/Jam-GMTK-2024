using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleScreen : MonoBehaviour
{
    public EndCard endcard;
    public List<Sprite> spriteList_;

    public List<EndCard> endCards;
    
    public int nReceivedCards = 24;
    private float time_ = 0, timeBanane_ = 0;
    private int state = 0; // 0: pre ease-in, 1: ease-in, 2: easeout
    private int count = 0; // number of cards that have changed
    private float rotAngle_ = -10;
    private int currentSprite = 0;

    public GameObject banane;

    [Header("UI")]
	[SerializeField] private UIDocument uiTitleSceen;
    private Toggle volumeToggle, soundToggle;
    private Slider volumeSlider, soundSlider;
    private Button playButton, howToButton;
    private NetworkedDrawing[] drawings;
    private void Awake()
	{

        VisualElement root = uiTitleSceen.rootVisualElement;
        volumeToggle = root.Q<Toggle>("VolumeToggle");
        volumeSlider = root.Q<Slider>("VolumeSlider");
        soundToggle = root.Q<Toggle>("SoundToggle");
        soundSlider = root.Q<Slider>("SoundSlider");
        playButton = root.Q<Button>("PlayButton");
        howToButton = root.Q<Button>("HowButton");


	}

	// Start is called before the first frame update
	async void Start()
    {
        drawings = await NetworkedDrawing.ReceiveLasts(nReceivedCards);

        for (int i = 0; i < Mathf.Min(drawings.Length, 3); i++)
        {
            EndCard ec2 = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
            float rot2 = Random.Range(2.0f, 10.0f);
            if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
            ec2.setupRotation(rot2 * 3, -rot2);
            ec2.setupScale(0.45f, 0.45f);
            ec2.setupPosition(new Vector3(-15, 4.5f), new Vector3(18 / 4.0f * (i + 1) - 9, 3.15f + Random.Range(0, 0.1f)));
            ec2.time_ = -1 + Random.Range(0.0f, 0.2f);
            count ++;
            ec2.Set(drawings[count % drawings.Length]);
            endCards.Add(ec2);
        }

        playButton.RegisterCallback<MouseEnterEvent>(_ => SoundManager.Instance.PlayOneShot("hover1"));
        playButton.RegisterCallback<ClickEvent>(_ => SoundManager.Instance.PlayOneShot("clickPaper"));
        howToButton.RegisterCallback<MouseEnterEvent>(_ => SoundManager.Instance.PlayOneShot("hover2"));
        howToButton.RegisterCallback<ClickEvent>(_ => SoundManager.Instance.PlayOneShot("clickPaper"));
        playButton.clicked += () => SceneManager.LoadSceneAsync(1);
        howToButton.clicked += () => SceneManager.LoadSceneAsync(3);
        volumeSlider.RegisterValueChangedCallback((evt) =>
		{
			SoundManager.Instance.ChangeVolumeMusic(evt.newValue / volumeSlider.highValue);
			SoundManager.Instance.PlaySound("click");
		});
		soundSlider.RegisterValueChangedCallback((evt) =>
		{
			SoundManager.Instance.ChangeVolumeSound(evt.newValue / soundSlider.highValue);
			SoundManager.Instance.PlaySound("click");
		});
        volumeToggle.RegisterValueChangedCallback(evt =>
        {
            SoundManager.Instance.musicSource.mute = !evt.newValue;
            SoundManager.Instance.newMusicSource.mute = !evt.newValue;
			SoundManager.Instance.PlayOneShot("click");
		});
        soundToggle.RegisterValueChangedCallback(evt =>
        {
            SoundManager.Instance.soundEffectSource.mute = !evt.newValue;
			SoundManager.Instance.PlayOneShot("click");
		});
		volumeSlider.value = SoundManager.Instance.volumeMusic * volumeSlider.highValue;
        soundSlider.value = SoundManager.Instance.volumeSound * soundSlider.highValue;
	}

    // Update is called once per frame
    void Update()
    {
        if(!Constants.Instance.pauseTitleAnimation)
            time_ += Time.deltaTime;
        timeBanane_ += Time.deltaTime;

        if(timeBanane_ > 2)
        {
            timeBanane_ = 0;
            banane.transform.Rotate(new Vector3(0, 0, rotAngle_));
            rotAngle_ *= -1;

            int newCurrent = currentSprite;
            while (newCurrent == currentSprite) {
                newCurrent = Random.Range(0, spriteList_.Count);
            }

            banane.GetComponent<SpriteRenderer>().sprite = spriteList_[newCurrent];
            currentSprite = newCurrent;
        }

        // pre ease in
        if (time_ > 0 && state == 0)
        {
            state++;
            for (int i = 0; i < 3; i++)
            {
                count++;
                if (drawings != null && drawings.Length > 0)
                {
                    endCards[count % 3].Set(drawings[count % drawings.Length]);
                }
            }
        }

        // ease in
        if (time_ > 3 && state == 1)
        {
            state++;
            for (int i = 0; i < 3; i++)
            {
                if (i >= endCards.Count) { break; }
                endCards[i].move(new Vector3(12, 2.5f));
                endCards[i].rotate(Random.Range(2.0f, 10.0f));
                endCards[i].time_ = Random.Range(0.0f, 0.2f);
                endCards[i].easingout = false;
            }
        }

        // ease out
        if(time_ > 5 && state == 2)
        {
            state = 0;
            time_ = 0;
            for (int i = 0; i < 3; i++)
            {
                if (i >= endCards.Count) { break; }
                float rot2 = Random.Range(2.0f, 10.0f);
                if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
                endCards[i].setupRotation(rot2 * 3, -rot2);
                endCards[i].setupScale(0.45f, 0.45f);
                endCards[i].setupPosition(new Vector3(-15, 4.5f), new Vector3(18 / 4.0f * (i + 1) - 9, 3.15f + Random.Range(0, 0.1f)));
                endCards[i].time_ = Random.Range(0.0f, 0.2f);
                endCards[i].easingout = true;
                endCards[i].blocked_bad = false;
                endCards[i].blocked_like = false;
                endCards[i].blocked_funny = false;
            }
        }
    }
}
