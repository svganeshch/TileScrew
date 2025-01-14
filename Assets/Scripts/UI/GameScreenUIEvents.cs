using UnityEngine;
using UnityEngine.UIElements;

public class GameScreenUIEvents : MonoBehaviour
{
    private UIDocument document;

    private Button nextButton;
    private Label fps;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        nextButton = document.rootVisualElement.Q("NextButton") as Button;
        nextButton.RegisterCallback<ClickEvent>(OnNextButtonClick);

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
}
