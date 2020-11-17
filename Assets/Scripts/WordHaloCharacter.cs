using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordHaloCharacter : MonoBehaviour
{
    public Vector3 localCirclePos;
    public Vector3 cameraCirclePos;
    public Quaternion localCircleRot;
    public Quaternion cameraCircleRot;
    public Vector3 localFlatPos;
    public Vector3 localFlatPosTransformed;
    public Quaternion localFlatRot;


    public void AnimateExplode(Vector3 goalPos, float speed, AnimationCurve curve)
    {
        StartCoroutine(MoveToGoal(this.cameraCirclePos, goalPos,
                       localCircleRot, localFlatRot, speed, curve, true));
    }

    public void AnimateRetract(float speed, AnimationCurve curve)
    {

        StartCoroutine(MoveToGoal(localFlatPosTransformed, localCirclePos,
                       localFlatRot, localCircleRot, speed, curve, false));
    }

    public IEnumerator MoveToGoal(Vector3 startPos, Vector3 endPos,
        Quaternion startRot, Quaternion endRot, 
        float speed, AnimationCurve curve, bool faceCamera)
    {
        float elapsed = 0f;
        float dist = Vector3.Distance(startPos, endPos);
        float t = 0f;

        while (elapsed < 1f)
        {
            t = curve.Evaluate(elapsed);
            this.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            if (!faceCamera)
                this.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            else
                this.transform.LookAt(Camera.main.transform, Camera.main.transform.up);

            elapsed += (speed * Time.deltaTime) / dist;
            yield return null;
        }
    }
}

