using UnityEngine;

public class CreatureBehavior : MonoBehaviour
{
    public float detectRange;
    public LayerMask playerLayer;
    [Range(0,2)] public float easeAmount;

    protected Vector3 velocity;
    protected bool facingLeft;
    protected Vector3 originalScale;
    
    public enum State
    {
        Passive,
        Aggro,
        Patrol,
        Dead
    }

    public State currentState;

    public virtual void Start()
    {
        Debug.Log("Parent");
        currentState = State.Passive;
        originalScale = transform.localScale;
        facingLeft = false;
    }

    public virtual void Update()
    {
        switch (currentState)
        {
            case State.Aggro:
                Aggro(); break;
            case State.Passive:
                Passive(); break;
            case State.Patrol:
                Patrol(); break;
            case State.Dead:
                Dead(); break;
        }
        
        SwitchSide();
    }
    
    protected bool CheckForProximity(float dist, ref Collider2D hit)
    {
        hit = Physics2D.OverlapCircle(new(transform.position.x, transform.position.y), dist, playerLayer);
        
        if (hit)
        {
            return true;
        }

        return false;
    }

    private void SwitchSide()
    {
        Debug.Log("Velocity: " + velocity.x);
        Debug.Log("FacingLeft: " + facingLeft);

        if (velocity.x < 0 && !facingLeft)
        {
            facingLeft = true;
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            Debug.Log("Switched to facingLeft");
        }
        else if (velocity.x > 0 && facingLeft)
        {
            facingLeft = false;
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            Debug.Log("Switched from facingLeft");
        }
    }

    
    public virtual void Aggro() { }
    public virtual void Passive() { }
    public virtual void Patrol() { }
    public virtual void Dead() { }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new(transform.position.x, transform.position.y), detectRange);
    }
    
    public float Ease(float x)
    {
        float a = easeAmount + 1;

        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

}