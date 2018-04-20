using System.Collections;
using UnityEngine;

public class CameraController: MonoBehaviour {

    enum CameraMode {
        None,
        Overview,
        Announcer,
        Match,
        Bots,
    }

    [Header("Camera Config")]
    [Tooltip("minimum height of camera from arena floor")]
    public float minCameraHeight = 15f;
    [Tooltip("attached camera, leave null for autodiscovery")]
    public Camera camera;
    [Tooltip("approximate time for the camera to refocus")]
    public float dampTime = 0.075f;
    [Tooltip("approximate center of the arena")]
    public Vector3 arenaCenter = Vector3.zero;
    [Tooltip("to avoid direct overhead camera shots, how far from center should we shift")]
    public float cameraOffset = 5f;
    [Tooltip("overview position")]
    public Vector3 overviewPosition = new Vector3(0,40,-30);

    [Header("State Variables")]
    public BotRuntimeSet currentBots;

    // indicates the desired position and rotation of the camera
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    private CameraMode cameraMode = CameraMode.None;

    void Start() {
        desiredPosition = transform.position;
        desiredRotation = transform.rotation;

        // if camera has not been assigned in editor, find attached camera
        if (camera == null) {
            camera = GetComponentInChildren<Camera>();
        }
    }
    public void WatchStop() {
        cameraMode = CameraMode.None;
    }

    public void WatchOverview() {
        if (cameraMode != CameraMode.Overview) {
            cameraMode = CameraMode.Overview;
            StartCoroutine(WatchOverviewLoop());
        }
    }
    IEnumerator WatchOverviewLoop() {
        // set desired position/rotation to overview, looking at center of arena
        desiredPosition = overviewPosition;
        desiredRotation = Quaternion.LookRotation(arenaCenter - transform.position);
        while (cameraMode == CameraMode.Overview) {
            yield return null;
        }
    }

    public void WatchAnnouncer() {
        if (cameraMode != CameraMode.Announcer) {
            cameraMode = CameraMode.Announcer;
            StartCoroutine(WatchAnnouncerLoop());
        }
    }
    IEnumerator WatchAnnouncerLoop() {
        while (cameraMode == CameraMode.Announcer) {
            yield return null;
        }
    }

    public void WatchBots() {
        if (cameraMode != CameraMode.Bots) {
            cameraMode = CameraMode.Bots;
            StartCoroutine(WatchBotsLoop());
        }
    }
    IEnumerator WatchBotsLoop() {
        while (cameraMode == CameraMode.Bots) {
            if (currentBots != null) {
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
                Vector3 average = Vector3.zero;
                // determine min/max/average positions of bots
                for (var i=0; i<currentBots.Items.Count; i++) {
                    if (currentBots.Items[i] == null) continue;
                    average += currentBots.Items[i].transform.position;
                    bounds.Encapsulate(currentBots.Items[i].transform.position);
                }
                average /= currentBots.Items.Count;
                // calculate overhead position
                var boundsMax = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
                var overheadPosition = new Vector3(average.x, bounds.center.y + (boundsMax*3f), average.z);

                var overheadCenter = new Vector3(arenaCenter.x, overheadPosition.y, arenaCenter.z);
                var offset = overheadPosition - overheadCenter;
                // if overhead position is far enough away from center...
                // calculate camera offset, so we are not staring directly down on the action
                if (offset.magnitude > 1f) {
                    offset = Vector3.ClampMagnitude(offset, cameraOffset);

                // we are too close to center and run the risk of weird rotations if we just calculate offset from straight center
                } else {
                    // compute new canned overhead center
                    var overheadCenterOffset = new Vector3(arenaCenter.x-1f, overheadPosition.y, arenaCenter.z-1f);
                    offset = Vector3.ClampMagnitude(overheadPosition - overheadCenterOffset, cameraOffset);
                }

                // desired position is overhead - offset
                desiredPosition = overheadPosition - offset;
                // set desired rotation: focus to center of battle
                desiredRotation = Quaternion.LookRotation(average - transform.position);
            }
            // update 10 time a second vs every frame
            yield return new WaitForSeconds(.1f);
        }
    }

    void Move() {
        Vector3 moveVelocity = Vector3.zero;
        // clamp height
        desiredPosition.y = Mathf.Max(desiredPosition.y, minCameraHeight);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
    }

    void Rotate() {
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, dampTime);
    }

    void LateUpdate() {
        Move();
        Rotate();
    }

}