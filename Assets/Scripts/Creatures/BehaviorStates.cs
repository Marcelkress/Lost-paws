using UnityEngine;

public class BehaviorStates : MonoBehaviour
{
    public float wakeUpRange;

    public enum State
    {
        Passive,
        Aggro,
        Patrol,
        Dead
    }

    public State currentState;
    public bool dead;

    void Start()
    {
        currentState = State.Passive;
    }

    void Update()
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

        EvaluateStates();
        UpdateAnimations();
    }

    public virtual void UpdateAnimations() { }
    public virtual void EvaluateStates() { }
    public virtual void Aggro() { }

    public virtual void Passive() { }
    
    public virtual void Patrol() { }

    public virtual void Dead() { }
  
}
