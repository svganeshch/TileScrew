using UnityEngine;
using UnityEngine.UIElements;

public class GameScreenUIEvents : MonoBehaviour
{
    private UIDocument document;

    private Button nextButton;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        nextButton = document.rootVisualElement.Q("NextButton") as Button;
        nextButton.RegisterCallback<ClickEvent>(OnNextButtonClick);
    }

    private void OnNextButtonClick(ClickEvent clickEvent)
    {
        GameManager.Instance.NextLevel();
    }
}
