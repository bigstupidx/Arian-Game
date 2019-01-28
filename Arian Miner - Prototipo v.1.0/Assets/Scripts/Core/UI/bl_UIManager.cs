using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_UIManager : MonoBehaviour {

	[SerializeField]private Text ScoreText;
    [SerializeField]private Text GameOverScoreText;
    [SerializeField]private Text InstructionText;
    [SerializeField]private Text FlipText;
    [SerializeField]private Text LevelGameOverText;
    [SerializeField]private Text QualityText;
    [SerializeField]private Text AudioEnableText;
    [SerializeField]private Text TapInitText;
    [SerializeField]private Slider VolumeSlider;
    [SerializeField]private Animator MenuAnim;
    [SerializeField]private Animator BeginAnim;
    [SerializeField]private Animator MessageBoardAnim;
    [SerializeField]private Animator PointScoreAnim;
    [SerializeField]private Animator PMWindows;
    [SerializeField]private GameObject MessageBoard;
    [SerializeField]private GameObject ScoreUI;
    [SerializeField]private GameObject[] PauseWindows;

    private bool quality;
    private bool audioEnable;

    void Start()
    {
        LoadPrefs();
        SetupScore();
        OnPlayAgain();
    } 

    void OnEnable()
    {
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnStartPlay>(OnStartPlay);
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnFailGame>(OnFailGame);
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnFinishMining>(OnFinishMining);
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnPoint>(OnPoint);
    }

    void OnDisable()
    {
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnStartPlay>(OnStartPlay);
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnFailGame>(OnFailGame);
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnFinishMining>(OnFinishMining);
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnPoint>(OnPoint);

    }

    void OnStartPlay(Arian.bl_GlobalEvents.OnStartPlay e)
    {
        ScoreText.gameObject.SetActive(true);
        ScoreUI.SetActive(true);

        MessageBoard.SetActive(false);

        BeginAnim.SetBool("show", false);        
        StartCoroutine(WaitForDesactive(BeginAnim.gameObject, BeginAnim.GetCurrentAnimatorClipInfo(0).Length));
        InstructionText.CrossFadeColor(new Color(0, 0, 0, 1), 2, true, true);
        Invoke("HideFirstText", 3);
    }

    void OnFailGame(Arian.bl_GlobalEvents.OnFailGame e)
    {
        SetupScore();
        ScoreText.gameObject.SetActive(false);
        StartCoroutine(WaitForShowGOUI());
    }

    public void OnPlayAgain()
    {        
        BeginAnim.gameObject.SetActive(true);
        ScoreUI.SetActive(true);
        BeginAnim.SetBool("show", true);

        MenuAnim.SetBool("show", false);
        StartCoroutine(WaitForDesactive(MenuAnim.gameObject, MenuAnim.GetCurrentAnimatorClipInfo(0).Length));
    }

    void OnFinishMining(Arian.bl_GlobalEvents.OnFinishMining e)
    {
        MessageBoard.SetActive(true);
        bl_MessageBoard msg = MessageBoard.GetComponent<bl_MessageBoard>() as bl_MessageBoard;
        msg.SetMessageBoard(e.miningState);
        StartCoroutine(PlayAnimationDelay(MessageBoardAnim, "show", 1f, true));
        
    }

    public void ChangePauseWindow(int id)
    {
        PMWindows.Play("change", 0, 0);
        StartCoroutine(WaitActiveInArray(PauseWindows, id,0.2f));
    }

    IEnumerator PlayAnimationDelay(Animator anim,string animClip,float delay,bool desactive = false,float timeanim = 0)
    {
        yield return new WaitForSeconds(delay);
        anim.gameObject.SetActive(true);
        anim.Play(animClip, 0, timeanim);
        if (desactive)
        {
            StartCoroutine(WaitForDesactive(anim.gameObject, anim.GetCurrentAnimatorStateInfo(0).length));
        }
    }

    private IEnumerator WaitActiveInArray(GameObject[] obj, int index, float time)
    {
        yield return StartCoroutine(Speedbox.bl_Utils.CorrutinesUtils.WaitForRealSeconds(time));
        foreach (GameObject o in obj) { o.SetActive(false); }
        obj[index].SetActive(true);
    }

    IEnumerator WaitForShowGOUI()
    {
        yield return new WaitForSeconds(1.5f);
        MenuAnim.gameObject.SetActive(true);
        MenuAnim.SetBool("show", true);
    }

    IEnumerator WaitForDesactive(GameObject obj,float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
    
    void SetupScore()
    {
        ScoreUI.SetActive(false);
        ScoreText.gameObject.SetActive(false);

        InstructionText.canvasRenderer.SetAlpha(0);
        if (!Speedbox.bl_Utils.IsMobile)
        {
            InstructionText.text = "Press arrow left to turn left and right to turn right";
            TapInitText.text = "SPACE TO PLAY";
            FlipText.text = "SPACE\n[FLIP]";
        }
    }

    void OnPoint(Arian.bl_GlobalEvents.OnPoint e)
    {
        ScoreText.text = e.Point.ToString();
        PointScoreAnim.Play("Point", 0, 0);
    }

    void HideFirstText()
    {
        InstructionText.CrossFadeColor(new Color(0, 0, 0, 0), 1, true, true);
    }

    public void SetAudioEnable()
    {
        audioEnable = !audioEnable;
        AudioListener.pause = !audioEnable;
        AudioEnableText.text = (audioEnable) ? "ENABLE" : "DISABLE";
        Speedbox.bl_Utils.PlayerPrefsX.SetBool(KeyMasters.Sound, audioEnable);
    }

    public void SetVolume(float v)
    {
        AudioListener.volume = v;
        PlayerPrefs.SetFloat(KeyMasters.Volume, v);
    }

    void LoadPrefs()
    {
        audioEnable = Speedbox.bl_Utils.PlayerPrefsX.GetBool(KeyMasters.Sound, true);
        AudioListener.pause = !audioEnable;
        AudioEnableText.text = (audioEnable) ? "ENABLE" : "DISABLE";

        quality = Speedbox.bl_Utils.PlayerPrefsX.GetBool(KeyMasters.Quality, false);
        QualityText.text = (quality) ? "LOW" : "GOOD";

        float v = PlayerPrefs.GetFloat(KeyMasters.Volume, 1);
        SetVolume(v);
        VolumeSlider.value = v;
    }
}