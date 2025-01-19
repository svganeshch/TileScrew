using UnityEngine;
using UnityEngine.UIElements;

public class LevelDoneScreenUIEvents : MonoBehaviour
{
    private UIDocument document;

    Button nextButton;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        if (!document.enabled) document.enabled = true;

        nextButton = document.rootVisualElement.Q("NextButton") as Button;
        nextButton.RegisterCallback<ClickEvent>(OnNextButton);

        document.rootVisualElement.style.visibility = Visibility.Hidden;
    }

    private void OnNextButton(ClickEvent clickEvent)
    {
        StartCoroutine(GameManager.Instance.LoadNextLevel());
        document.rootVisualElement.style.visibility = Visibility.Hidden;
    }
}
