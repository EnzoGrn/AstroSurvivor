using UnityEngine.SceneManagement;
using UnityEngine;

namespace AstroSurvivor {

    public class GameManager : MonoBehaviour {

        public static GameManager Instance;

        [Header("Player")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnPoint;

        [Header("UI")]
        [SerializeField] private DeadUI deadUI;
        [SerializeField] private StartUI startUI;

        private GameObject currentPlayer;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            startUI.OnGameStarted += SpawnPlayer;

            startUI.Show();
            deadUI.Hide();
        }

        public void SpawnPlayer()
        {
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            PlayerStats stats = currentPlayer.GetComponent<PlayerStats>();

            stats.OnPlayerDied += HandlePlayerDeath;
        }

        private void HandlePlayerDeath()
        {
            EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();

            deadUI.Show(spawner.currentWave);

            Destroy(currentPlayer);

            // Destroy all tag projectile and all tag enemies
            GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

            foreach (GameObject projectile in projectiles)
                Destroy(projectile);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
                Destroy(enemy);
            Time.timeScale = 0f;
        }

        public void RestartGame()
        {
            SpawnPlayer();

            Time.timeScale = 1f;
        }
    }
}
