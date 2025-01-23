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
        SetRandomBG(0);
    }

    public void SetRandomBG(int temp)
    {
        var randomBgSprite = bgSprites[Random.Range(0, bgSprites.Length)];

        spriteRenderer.sprite = randomBgSprite;
    }
}
