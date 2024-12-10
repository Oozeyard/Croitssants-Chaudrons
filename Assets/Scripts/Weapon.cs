using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private const int VELOCITY_FRAME_COUNT = 10;
    private Vector3[] velocityFrames = new Vector3[VELOCITY_FRAME_COUNT];
    private int currentFrame = 0;

    public float baseDamage = 10;

    public void SetVelocity(Vector3 velocity) {
        velocityFrames[currentFrame] = velocity;
        currentFrame = (currentFrame + 1) % VELOCITY_FRAME_COUNT;
    }

    public Vector3 GetVelocity() {
        Vector3 velocity = Vector3.zero;
        for (int i = 0; i < VELOCITY_FRAME_COUNT; i++) {
            velocity += velocityFrames[i];
        }
        return velocity / VELOCITY_FRAME_COUNT;
    }

    public void clearVelocity() {
        for (int i = 0; i < VELOCITY_FRAME_COUNT; i++) {
            velocityFrames[i] = Vector3.zero;
        }
    }

    private void OnEnable() {
        clearVelocity();
    }

}
