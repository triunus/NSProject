using UnityEngine;

namespace GameSystem.Camera
{
    public interface ICameraTracking
    {
        public Vector2 LimitedBackGroundCenter { get; }
        public Vector2 LimitedBackGroundSize { get; }
    }

    public class CameraTracking : MonoBehaviour, ICameraTracking
    {
        private Transform cameraTransform;
        private Transform playerTransform;

        [SerializeField]
        private float cameraSpeed = 3f;
        [SerializeField]
        private Vector2 limitedBackGroundCenter;
        [SerializeField]
        private Vector2 limitedBackGroundSize;

        private float height;
        private float width;

        public Vector2 LimitedBackGroundCenter { get { return this.limitedBackGroundCenter; } }
        public Vector2 LimitedBackGroundSize { get { return this.limitedBackGroundSize; } }

        private void Awake()
        {
            this.cameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();

            this.height = UnityEngine.Camera.main.orthographicSize;
            this.width = height * Screen.width / Screen.height;

/*            Debug.Log("Screen.width : " + Screen.width + " ||| Screen.height : " + Screen.height);
            Debug.Log("height : " + height + " ||| width : " + width);*/
        }

        private void Start()
        {
            this.playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(limitedBackGroundCenter, limitedBackGroundSize);
        }

        private void LateUpdate()
        {
            if (GameManager.Instance.IsInGame)
            {
                this.cameraTransform.position = this.playerTransform.position;

                float coordinateX = Mathf.Clamp(this.cameraTransform.position.x,
                    this.limitedBackGroundCenter.x - this.limitedBackGroundSize.x / 2 + this.width,
                    this.limitedBackGroundCenter.x + this.limitedBackGroundSize.x / 2 - this.width);

                float coordinateY = Mathf.Clamp(this.cameraTransform.position.y,
                    this.limitedBackGroundCenter.y - this.limitedBackGroundSize.y / 2 + this.height,
                    this.limitedBackGroundCenter.y + this.limitedBackGroundSize.y / 2 - this.height);

                this.cameraTransform.position = new Vector3(coordinateX, coordinateY, -10f);
            }
        }
    }
}
