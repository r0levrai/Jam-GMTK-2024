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

    private float time_ = 0, timeBanane_ = 0;
    private bool once = false;
    private float rotAngle_ = -10;
    private int currentSprite = 0;

    public GameObject banane;

    [Header("UI")]
	[SerializeField] private UIDocument uiTitleSceen;
    private Toggle volumeToggle, soundToggle;
    private Slider volumeSlider, soundSlider;
    private Button playButton, howToButton;
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
	void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            EndCard ec2 = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
            float rot2 = Random.Range(2.0f, 10.0f);
            if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
            ec2.setupRotation(rot2 * 3, -rot2);
            ec2.setupScale(0.45f, 0.45f);
            ec2.setupPosition(new Vector3(-15, 4.5f), new Vector3(18 / 4.0f * (i + 1) - 9, 3.15f + Random.Range(0, 0.1f)));
            ec2.time_ = -1 + Random.Range(0.0f, 0.2f);

            endCards.Add(ec2);
        }

        playButton.clicked += () => SceneManager.LoadSceneAsync(1);
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

        if(time_ > 3 && !once)
        {
            once = true;
            for(int i = 0; i < 3; i++)
            {
                endCards[i].move(new Vector3(12, 2.5f));
                endCards[i].rotate(Random.Range(2.0f, 10.0f));
                endCards[i].time_ = Random.Range(0.0f, 0.2f);
                endCards[i].easingout = false;
            }
        }

        if(time_ > 5)
        {
            time_ = 0;
            once = false;
            for (int i = 0; i < 3; i++)
            {
                float rot2 = Random.Range(2.0f, 10.0f);
                if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
                endCards[i].setupRotation(rot2 * 3, -rot2);
                endCards[i].setupScale(0.45f, 0.45f);
                endCards[i].setupPosition(new Vector3(-15, 4.5f), new Vector3(18 / 4.0f * (i + 1) - 9, 3.15f + Random.Range(0, 0.1f)));
                endCards[i].time_ = Random.Range(0.0f, 0.2f);
                endCards[i].easingout = true;
            }
        }
    }
}
