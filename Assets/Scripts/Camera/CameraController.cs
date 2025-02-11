using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera Camera;
    public float WASDSpeed;
    public float BoundsOffset;

    private Vector3 dragOrigin;

    private void Awake()
    {
        G.CameraController = this;
        Camera = Camera.main;
    }

    private void Start()
    {
        var worldSize = G.Main.GameConfig.WorldSize;
        Camera.transform.position = new Vector3(worldSize.x / 2f, worldSize.y / 2f, -10);
    }

    private void Update()
    {
        //middle mouse movement
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Camera.ScreenToWorldPoint(Input.mousePosition);
            return;
        }
        if (Input.GetMouseButton(1))
        {
            var moveDeltaEnd = Camera.ScreenToWorldPoint(Input.mousePosition);
            MoveCamera(moveDeltaEnd);
            return;
        }

        //wasd movement
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");

        if (x != 0 || y != 0) MoveCamera(x, y);
    }
    private void ClampPosition()
    {
        var clampedPosition = Camera.transform.position;
        var worldSize = (Vector2)G.Main.GameConfig.WorldSize;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -BoundsOffset, worldSize.x + BoundsOffset);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -BoundsOffset, worldSize.y + BoundsOffset);

        Camera.transform.position = clampedPosition;
    }
    private void MoveCamera(Vector3 moveDeltaEnd)
    {
        var difference = dragOrigin - moveDeltaEnd;
        Camera.transform.position += difference;
        ClampPosition();
    }
    private void MoveCamera(float x, float y)
    {
        Camera.transform.position += new Vector3(x, y, 0).normalized * WASDSpeed * Time.deltaTime;
        ClampPosition();
    }
}
