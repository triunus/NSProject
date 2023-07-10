using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Player
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Balt,
        Lightning,
        Hurt,
        Die
    }
    public interface IPlayerStateObserverForView
    {
        public void UpdatePlayerStateObserver();
    }
    public interface IPlayerSateObserver
    {
        public void RegisterPlayerStateObserver(IPlayerStateObserverForView observer);
        public void RemovePlayerStateObserver(IPlayerStateObserverForView observer);
        public PlayerState GetPlayerState();
    }

    public interface IPlayerNextMovementObserverForView
    {
        public void UpdatePlayerNextMovementObserver();
    }
    public interface IPlayerNextMovementObserver
    {
        public void RegisterPlayerNextMovementObserver(IPlayerNextMovementObserverForView observer);
        public UnityEngine.Vector2 GetPlayerNextMovement();
    }

    public interface IMoveDirectionObserverForView
    {
        public void UpdateMoveDirectionObserver();
    }
    public interface IMoveDirectionObserver
    {
        public void RegisterMoveDirectionObserver(IMoveDirectionObserverForView observer);
        public bool GetMoveDirection();
    }

    public interface IPlayerModelForPlayerManager
    {
        public void Destroy();
    }

    public interface IPlayerModel : IPlayerSateObserver, IPlayerNextMovementObserver, IMoveDirectionObserver
    {
        public void Idle(float nextMovement);
        public void Walk(float nextMovement);
        public void Run(float nextMovement);
        public void BeHurt(int damage);

        public void BaltAttack();
        public void LightingAttack();

        public void UpdatePlayerManager(Vector3 playerPosition);
        public void SpawnAttackObject(AttackObject.SkillName skillName);
        public void GameOver();
    }

    public class PlayerModel : MonoBehaviour, IPlayerModelForPlayerManager, IPlayerModel
    {
        private IPlayerManagerForModel playerManager;

        private List<IPlayerStateObserverForView> playerStateObservers = new List<IPlayerStateObserverForView>();
        private List<IPlayerNextMovementObserverForView> playerNextPositionObservers = new List<IPlayerNextMovementObserverForView>();
        private List<IMoveDirectionObserverForView> moveDirectionObservers = new List<IMoveDirectionObserverForView>();

        private PlayerState playerState;
        private bool moveDirection = false;
        private Vector2 playerNextMovement;

        private float baseSpeed = 3f;
        private float sprintSpeed = 2f;
        private float hurtSpeed = 2f;

        private int currentHP = 0;
        private float currentSpeed = 0f;        

        private void Start()
        {
            this.playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<PlayerManager>();
            this.initializing();
        }

        private void initializing()
        {
            this.playerState = this.playerManager.PlayerState;
            this.moveDirection = this.playerManager.MoveDirection;
            this.gameObject.transform.position = this.playerManager.PlayerPosition;
            this.currentHP = this.playerManager.CurrentHP;
        }

        public void Idle(float nextMovement)
        {
            this.playerState = PlayerState.Idle;

            this.currentSpeed = 0f;
            this.playerNextMovement = new Vector3(nextMovement, 0, 0).normalized * currentSpeed * Time.fixedDeltaTime;

            this.NotifyPlayerStateObservers();
            this.NotifyPlayerNextMovementObservers();
            this.NotifyMoveDirectionObservers();
        }
        public void Walk(float nextMovement)
        {
            this.playerState = PlayerState.Walk;

            this.currentSpeed = baseSpeed;
            this.playerNextMovement = new Vector3(nextMovement, 0, 0).normalized * currentSpeed * Time.fixedDeltaTime;

            if (nextMovement > 0) this.moveDirection = false;
            else if(nextMovement < 0) this.moveDirection = true;

            this.NotifyPlayerStateObservers();
            this.NotifyPlayerNextMovementObservers();
            this.NotifyMoveDirectionObservers();
        }
        public void Run(float nextMovement)
        {
            this.playerState = PlayerState.Run;

            this.currentSpeed = baseSpeed + sprintSpeed;
            this.playerNextMovement = new Vector3(nextMovement, 0, 0).normalized * currentSpeed * Time.fixedDeltaTime;

            if (nextMovement > 0) this.moveDirection = false;
            else if (nextMovement < 0) this.moveDirection = true;

            this.NotifyPlayerStateObservers();
            this.NotifyPlayerNextMovementObservers();
            this.NotifyMoveDirectionObservers();
        }
        public void BeHurt(int damage)
        {
            this.currentHP = this.currentHP - damage;

            if (currentHP <= 0)
            {
                this.playerState = PlayerState.Die;
            }
            else
            {
                this.playerState = PlayerState.Hurt;

                this.currentSpeed = hurtSpeed;
                this.playerNextMovement = new Vector3((this.moveDirection == true ? 1f : -1f), 0, 0).normalized * currentSpeed * Time.fixedDeltaTime;
            }

            // 아마 currentHP는 Observer로 만들어야 될 것 같다. 차후, HP UI에서 사용할 필요성이 있음.
            this.NotifyPlayerStateObservers();
            this.NotifyPlayerNextMovementObservers();
        }

        public void BaltAttack()
        {
            this.playerState = PlayerState.Balt;

            this.NotifyPlayerStateObservers();
        }
        public void LightingAttack()
        {
            this.playerState = PlayerState.Lightning;

            this.NotifyPlayerStateObservers();
        }

        public void UpdatePlayerManager(Vector3 playerPosition)
        {
            this.playerManager.PlayerState = this.playerState;
            this.playerManager.MoveDirection = this.moveDirection;
            this.playerManager.PlayerPosition = playerPosition;
        }
        public void SpawnAttackObject(AttackObject.SkillName skillName)
        {
            this.playerManager.SpawnAttackObject(skillName);
        }
        public void GameOver()
        {
            // Singleton으로 구현된 GameManager을 만들어, 게임 전체 정지 -> 게임 종료 UI 출력 -> 메뉴화면으로 이동 구현.

        }

        public void Destroy()
        {
            Destroy(this.gameObject);
        }


        public void RegisterPlayerStateObserver(IPlayerStateObserverForView observer)
        {
            playerStateObservers.Add(observer);
        }
        public void RemovePlayerStateObserver(Player.IPlayerStateObserverForView observer)
        {
            playerStateObservers.Remove(observer);
        }
        public PlayerState GetPlayerState()
        {
            return this.playerState;
        }
        private void NotifyPlayerStateObservers()
        {
            int i = 0;

            while (i < playerStateObservers.Count)
            {
                playerStateObservers[i].UpdatePlayerStateObserver();
                ++i;
            }
        }

        public void RegisterPlayerNextMovementObserver(IPlayerNextMovementObserverForView observer)
        {
            this.playerNextPositionObservers.Add(observer);
        }
        public Vector2 GetPlayerNextMovement()
        {
            return this.playerNextMovement;
        }
        private void NotifyPlayerNextMovementObservers()
        {
            int i = 0;

            while (i < playerNextPositionObservers.Count)
            {
                playerNextPositionObservers[i].UpdatePlayerNextMovementObserver();
                ++i;
            }
        }

        public void RegisterMoveDirectionObserver(IMoveDirectionObserverForView observer)
        {
            moveDirectionObservers.Add(observer);
        }
        public bool GetMoveDirection()
        {
            return this.moveDirection;
        }
        private void NotifyMoveDirectionObservers()
        {
            int i = 0;

            while (i < moveDirectionObservers.Count)
            {
                moveDirectionObservers[i].UpdateMoveDirectionObserver();
                ++i;
            }
        }
    }
}