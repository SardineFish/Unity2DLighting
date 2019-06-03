using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    public int targetFrameRate = 60;
    void Update()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}