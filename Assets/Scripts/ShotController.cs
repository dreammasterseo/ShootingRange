using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShotController : MonoBehaviour {

    public float sensitivity = 5F;
    public float smoothing = 2f;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;

    public float rechargeTime;
    public GameObject pistol;
    public RectTransform rechargeBar;
    public Text score;

    private float rotationX = 0F;
    private float rotationY = 0F;
    private Quaternion originalRotation;

    private Ray ray;
    private RaycastHit hit;
    private bool mayShoot = true;
    private float rechargeSpeed; // pixels per second
    private RectTransform rectTransform;

    private Animator pistolAnimator;
    private AudioSource pistolShotSound;

    void Start()
    {
        Cursor.visible = false;
        originalRotation = transform.localRotation; 

        pistolAnimator = pistol.GetComponent<Animator>();
        pistolShotSound = pistol.GetComponent<AudioSource>();

        rectTransform = rechargeBar.GetComponent<RectTransform>();
        rechargeSpeed = rectTransform.sizeDelta.x / rechargeTime;
    }

    void Update ()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        Quaternion quaternionX = Quaternion.AngleAxis(rotationX, Vector3.up);
        Quaternion quaternionY = Quaternion.AngleAxis(rotationY, -Vector3.right);
        Quaternion quaternionXY = originalRotation * quaternionX * quaternionY;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, quaternionXY, Time.deltaTime * smoothing);

        if (!mayShoot)
        {
            if (rectTransform.sizeDelta.x <= 220)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + rechargeSpeed * Time.deltaTime, rectTransform.sizeDelta.y);
            } else
            {
                mayShoot = true;
            }
        }
    }

    void FixedUpdate () {
        if (Input.GetMouseButtonDown(0) && mayShoot)
        {
            // Выстрел
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            // Перезарядка
            rectTransform.sizeDelta = new Vector2(0f, rectTransform.sizeDelta.y);
            mayShoot = false;

            if (Physics.Raycast(ray, out hit))
            {
                // Если попали в цель       
                if (hit.collider.CompareTag("Aim"))
                {
                    Aim aim = hit.collider.GetComponent<Aim>();
                    // Применение силы к цели, когда попали в нее
                    // hit.collider.GetComponent<Rigidbody>().AddForce(Vector3.forward * 500);

                    // Эффект попадания

                    // Начисление очков
                    score.text =  (int.Parse(score.text) + aim.points).ToString();
                    // Подсчет количества оставшихся целей
                    aim.SubAimCount();
                    // Разрушение цели
                    Destroy(hit.collider.gameObject);
                }
            }

            // Эффекты
            pistolAnimator.SetTrigger("shot");
            pistolShotSound.Play();
        }
    }
}