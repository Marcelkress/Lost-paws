using UnityEngine;

public class UnlockWallClimb : MonoBehaviour, IInteractable
{
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Interact(Transform player)
    {
        // play animation 
        sprite.color = Color.gray;
        player.GetComponent<CatInput>().canWallJump = true;
    }
}
