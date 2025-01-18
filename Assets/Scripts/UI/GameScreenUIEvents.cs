using UnityEngine;
using UnityEngine.UIElements;

public class GameScreenUIEvents : MonoBehaviour
{
    private UIDocument document;

    private Button nextButton;
    private Button magnetButton;
    private Button shuffleButton;
    private Label fps;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        nextButton = document.rootVisualElement.Q("NextButton") as Button;
        nextButton.RegisterCallback<ClickEvent>(OnNextButtonClick);

        magnetButton = document.rootVisualElement.Q("MagnetButton") as Button;
        magnetButton.RegisterCallback<ClickEvent>(OnMagnetButtonClick);

        shuffleButton = document.rootVisualElement.Q("ShuffleButton") as Button;
        shuffleButton.RegisterCallback<ClickEvent>(OnShuffleButtonClick);

        fps = document.rootVisualElement.Q("fps") as Label;
    }

    private void Update()
    {
        fps.text = FPSCounter.Instance.smoothFps.ToString("F2");
    }

    private void OnNextButtonClick(ClickEvent clickEvent)
    {
        StartCoroutine(GameManager.Instance.LoadNextLevel());
    }

    private void OnMagnetButtonClick(ClickEvent clickEvent)
    {
        BoosterManager.Instance.MagnetBooster();
    }

    private void OnShuffleButtonClick(ClickEvent clickEvent)
    {
        StartCoroutine(BoosterManager.Instance.ShuffleLevelBooster());
    }
}
