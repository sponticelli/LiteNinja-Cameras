using UnityEngine;

namespace LiteNinja.Cameras
{
    public class CameraTrackTargets : MonoBehaviour
    {
        public GameObject[] targets;
        [SerializeField] private float boundingBoxPadding = 2f;

        [Header("Runtime Update Params")] [SerializeField]
        private bool realTimeUpdate;

        [SerializeField] private float zoomSpeed = 20f;

        private Camera myCamera;

        public void SetTargets(GameObject[] transformTargets)
        {
            targets = transformTargets;
        }

        private void Awake()
        {
            myCamera = gameObject.GetComponent<Camera>();
            myCamera.orthographic = true;
        }

        public void UpdateCamera()
        {
            if (targets.Length == 0) return;
            var boundingBox = CalculateTargetsBoundingBox();
            transform.position = CalculateCameraPosition(boundingBox);
            myCamera.orthographicSize = CalculateOrthographicSize(boundingBox);
        }

        private void LateUpdate()
        {
            if (!realTimeUpdate) return;
            var boundingBox = CalculateTargetsBoundingBox();
            transform.position = CalculateCameraPosition(boundingBox);
            myCamera.orthographicSize = Mathf.Lerp(myCamera.orthographicSize, CalculateOrthographicSize(boundingBox),
                Time.deltaTime * zoomSpeed);
        }

        private Rect CalculateTargetsBoundingBox()
        {
            var minX = Mathf.Infinity;
            var maxX = Mathf.NegativeInfinity;
            var minY = Mathf.Infinity;
            var maxY = Mathf.NegativeInfinity;

            foreach (var target in targets)
            {
                var position = target.transform.position;

                minX = Mathf.Min(minX, position.x);
                minY = Mathf.Min(minY, position.y);
                maxX = Mathf.Max(maxX, position.x);
                maxY = Mathf.Max(maxY, position.y);
            }

            return Rect.MinMaxRect(minX - boundingBoxPadding,
                maxY + boundingBoxPadding,
                maxX + boundingBoxPadding,
                minY - boundingBoxPadding);
        }

        private static Vector3 CalculateCameraPosition(Rect boundingBox)
        {
            var boundingBoxCenter = boundingBox.center;
            return new Vector3(boundingBoxCenter.x, boundingBoxCenter.y, -10f);
        }

        private float CalculateOrthographicSize(Rect boundingBox)
        {
            float orthographicSize;
            var topRight = new Vector3(boundingBox.x + boundingBox.width, boundingBox.y, 0f);
            var v = myCamera.WorldToViewportPoint(topRight);

            if (v.x >= v.y)
                orthographicSize = Mathf.Abs(boundingBox.width) / myCamera.aspect / 2f;
            else
                orthographicSize = Mathf.Abs(boundingBox.height) / 2f;

            return orthographicSize;
        }
    }
    
    
}