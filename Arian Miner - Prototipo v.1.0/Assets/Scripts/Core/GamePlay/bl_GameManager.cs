using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.IO;
using UnityEngine.SceneManagement;

public class bl_GameManager : Singleton<bl_GameManager> 
{

    [Header("[Nivel]")]
    public List<bl_LevelInfo> Levels = new List<bl_LevelInfo>();
    private int lastLevel = 0;
    public Material[] LevelMats;
    private int currentObstacleLevel = 0;

    [Header("[Healt Points]")]
    public Image healthPointsBar;
    [Range(1,5)] public int healthPoints = 3;
    [Range(0f,0.3f)] public float healthPointsFillAmountDefault;
    private float healthPointsAmount;
    private int hits = 0;

    [Header("[Audio]")]
	public AudioClip pointSound;
    //[SerializeField]private AudioClip progresivePointSound;
	public AudioClip[] musics;
	public AudioSource audioSource;
    [SerializeField]private AudioSource SfxSource;
    [SerializeField]private AudioSource VoiceSource;
    [SerializeField]private AudioMixerSnapshot NormalSnap;
    [SerializeField]private AudioMixerSnapshot PauseSnap;

    [Header("[HUD]")]
    [SerializeField]private Text LevelText;
    public Text lapsText;

    [Header("[Score]")]
    public Text scoreText;
    [SerializeField]private GameObject CounterScore;
    [SerializeField]private Animator ScorePAnim;
    [SerializeField]private Image FilledScoreImg;
    public Image[] checkPointsImg;
    [SerializeField] private Sprite checkPointSpriteOn;
    [SerializeField] private Sprite checkPointSpriteOff;
	private int _point = 0;
    private int PointByLevel = 0;
    private int NextScoreByLevel = 0;
    private int savedPoints = 0;

    [Header("[Timer]")]
    public Image timerBar;
    public Text timeUpText;
    public float maxTime = 60f;
    private float timeLeft;

    [Header("[Misc]")]
	public Transform player;
    public Transform stabilizer;
    [HideInInspector]public Vector3 CacheDefaultPlayerScale;
    private Vector3 cacheDeafultPlayerPosition;
    [SerializeField]private Animator PauseAnim;
    private bool isPause = false;
    private bool isGameOver;
    public bl_PlayerController PlayerController;
 
    // Time & Laps
    private int hashTime = 0;
    private float prevRealTime;
    private float thisRealTime;
    private int laps = 0;
    private int PlayTimes;

    //Encapsulated
    public int point
	{
		set 
		{
			_point = value;

            if (!isGameOver) 
			{
                new Arian.bl_GlobalEvents.OnPoint(value);
                SetPoint(value);
			}
		}
		get 
		{
			return _point;
		}
	}
	public float ScrollSpeed
	{
		get 
		{
			return GetLevelSpeed();
		}
	}

    // Functions
	void Awake()
	{
        audioSource = GetComponent<AudioSource> ();
		Application.targetFrameRate = 60;
		RenderSettings.ambientLight = Color.white;
		isGameOver = true;
        FilledScoreImg.fillAmount = 0;
        LevelText.canvasRenderer.SetAlpha(0);
        CacheDefaultPlayerScale = player.transform.localScale;
        cacheDeafultPlayerPosition = player.localPosition;
        PauseSnap.TransitionTo(1);

        LoadSettings();
        PlayRandomMusic();
        SetHP();
        SetLaps();
    }

    void Update()
    {
        //TimeManager();
        ScrollLevel();
        PCInputs();
        //PlayTimeBar();
        PlayTimer();
    }

    void LoadSettings()
    {
        audioSource.volume = 0.1f;
        AudioListener.volume = 1;
        CounterScore.SetActive(true);
    }

	void PlayRandomMusic()
	{
		if (PlayerPrefs.GetInt (KeyMasters.Sound, 1) == 0)
			return;
		
		audioSource.clip = musics [Random.Range (0, musics.Length)];
		audioSource.Play ();
	}

    private void SetHP() 
    {
        healthPointsBar.fillAmount = 1 - healthPointsFillAmountDefault;
        healthPointsAmount = (1 - healthPointsFillAmountDefault * 2)/healthPoints;
    }

    private void LoseHP()
    {
        healthPointsBar.fillAmount -= healthPointsAmount;

    }
    public void Hit()
    {   
        LoseHP();
        hits++;        
        if(hits >= healthPoints) GameOver();
        else bl_Shaker.Instance.Do(3);
    }

