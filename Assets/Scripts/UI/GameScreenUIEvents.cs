using UnityEngine;
using UnityEngine.UIElements;

public class GameScreenUIEvents : MonoBehaviour
{
    private UIDocument document;

    private Button nextButton;
    private Button magnetButton;
    private Button shuffleButton;
    private Button undoButton;
    private Button drillSlotButton;

    private Label level;
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

        undoButton = document.rootVisualElement.Q("UndoButton") as Button;
        undoButton.RegisterCallback<ClickEvent>(OnUndoButtonClick);

        drillSlotButton = document.rootVisualElement.Q("DrillSlotButton") as Button;
        drillSlotButton.RegisterCallback<ClickEvent>(OnDrillSlotButtonClick);

        level = document.rootVisualElement.Q("level") as Label;
        fps = document.rootVisualElement.Q("fps") as Label;

        UIManager.Instance.onLevelChangeEvent.AddListener(SetLevelText);
    }

    private void Update()
    {
        fps.text = FPSCounter.Instance.smoothFps.ToString("F2");
    }

    public void SetLevelText(int levelNum)
    {
        level.text = levelNum.ToString();
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
        BoosterManager.Instance.ShuffleLevelBooster();
    }

    private void OnUndoButtonClick(ClickEvent clickEvent)
    {
        BoosterManager.Instance.Undo();
    }

    private void OnDrillSlotButtonClick(ClickEvent clickEvent)
    {
        BoosterManager.Instance.DrillExtraSlot();
    }
}
