using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Enemy
{
    public interface IEnemySpawnerManager
    {
        public Vector3 EnemySpawnerPosition { get; set; }
    }

    public class EnemySpawnerManager : MonoBehaviour, IEnemySpawnerManager
    {
        private GameObject enemySpawner;

        private SceneName sceneName;
        private string enemySpawnerName;
        private Vector3 enemySpawnerPosition;

        private void Awake()
        {
            this.sceneName = GameManager.Instance.GameDataStruct.SceneName;
            this.ChooseEnemySpawnerAndPosition();
        }
        private void Start()
        {
            this.enemySpawner = Instantiate(Resources.Load<GameObject>("Prefab/Monster/Enemy/" + this.enemySpawnerName));
            this.enemySpawner.transform.position = this.enemySpawnerPosition;
        }
        private void ChooseEnemySpawnerAndPosition()
        {
            switch (this.sceneName)
            {
                case SceneName.Forest:
                    this.enemySpawnerName = "ForestEnemySpawner";
                    this.enemySpawnerPosition = new Vector3(23.5f, -3.35f, 0);
                    break;
                case SceneName.Desert:
                    break;
                case SceneName.ForestDungeon:
                    break;
                case SceneName.DesertDungeon:
                    break;
                default:
                    break;
            }
        }

        public Vector3 EnemySpawnerPosition { get { return this.enemySpawnerPosition; } set { this.enemySpawnerPosition = value; } }
    }

}