using UnityEngine;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction;
using Oculus.Interaction.Grab;
using Oculus.Interaction.Input;

public class GrabDetector : MonoBehaviour
{
    [SerializeField] private HandGrabInteractor handGrabInteractor;
    [SerializeField] private HandGrabInteractable handGrabInteractable;

    private const string GRABBABLE_TAG = "Grabbable";

    // serial Manager connected to the bluetooth gloves
    private SerialManager _serialManager;
    private bool isGrabbing = false;
    private GrabTypeFlags currentGrabType = GrabTypeFlags.None;
    private GameObject currentlyGrabbedObject = null;
    private HandFingerFlags activeGrabbingFingers = HandFingerFlags.None;

    void Start()
    {
        if (handGrabInteractable == null)
        {
            handGrabInteractable = GetComponent<HandGrabInteractable>();
        }

        _serialManager = FindObjectOfType<SerialManager>();
        if (_serialManager == null)
        {
            Debug.LogError("SerialManager not found!");
        }
    }

    void Update()
    {
        if (_serialManager == null) return;

        GameObject targetObject = handGrabInteractor?.SelectedInteractable?.gameObject;

        if (targetObject != null && targetObject.CompareTag(GRABBABLE_TAG))
        {
            float grabScore = HandGrabInteraction.ComputeHandGrabScore(
                handGrabInteractor,
                handGrabInteractable,
                out GrabTypeFlags grabTypes
            );

            HandFingerFlags grabbingFingers = HandGrabInteraction.GrabbingFingers(
                handGrabInteractor,
                handGrabInteractable
            );

            GrabTypeFlags shouldSelect = HandGrabInteraction.ComputeShouldSelect(
                handGrabInteractor,
                handGrabInteractable
            );

            if (!isGrabbing && shouldSelect != GrabTypeFlags.None)
            {
                HandleGrabStart(shouldSelect, targetObject, grabbingFingers);
            }
            else if (isGrabbing && grabbingFingers != activeGrabbingFingers)
            {
                UpdateGrabbingFingers(grabbingFingers);
            }

            GrabTypeFlags shouldUnselect = HandGrabInteraction.ComputeShouldUnselect(
                handGrabInteractor,
                handGrabInteractable
            );

            if (isGrabbing && shouldUnselect != GrabTypeFlags.None)
            {
                HandleGrabEnd();
            }
        }
        else if (isGrabbing)
        {
            HandleGrabEnd();
        }
    }

    private void HandleGrabStart(GrabTypeFlags grabType, GameObject grabbedObject, HandFingerFlags grabbingFingers)
    {
        isGrabbing = true;
        currentGrabType = grabType;
        currentlyGrabbedObject = grabbedObject;
        activeGrabbingFingers = grabbingFingers;

        // Send both haptic and brake commands for the grabbing fingers
        ActivateFingersWithFeedback(grabbingFingers);

        string fingersList = GetActiveFingerNames(grabbingFingers);
        Debug.Log($"Grab detected on {grabbedObject.name} using fingers: {fingersList}");
    }

    private void UpdateGrabbingFingers(HandFingerFlags newGrabbingFingers)
    {
        // Handle fingers that are no longer grabbing
        HandFingerFlags releasedFingers = activeGrabbingFingers & ~newGrabbingFingers;
        if (releasedFingers != HandFingerFlags.None)
        {
            ReleaseFingersWithFeedback(releasedFingers);
        }

        // Handle new grabbing fingers
        HandFingerFlags newFingers = newGrabbingFingers & ~activeGrabbingFingers;
        if (newFingers != HandFingerFlags.None)
        {
            ActivateFingersWithFeedback(newFingers);
        }

        activeGrabbingFingers = newGrabbingFingers;
    }

    private void HandleGrabEnd()
    {
        if (isGrabbing)
        {
            Debug.Log($"Released grab on {currentlyGrabbedObject?.name}");
            ReleaseFingersWithFeedback(activeGrabbingFingers);

            isGrabbing = false;
            currentGrabType = GrabTypeFlags.None;
            currentlyGrabbedObject = null;
            activeGrabbingFingers = HandFingerFlags.None;
        }
    }

    private void ActivateFingersWithFeedback(HandFingerFlags fingers)
    {
        // First send haptic feedback for the grabbing fingers then force feedbacj per finger
        if ((fingers & HandFingerFlags.Thumb) != 0)
        {
            _serialManager.SendCommand("thumb_haptic\n");
            _serialManager.SendCommand("brake_thumb\n");
        }
        if ((fingers & HandFingerFlags.Index) != 0)
        {
            _serialManager.SendCommand("index_haptic\n");
            _serialManager.SendCommand("brake_index\n");
        }
        if ((fingers & HandFingerFlags.Middle) != 0)
        {
            _serialManager.SendCommand("middle_haptic\n");
            _serialManager.SendCommand("brake_middle\n");
        }
        if ((fingers & HandFingerFlags.Ring) != 0)
        {
            _serialManager.SendCommand("ring_haptic\n");
            _serialManager.SendCommand("brake_ring\n");
        }
        if ((fingers & HandFingerFlags.Pinky) != 0)
        {
            _serialManager.SendCommand("pinky_haptic\n");
            _serialManager.SendCommand("brake_pinky\n");
        }

        // Also sending the general buzz for overall feedback
        _serialManager.SendCommand("buzz\n");
    }

    private void ReleaseFingersWithFeedback(HandFingerFlags fingers)
    {
        _serialManager.SendCommand("release\n");
    }

    private string GetActiveFingerNames(HandFingerFlags fingers)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if ((fingers & HandFingerFlags.Thumb) != 0) sb.Append("Thumb ");
        if ((fingers & HandFingerFlags.Index) != 0) sb.Append("Index ");
        if ((fingers & HandFingerFlags.Middle) != 0) sb.Append("Middle ");
        if ((fingers & HandFingerFlags.Ring) != 0) sb.Append("Ring ");
        if ((fingers & HandFingerFlags.Pinky) != 0) sb.Append("Pinky ");
        return sb.ToString().Trim();
    }

    private void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.Label($"Grabbing: {isGrabbing}");
            GUILayout.Label($"Grab Type: {currentGrabType}");
            if (currentlyGrabbedObject != null)
            {
                GUILayout.Label($"Grabbed Object: {currentlyGrabbedObject.name}");
                GUILayout.Label($"Active Fingers: {GetActiveFingerNames(activeGrabbingFingers)}");
            }
            GUILayout.EndArea();
        }
    }
}