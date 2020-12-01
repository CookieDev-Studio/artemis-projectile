using UnityEngine;

//Intended for demo purposes only.
public sealed class PlayerController : MonoBehaviour
{
    public GameObject head;
    public GameObject body;
    public GameObject BulletPrefab;

    [SerializeField] private float speed;
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private float sensitivity;
    [SerializeField] private float zoomLevel = 2;

    private float standardSpeed;

    private Rigidbody rigidBody;
    private Vector3 rotation;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        rotation = transform.eulerAngles;

        standardSpeed = speed;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            SpawnBullet();

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Camera.main.fieldOfView /= zoomLevel;
            sensitivity /= zoomLevel;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Camera.main.fieldOfView *= zoomLevel;
            sensitivity *= zoomLevel;
        }
        if (Input.GetKey(KeyCode.Space))
            Time.timeScale = 0.01f;
        else
            Time.timeScale = 1;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = standardSpeed * speedMultiplier;
        else
            speed = standardSpeed;

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            movement += Vector3.forward;
        if (Input.GetKey(KeyCode.A))
            movement += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            movement += Vector3.right;
        if (Input.GetKey(KeyCode.S))
            movement += Vector3.back;

        rigidBody.MovePosition(rigidBody.position + transform.TransformDirection(movement.normalized) * speed * Time.deltaTime);
    }

    void LateUpdate()
    {
        //camera rotation
        rotation.x -= Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        rotation.y += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        rotation.x = Mathf.Clamp(rotation.x, -90, 90);
        head.transform.eulerAngles = new Vector3(rotation.x, head.transform.eulerAngles.y, 0);
        transform.eulerAngles = new Vector3(0, rotation.y, 0);
    }

    void SpawnBullet()
    {
        GameObject bullet = Instantiate(BulletPrefab, head.transform.position + head.transform.forward, head.transform.rotation * Quaternion.identity);
        Destroy(bullet, 3);
    }
}