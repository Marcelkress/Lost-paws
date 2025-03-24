using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Movement target;
    public Vector2 foucesAreaSize;
    public float verticalOffset;
    public float lookAheadDistanceX, lookSmoothTimeX, lookSmoothTimeY;
    private FocusArea focusArea;

    private float currentLookAheadX, targetLookAheadX, lookAheadDirX, smoothLookVelocityX, smoothVelocityY;

    private bool lookAheadStopped;
    
    void Start()
    {
        focusArea = new FocusArea(target.collider2d.bounds, foucesAreaSize);
    }

    private void LateUpdate()
    {
        focusArea.Update(target.collider2d.bounds);

        Vector2 focusPosistion = focusArea.center + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDistanceX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistanceX - currentLookAheadX) / 4;
                    lookAheadStopped = true;
                }
            }
        }
        
        currentLookAheadX =
            Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosistion.y =
            Mathf.SmoothDamp(transform.position.y, focusPosistion.y, ref smoothVelocityY, lookSmoothTimeY);
        
        focusPosistion += Vector2.right * currentLookAheadX;
            
        transform.position = (Vector3)focusPosistion + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new(1, 0, 0, 0.5f);
        
        Gizmos.DrawCube(focusArea.center, foucesAreaSize);
    }

    struct FocusArea
    {
        public Vector2 center;
        private float left, right;
        private float top, bottom;
        public Vector2 velocity;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;

            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            
            velocity = Vector2.zero;
            center = new((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds) {
            float shiftX = 0;
            if (targetBounds.min.x < left) {
                shiftX = targetBounds.min.x - left;
            } else if (targetBounds.max.x > right) {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom) {
                shiftY = targetBounds.min.y - bottom;
            } else if (targetBounds.max.y > top) {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            center = new Vector2((left+right)/2,(top +bottom)/2);
            velocity = new Vector2 (shiftX, shiftY);
        }
    }
}
