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

        // Ư�� Action�� ���� ���� ���� ������� �Է��� ���ؾ� �Ѵ�.
        // Update���� ���� �Է��� �����ֱ� ���� isAction ������ ����Ѵ�.
        private bool isAction = false;

        public bool IsAction { get { return this.isAction; } set { this.isAction = value; } }

        private void Awake()
        {
            this.playerModel = GetComponent<PlayerModel>();

            this.isAction = false;
        }

        // ������� �Է��� ������� ��ǻ�� ���׿� �¹��� �۵��ϴ� Update���� �����ϵ��� �Ѵ�.
        private void Update()
        {
            // �������� Action ���� = true, �������� Action ���� = false �̴�.
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