    public void PlayTimer()
    {
        if(!isGameOver)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                timerBar.fillAmount = timeLeft / maxTime;
                
                string minutes  = Mathf.Floor(timeLeft / 60).ToString("00");                // minutes
                string seconds  = Mathf.Floor(timeLeft % 60).ToString("00");                // seconds
                string millis   = Mathf.Floor((timeLeft * 1000) % 1000).ToString("000");    // miliseconds
                timeUpText.text = minutes + ":" + seconds + "." + millis;
            }
            else
            {
                OnFinishMining(MiningStates.TimeUp);
            }
        }
    }

    public void ClearTimer()
    {
        timeLeft = maxTime;
        timerBar.fillAmount = 1;
    }

    public void SetLaps()
    {
        laps++;

        if(laps > 0 && laps <= 1)
           lapsText.text = "<size=30>LAP</size><size=60>0" + laps + "</size><size=25>ST</size>";
        else if (laps > 1 && laps < 2)
           lapsText.text = "<size=30>LAP</size><size=60>0" + laps + "</size><size=25>ND</size>";
        else if (laps > 2 && laps < 3)
           lapsText.text = "<size=30>LAP</size><size=60>0" + laps + "</size><size=25>RD</size>";
        else if (laps > 3 && laps < 10)
           lapsText.text = "<size=30>LAP</size><size=60>0" + laps + "</size><size=25>RD</size>";
        else if (laps >= 10)
           lapsText.text = "<size=30>LAP</size><size=60>" + laps + "</size><size=25>TH</size>";
    }

    public void ClearLaps()
    {
        laps = 0;
        lapsText.text = "<size=30>LAP</size><size=60>00</size>";
    }

    void SetPoint(int p)
    {
        SfxSource.PlayOneShot(pointSound);
        PointByLevel++;

        float percent = (PointByLevel * 100) / NextScoreByLevel;
        Debug.Log(percent + "% :" + PointByLevel + " / " + NextScoreByLevel);

        FilledScoreImg.fillAmount = percent / 100;
        bl_Showwave.Instance.Play("middle",Color.black);

        if (HaveMoreLevels)
        {
            if(point >= Levels[lastLevel].PointsNeeded)
            {
                checkPointsImg[lastLevel].sprite = checkPointSpriteOn;
                lastLevel++;
                if(!(lastLevel > Levels.Count - 1))
                {
                    OnSpeedLevel();
                }
                else
                {
                    OnFinishMining(MiningStates.Success);
                    Continue();

                }
            }
        }

        bl_Shaker.Instance.Do(4);
    }

    void OnSpeedLevel()
    {
        if (Levels[lastLevel].ObstaclesLevel > currentObstacleLevel)
        {
            currentObstacleLevel = Levels[lastLevel].ObstaclesLevel;
        }

        ShowLevelText();
        bl_SlowMotion.Instance.DoSlow(1, 0.1f,true);

        SavePoints(PointByLevel);
        PointByLevel = 0;

        int last = GetLastScoreNeeded();
        NextScoreByLevel = GetNextScoreNeeded() - last;

        FilledScoreImg.fillAmount = 0;
        bl_Stylish.Instance.ChangeStyle(lastLevel);
        
        if (Levels[lastLevel].m_AudioClip != null)
        {
            VoiceSource.clip = Levels[lastLevel].m_AudioClip;
            VoiceSource.PlayDelayed(1);
        }
    }

    void SavePoints(int points)
    {
        savedPoints += points;
        Debug.Log("PointSaved: " + savedPoints);
    }

    void ShowLevelText(float hideIn = 3)
    {
        if (string.IsNullOrEmpty(Levels[lastLevel].Name))
        {
            LevelText.CrossFadeAlpha(1, 2, true);
            LevelText.text = Levels[lastLevel].Name;
        }
        else
        {
            LevelText.text = Levels[lastLevel].Name;
            LevelText.CrossFadeAlpha(1, 1.5f, true);
        }
        Invoke("HideLevelText", hideIn);
    }

    void HideLevelText() { LevelText.CrossFadeAlpha(0, 2, true);}

    void OnFinishMining(MiningStates _state)
    {
        new Arian.bl_GlobalEvents.OnFinishMining(_state);
        Continue();
    }

	public void StartGame()
    {
        Arian.bl_Event.Global.DispatchEvent(new Arian.bl_GlobalEvents.OnStartPlay());

        NormalSnap.TransitionTo(1.5f);

        player.gameObject.SetActive(true);

        bl_ChangerManager.Instance.Change(LevelSides.Down, false);


        //reset values
        _point = 0;
        PointByLevel = 0;

        VoiceSource.clip = Levels[0].m_AudioClip;
        VoiceSource.PlayDelayed(1);

        NextScoreByLevel = GetNextScoreNeeded();
        new Arian.bl_GlobalEvents.OnPoint(0);
        isGameOver = false;
        bl_Stylish.Instance.ChangeStyle(0);
        PlayTimes++;
        ShowLevelText(1.5f);

        timeLeft = maxTime;

    }

    public void Pause()
    {
        isPause = !isPause;
        new Arian.bl_GlobalEvents.OnPause(isPause);        
        if (isPause)
        {
            PauseSnap.TransitionTo(0.01f);
            PauseAnim.gameObject.SetActive(true);
            PauseAnim.SetBool("show", isPause);
        }
        else
        {
            PauseAnim.SetBool("show", isPause);
            StartCoroutine(Speedbox.bl_Utils.AnimatorUtils.WaitAnimationLenghtForDesactive(PauseAnim));
            NormalSnap.TransitionTo(1.5f);
        }
        Time.timeScale = (isPause) ? 0 : 1;
    }

	public void GameOver()
    {
        if (isGameOver)
            return;

        StopAllCoroutines();
        if (isPause)
        {
            Pause();
        }

        PauseAnim.gameObject.SetActive(false);
        
        isGameOver = true;
       
        bl_Showwave.Instance.Play("full", Color.white, bl_Showwave.Type.Full);
        
        //Per gamemode logic finish
        PauseSnap.TransitionTo(1);
        new Arian.bl_GlobalEvents.OnFailGame();
        bl_Shaker.Instance.Do(1);
        Reset();
    }    
    void Reset()
    {
        // Player
        PlayerController.correctRotation = false;
        stabilizer.rotation = Quaternion.identity;
    
        // Levels
        lastLevel = 0;
        bl_Stylish.Instance.ChangeStyle(lastLevel);

        // Score
        scoreText.text = "0";
        FilledScoreImg.fillAmount = 0;
        for(int i = 0; i < checkPointsImg.Length; i++) checkPointsImg[i].sprite = checkPointSpriteOff;

        PointByLevel = 0;
        savedPoints = 0;
        _point = 0;

        // Timer
        ClearTimer();

        // Laps
        ClearLaps();
    }

    void Continue()
    {
        // Player
        PlayerController.correctRotation = false;
        stabilizer.rotation = Quaternion.identity;
    
        // Levels
        lastLevel = 0;
        bl_Stylish.Instance.ChangeStyle(lastLevel);

        // Score
        scoreText.text = "0";
        FilledScoreImg.fillAmount = 0;
        for(int i = 0; i < checkPointsImg.Length; i++) checkPointsImg[i].sprite = checkPointSpriteOff;

        PointByLevel = 0;
        savedPoints = 0;
        _point = 0;

        // Timer
        ClearTimer();
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    void PCInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isGameOver)
                StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnFinishMining(MiningStates.Missed);
        }
    }
