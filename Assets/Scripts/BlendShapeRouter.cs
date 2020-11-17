using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


[RequireComponent(typeof(ARFace))]
public class BlendShapeRouter : MonoBehaviour
{
    public Text BrowDownLeftText, BrowDownRightText, BrowOuterUpLeftText, BrowOuterUpRightText, BrowInnerUpText;
    public float BrowCoefficient = 0f;
    ARFace m_Face;

    ARKitFaceSubsystem m_ARKitFaceSubsystem;


    void Awake()
    {
        m_Face = GetComponent<ARFace>();
    }

    void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();
        if (faceManager != null)
        {
            m_ARKitFaceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
        }

        m_Face.updated += OnUpdated;
    }

    void OnDisable()
    {
        m_Face.updated -= OnUpdated;
    }

    void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
        UpdateFaceFeatures();
    }

    void UpdateFaceFeatures()
    {
        using (var blendShapes = m_ARKitFaceSubsystem.GetBlendShapeCoefficients(m_Face.trackableId, Unity.Collections.Allocator.Temp))
        {
            
            foreach (var featureCoefficient in blendShapes)
            {
                if (featureCoefficient.blendShapeLocation == ARKitBlendShapeLocation.BrowInnerUp)
                {
                    this.BrowCoefficient = featureCoefficient.coefficient * 100f;
                }
            }
        }
    }
}
