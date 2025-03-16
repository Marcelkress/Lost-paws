using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
public class CatInput : MonoBehaviour
{
    public Movement movement;
    
    public Vector2 moveVector;
    public bool jump;
    public bool sprint;

    void Start()
    {
        movement = GetComponent<Movement>();
    }
    
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }
    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
        Debug.Log(("Sprint" + newSprintState));
    }
    public void MoveInput(Vector2 newMoveDirection)
    {
        moveVector = newMoveDirection;
        Debug.Log(("move" + newMoveDirection));
    } 
    
    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
        Debug.Log(("jump" + newJumpState));
    }
}