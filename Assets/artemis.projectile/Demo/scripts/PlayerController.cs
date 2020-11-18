using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject head;
    public GameObject body;
    public GameObject BulletPrefab;

    [SerializeField] private float jumpHight;
    [SerializeField] private float speed;
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private float sensitivity;
    [SerializeField] private float zoomLevel = 2;

    private float standardSpeed;

    private Rigidbody rigidBody;
    new private BoxCollider collider;
    private Vector3 rotation;
    private bool grounded;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        collider = body.GetComponent<BoxCollider>();

        rotation = transform.eulerAngles;

        standardSpeed = speed;

        Cursor.lockState = CursorLockMode.Locked;

        Destroy(head.GetComponent<MeshRenderer>());
        Destroy(head.GetComponent<MeshFilter>());

        Destroy(body.GetComponent<MeshRenderer>());
        Destroy(body.GetComponent<MeshFilter>());

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SpawnBullet();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Camera.main.fieldOfView /= zoomLevel;
            sensitivity /= zoomLevel;
            //Time.timeScale = 0.1f;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Camera.main.fieldOfView *= zoomLevel;
            sensitivity *= zoomLevel;
            //Time.timeScale = 1;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            speed = standardSpeed * sprintMultiplier;
        else
            speed = standardSpeed;

        if (
            Physics.BoxCast(
                body.transform.position,
                new Vector3(collider.bounds.extents.x, 0.1f, collider.bounds.extents.z),
                Vector3.down,
                Quaternion.identity,
                collider.bounds.extents.y,
                (1 << 9)))
        {
            grounded = true;
        }
        else
            grounded = false;

        //movement
        if (grounded)
        {
            Vector3 movement = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                movement += Vector3.forward;
            if (Input.GetKey(KeyCode.A))
                movement += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                movement += Vector3.right;
            if (Input.GetKey(KeyCode.S))
                movement += Vector3.back;

            rigidBody.MovePosition(rigidBody.position + transform.TransformDirection(movement.normalized) * speed * Time.fixedDeltaTime);

            //jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 velocity = Vector3.zero;

                if (Input.GetKey(KeyCode.W))
                    velocity += Vector3.forward;
                if (Input.GetKey(KeyCode.A))
                    velocity += Vector3.left;
                if (Input.GetKey(KeyCode.D))
                    velocity += Vector3.right;
                if (Input.GetKey(KeyCode.S))
                    velocity += Vector3.back;

                velocity = velocity.normalized * speed;
                velocity.y = jumpHight;

                rigidBody.velocity = transform.TransformDirection(velocity);
            }
        }
        else
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rigidBody.velocity);

            if (Input.GetKey(KeyCode.W) && localVelocity.z < standardSpeed)
                rigidBody.velocity += transform.TransformDirection(Vector3.forward) * standardSpeed * 2 * Time.deltaTime;
            if (Input.GetKey(KeyCode.A) && localVelocity.x > -standardSpeed)
                rigidBody.velocity += transform.TransformDirection(Vector3.left) * standardSpeed * 2 * Time.deltaTime;
            if (Input.GetKey(KeyCode.D) && localVelocity.x < standardSpeed)
                rigidBody.velocity += transform.TransformDirection(Vector3.right) * standardSpeed * 2 * Time.deltaTime;
            if (Input.GetKey(KeyCode.S) && localVelocity.z > -standardSpeed)
                rigidBody.velocity += transform.TransformDirection(Vector3.back) * standardSpeed * 2 * Time.deltaTime;
        }
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
        //GameObject bullet = Instantiate(BulletPrefab, head.transform.position + head.transform.forward, head.transform.rotation * Quaternion.Euler(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 1));
        GameObject bullet = Instantiate(BulletPrefab, head.transform.position + head.transform.forward, head.transform.rotation * Quaternion.identity);
        Destroy(bullet, 3);
    }
}