using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Sprite[] bgSprites;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        
    }

    private void Start()
    {
        UIManager.Instance.onLevelChangeEvent.AddListener(SetRandomBG);
        SetRandomBG("");
    }

    public void SetRandomBG(string temp)
    {
        var randomBgSprite = bgSprites[Random.Range(0, bgSprites.Length)];

        spriteRenderer.sprite = randomBgSprite;
    }
}
