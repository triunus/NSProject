using UnityEngine;

namespace GameSystem.Player
{
    public interface IPlayerController
    {
        public void UpdatePlayerManager(Vector3 playerPosition);
        public void SpawnAttackObject(AttackObject.SkillName skillName);
        public void GameOver();

        public bool IsAction { get; set; }
    }

    public class PlayerController : MonoBehaviour, IPlayerController
    {
        private IPlayerModel playerModel;

        // 특정 Action을 수행 중일 때는 사용자의 입력을 블럭해야 한다.
        // Update에서 들어온 입력을 블럭해주기 위해 isAction 변수를 사용한다.
        private bool isAction = false;

        public bool IsAction { get { return this.isAction; } set { this.isAction = value; } }

        private void Awake()
        {
            this.playerModel = GetComponent<PlayerModel>();

            this.isAction = false;
        }

        // 사용자의 입력은 사용자의 컴퓨터 사항에 맞물려 작동하는 Update에서 동작하도록 한다.
        private void Update()
        {
            // 진행중인 Action 있음 = true, 진행중인 Action 없음 = false 이다.
            if (this.isAction) { }
            else
            {
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        this.playerModel.Run(Input.GetAxis("Horizontal"));
                    }
                    else
                    {
                        this.playerModel.Walk(Input.GetAxis("Horizontal"));
                    }
                }
                else
                {
                    this.playerModel.Idle(Input.GetAxis("Horizontal"));
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    this.isAction = true;
                    this.playerModel.BaltAttack();
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    this.isAction = true;
                    this.playerModel.LightingAttack();
                }
            }
        }

        public void UpdatePlayerManager(Vector3 playerPosition)
        {
            this.playerModel.UpdatePlayerManager(playerPosition);
        }

        public void OnCollisionWithAttackObject(int damage)
        {
            this.IsAction = true;

            this.playerModel.BeHurt(damage);
        }

        public void SpawnAttackObject(AttackObject.SkillName skillName)
        {
            this.playerModel.SpawnAttackObject(skillName);
        }

        public void GameOver()
        {
            Debug.Log("Controller : GameOver");
            this.playerModel.GameOver();
        }
    }
}