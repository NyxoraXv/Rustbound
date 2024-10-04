using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    // RectTransforms for each button
    [SerializeField] private RectTransform play, or, quit, pivotButton;
    [SerializeField] private Transform player;

    // Variables to store default positions and rotations
    private Vector3 playDefaultPosition, orDefaultPosition, quitDefaultPosition;
    private Quaternion playDefaultRotation, orDefaultRotation, quitDefaultRotation;

    // Target positions and rotations (can be assigned from the Inspector)
    [Header("Play Button Target Values")]
    [SerializeField] private Vector3 playTargetPosition;
    [SerializeField] private Vector3 playTargetRotation;

    [Header("Or Button Target Values")]
    [SerializeField] private Vector3 orTargetPosition;
    [SerializeField] private Vector3 orTargetRotation;

    [Header("Quit Button Target Values")]
    [SerializeField] private Vector3 quitTargetPosition;
    [SerializeField] private Vector3 quitTargetRotation;

    private void Start()
    {

        playDefaultPosition = play.localPosition;
        orDefaultPosition = or.localPosition;
        quitDefaultPosition = quit.localPosition;

        playDefaultRotation = play.localRotation;
        orDefaultRotation = or.localRotation;
        quitDefaultRotation = quit.localRotation;
    }

    // Hover Enter: Play Button
    public void PlayHoverEnter()
    {
        DOTween.Kill(play);
        play.DOScale(1.1f, 0.3f);
        FadeInGlitch(play.GetComponent<Image>());
        Debug.Log("hovering enter");
        // Animate to new position and rotation assigned in the Inspector
        play.DOLocalMove(playTargetPosition, 0.3f);
        play.DOLocalRotate(playTargetRotation, 0.3f);
    }

    // Hover Exit: Play Button
    public void PlayHoverExit()
    {
        DOTween.Kill(play);
        play.DOScale(1f, 0.3f);
        FadeOutGlitch(play.GetComponent<Image>());
        // Return to default position and rotation
        play.DOLocalMove(playDefaultPosition, 0.3f);
        play.DOLocalRotate(playDefaultRotation.eulerAngles, 0.3f);
    }

    // Hover Enter: Or Button
    public void OrHoverEnter()
    {
        // Animate to new position and rotation assigned in the Inspector
        or.DOLocalMove(orTargetPosition, 0.3f);
        or.DOLocalRotate(orTargetRotation, 0.3f);
    }

    // Hover Exit: Or Button
    public void OrHoverExit()
    {
        // Return to default position and rotation
        or.DOLocalMove(orDefaultPosition, 0.3f);
        or.DOLocalRotate(orDefaultRotation.eulerAngles, 0.3f);
    }

    // Hover Enter: Quit Button
    public void QuitHoverEnter()
    {
        // Animate to new position and rotation assigned in the Inspector
        quit.DOLocalMove(quitTargetPosition, 0.3f);
        quit.DOLocalRotate(quitTargetRotation, 0.3f);
        FadeInGlitch(quit.GetComponent<Image>());
    }

    // Hover Exit: Quit Button
    public void QuitHoverExit()
    {
        // Return to default position and rotation
        quit.DOLocalMove(quitDefaultPosition, 0.3f);
        quit.DOLocalRotate(quitDefaultRotation.eulerAngles, 0.3f);
        FadeOutGlitch(quit.GetComponent<Image>());
    }

    // Placeholder actions for buttons

    public void PlayButtonDown()
    {
        DOTween.Kill(play);
        play.DOScale(0.9f, 0.3f);
    }

    public void PlayButtonUp()
    {
        DOTween.Kill(play);
        play.DOScale(1.1f, 0.3f);
    }

    public void PlayAction()
    {

        Animator ac = player.GetComponent<Animator>();
        ac.SetBool("jalankan", true);
        
        DOVirtual.DelayedCall(0.6f, () =>
        { player.DOMove(new Vector3(7.11899996f, -0.0399999991f, -8.77099991f), 5f); });
        DOVirtual.DelayedCall(1f, () =>
        {
            GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                Camera.main.GetComponent<MouseParallax>().enabled = false;
                Camera.main.transform.DOMoveY(-3f, 1f).SetEase(Ease.InOutCubic);
            }).OnStart(() =>
            {
                pivotButton.DORotate(new Vector3(6.56699991f, 0.075000003f, -8.6239996f), 1f).SetEase(Ease.InOutCubic).OnComplete(() => { DOVirtual.DelayedCall(1f, () => { SceneManager.LoadScene(1); }); });
            });
        });

        
    }

    public void OrAction()
    {
        // Action for or button
    }

    public void QuitAction()
    {
        // Action for quit button
    }


    public void FadeInGlitch(Image img)
    {
        Material mat = img.material;

        // Convert property name to ID within the function
        int glitchPropertyID = Shader.PropertyToID("_GlitchFade");

        mat.SetFloat(glitchPropertyID, 1f);
    }

    // Call this to fade out glitch effect
    public void FadeOutGlitch(Image img)
    {
        Material mat = img.material;

        // Convert property name to ID within the function
        int glitchPropertyID = Shader.PropertyToID("_GlitchFade");

        mat.SetFloat(glitchPropertyID, 0f);

    }
}
