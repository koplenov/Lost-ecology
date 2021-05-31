using UnityEngine;

public class WasteOfType : MonoBehaviour
{
    
    public enum Types
    {
        Food,
        BrokenBottle,
        Packet,
        Cover,
    }

    public Types Type;

    public SpriteRenderer spriteRenderer;
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetActiveSprite()
    {
        spriteRenderer.sprite = activeSprite;
    }
    public void SetInactiveSprite()
    {
        spriteRenderer.sprite = inactiveSprite;
    }
}
