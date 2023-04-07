using UnityEngine;

public class RangedBolt : MonoBehaviour
{
    [Range(10,50)][SerializeField]private float _launchSpeed;
    [SerializeField] private float _destroyTimer;

    [SerializeField] private const int DESTROYTIME = 4;

    [SerializeField] private int _damageValue;
    // Start is called before the first frame update
    private void Start()
    {
        _damageValue = 10;
    }

    // Update is called once per frame
    private void Update()
    {
        MoveBolt();
        Timer();
        if (_destroyTimer >= DESTROYTIME)
            DestroyBolt();
    }
    private void MoveBolt()
    {
        var moveVector = new Vector3(0, 0, _launchSpeed * Time.deltaTime);
        transform.position += transform.TransformVector(moveVector);
        transform.Rotate(Vector3.forward,10f * Time.deltaTime);
    }

    private void Timer()
    {
        _destroyTimer += 1f * Time.deltaTime;
    }

    private void DestroyBolt()
    {
            Destroy(this.gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            DestroyBolt();
            return;
        }
        Debug.Log(other.gameObject);
        CheckForDamageable(other.gameObject);
    }

    private void CheckForDamageable(GameObject other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        Debug.Log(damageable);
        if (damageable != null)
        {
            Debug.Log("Handling Damage");
            damageable.HandleDamage(_damageValue);
        }
    }
}
