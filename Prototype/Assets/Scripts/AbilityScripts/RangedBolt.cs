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


    //If there is no hit returned on the bolt, Destroy the game object when the destroy timer has been reached.
    private void Update()
    {
        MoveBolt();
        Timer();
        if (_destroyTimer >= DESTROYTIME)
            DestroyBolt();
    }
    private void MoveBolt() //Moves to the current forward transform.
    {
        var moveVector = new Vector3(0, 0, _launchSpeed * Time.deltaTime);
        transform.position += transform.TransformVector(moveVector);
        transform.Rotate(Vector3.forward,10f * Time.deltaTime);
    }

    private void Timer() //Update timer
    {
        _destroyTimer += 1f * Time.deltaTime;
    }

    private void DestroyBolt() //Destroy this object.
    {
            Destroy(this.gameObject);
    }

    //Check if the bolt has hit the player or anything else. 
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

    //Checks for a damageable component on the object it has hit. If an component is returned then the object runs it's own HandleDamage function. Use of IDamageable interface.
    private void CheckForDamageable(GameObject other) 
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        Debug.Log(damageable);
        if (damageable != null)
        {
            Debug.Log("Handling Damage");
            damageable.HandleDamage(_damageValue);
        }
        DestroyBolt();
    }
}