/*
    void TimeManager()
    {
        prevRealTime = thisRealTime;
        thisRealTime = Time.realtimeSinceStartup;
    }
*/
    void ScrollLevel()
    {
        foreach(Material m in LevelMats)
        {
            m.SetTextureOffset("_MainTex", new Vector2(0, -Time.time * ScrollSpeed));
        }
    }

    public bool HaveMoreLevels
    {
        get
        {
            return !(lastLevel > Levels.Count - 1);
        }
    }

    public int GetCurrentObstacleLevel
    {
        get
        {
            return currentObstacleLevel;
        }
    }

    public int GetLevelSpeed()
    {
        int speed = Levels[0].Speed;
        foreach (bl_LevelInfo info in Levels)
        {
            if (point > info.PointsNeeded)
            {
                speed = info.Speed;
            }
        }
        Debug.Log("SPEED: " + speed);
        return speed;
    }

    public int GetNextScoreNeeded()
    {
        int speed = Levels[lastLevel].PointsNeeded;
      
        return speed;
    }

    public int GetLastScoreNeeded()
    {
        int speed = (lastLevel > 0) ? Levels[lastLevel - 1].PointsNeeded : Levels[0].PointsNeeded;

        return speed;
    }

    public float deltaTime
    {
        get
        {
            if (Time.timeScale > 0f) return Time.deltaTime / Time.timeScale;
            return Time.realtimeSinceStartup - prevRealTime; // Checks realtimeSinceStartup again because it may have changed since Update was called
        }
    }

    //public int GetCacheScore { get { return CacheScore; } }

    public int CurrentLevel
    {
        get
        {
            return lastLevel + 1;
        }
    }

    public static bl_GameManager Instance
    {
        get
        {
            return ((bl_GameManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}