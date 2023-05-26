using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector3 _playerSpawn;
    [SerializeField] private Vector3 _playerSpawnEuler;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _playerRef;

    [SerializeField] private List<Vector3> _enemySpawns;
    [SerializeField] private List<GameObject> _enemies;
    [SerializeField] private GameObject _enemyRef;

    [SerializeField] private LightManager _lightManager;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _lightManager = FindObjectOfType<LightManager>();
        SetPlayer();
        SetEnemies();
    }

    private void SetPlayer() //Sets up the player tracking for the game manager
    {
        _player = GameObject.Find("Player");
        _playerSpawnEuler = _player.transform.rotation.eulerAngles;
        _playerSpawn = _player.transform.position;
    }

    private void SetEnemies() //sets up the Enemy tracking for the game manager
    {
        var enemies = FindObjectsOfType<EnemyStateMachine>().ToList();
        foreach (var enemy in enemies)
        {
            _enemies.Add(enemy.transform.gameObject);
            _enemySpawns.Add(enemy.transform.position);
        }
    }

    void Update()
    {
        if(_player.GetComponent<PlayerStateMachine>().Health <= 0) //if the player has 0 health, run the respawn method.
            Respawn();

        if (Input.GetKeyDown(KeyCode.Escape)) //return to the main menu if the player presses Escape
            SceneManager.LoadScene(0);
    }

    private void Respawn() //Delete the player and all enemies.  Then re-initialize all characters at their spawn points. 
    {
        for (var i = 0; i < _enemies.Count; i++)
        {
            Destroy(_enemies[i]);
            _enemies.RemoveAt(i);
        }
        Destroy(_player);

        _player = Instantiate(_playerRef, _playerSpawn, Quaternion.Euler(_playerSpawnEuler), null);
        _player.name = "Player";

        for (var i = 0; i < _enemySpawns.Count; i++)
        {
            var enemy = Instantiate(_enemyRef, _enemySpawns[i], Quaternion.identity, null);
            enemy.name = $"Enemy{i}";
            _enemies.Add(enemy);
        }
        _lightManager.SetList(); //reset the light manager character list. Enables light manager tracking for characters.
    }

    private void OnTriggerEnter(Collider other) //if the player enters the end collider. return to main menu. 
    {
        if (other.tag == "Player")
            SceneManager.LoadScene(0);
    }
}
