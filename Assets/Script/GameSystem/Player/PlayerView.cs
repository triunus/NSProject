using System.Collections;
using UnityEngine;

namespace GameSystem.Player
{
    public class PlayerView : MonoBehaviour, IPlayerStateObserverForView, IPlayerNextMovementObserverForView, IMoveDirectionObserverForView
    {
        private IPlayerSateObserver playerStateModel;
        private IPlayerNextMovementObserver playerNextMovementModel;
        private IMoveDirectionObserver moveDirectionModel;

        private IPlayerController playerController;

        private Rigidbody2D playerRigidbody2D;
        private SpriteRenderer playerSpriteRenderer;
        private Animator playerAnimator;

        private PlayerState playerState;
        private Vector2 playerNextMovement;
        private bool moveDirection = false;

        private bool isHurt = false;
        private bool isDie = false;
        private bool isAttack = false;

        private void Awake()
        {
            this.playerStateModel = GetComponent<PlayerModel>();
            this.playerNextMovementModel = GetComponent<PlayerModel>();
            this.moveDirectionModel = GetComponent<PlayerModel>();

            this.playerController = GetComponent<PlayerController>();

            this.playerRigidbody2D = GetComponent<Rigidbody2D>();
            this.playerSpriteRenderer = GetComponent<SpriteRenderer>();
            this.playerAnimator = GetComponent<Animator>();

            this.playerStateModel.RegisterPlayerStateObserver(this);
            this.playerNextMovementModel.RegisterPlayerNextMovementObserver(this);
            this.moveDirectionModel.RegisterMoveDirectionObserver(this);

            this.playerRigidbody2D.position = new Vector3(-6f, -3f, 0);
        }

        // 전반적인 일정한 시각 간격으로 동작은 FixedUpdate에서 수행하도록 한다.
        private void FixedUpdate()
        {
            playerSpriteRenderer.flipX = this.moveDirection;

            switch (this.playerState)
            {
                case PlayerState.Idle:
                    playerRigidbody2D.MovePosition(this.playerRigidbody2D.position + playerNextMovement);
                    this.playerController.IsAction = false;
                    this.IdleAnimation();
                    break;
                case PlayerState.Walk:
                    playerRigidbody2D.MovePosition(this.playerRigidbody2D.position + playerNextMovement);
                    this.playerController.IsAction = false;
                    this.WalkAnimation();
                    break;
                case PlayerState.Run:
                    playerRigidbody2D.MovePosition(this.playerRigidbody2D.position + playerNextMovement);
                    this.playerController.IsAction = false;
                    this.RunAnimation();
                    break;
                case PlayerState.Hurt:
                    playerRigidbody2D.MovePosition(this.playerRigidbody2D.position + playerNextMovement);
                    if (!isHurt)
                    {
                        StopCoroutine("HurtAction");
                        StartCoroutine("HurtAction");
                    }
                    break;
                case PlayerState.Die:
                    if (!isDie)
                    {
                        StopCoroutine("DieAction");
                        StartCoroutine("DieAction");
                    }
                    break;
                case PlayerState.Balt:
                    if (!isAttack)
                    {
                        StopCoroutine("PlayerBaltAction");
                        StartCoroutine("PlayerBaltAction");
                    }
                    break;
                case PlayerState.Lightning:
                    if (!isAttack)
                    {
                        StopCoroutine("PlayerLightningAction");
                        StartCoroutine("PlayerLightningAction");
                    }
                    break;
                default:
                    break;
            }

            this.playerController.UpdatePlayerManager(this.playerRigidbody2D.position);
        }

        // playerAnimator.runtimeAnimatorController.animationClips : 0 - Idle, 1 - Walk, 2 - Run, 3 - Die, 4 - Hurt, 5 - BaltPhase01, 6 - BaltPhase02, 7 - LightningPhase01, 8 - LightningPhase02
        private void IdleAnimation()
        {
            playerAnimator.SetBool("isWalking", false);
            playerAnimator.SetBool("isRunning", false);
        }
        private void WalkAnimation()
        {
            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetBool("isRunning", false);
        }
        private void RunAnimation()
        {
            playerAnimator.SetBool("isWalking", false);
            playerAnimator.SetBool("isRunning", true);
        }

        private IEnumerator HurtAction()
        {
            this.isHurt = true;

            float waitTime = playerAnimator.runtimeAnimatorController.animationClips[3].length;

            playerSpriteRenderer.color = Color.red;
            playerAnimator.SetBool("isHurt", true);

            yield return new WaitForSeconds(waitTime);

            playerAnimator.SetBool("isHurt", false);
            playerSpriteRenderer.color = Color.white;

            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;

            this.playerController.IsAction = false;
            this.isHurt = false;

            // 피격 후 무적 시간.
            yield return new WaitForSeconds(1.5f);

            this.gameObject.tag = "Player";
            this.gameObject.layer = 6;
        }
        private IEnumerator DieAction()
        {
            this.isDie = true;

            // Die 애니메이션 번호 찾기.
            float waitTime = playerAnimator.runtimeAnimatorController.animationClips[4].length;

            playerAnimator.SetBool("isDie", true);

            yield return new WaitForSeconds(waitTime);

            playerAnimator.SetBool("isDie", false);

            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;

            this.isDie = false;
            this.playerController.GameOver();
        }
        private IEnumerator PlayerBaltAction()
        {
            this.isAttack = true;

            // 5 : baltPhase01, 6 : baltPhase02
            float baltPhase01WaitTime = this.playerAnimator.runtimeAnimatorController.animationClips[5].length;
            float baltPhase02WaitTime = this.playerAnimator.runtimeAnimatorController.animationClips[6].length;

            playerAnimator.SetBool("BaltPhase01", true);

            yield return new WaitForSeconds(baltPhase01WaitTime);

            playerAnimator.SetBool("BaltPhase01", false);
            playerAnimator.SetBool("BaltPhase02", true);

            // Balt 프리팹 생성.
            this.playerController.SpawnAttackObject(AttackObject.SkillName.LightningBalt);

            yield return new WaitForSeconds(baltPhase02WaitTime + Time.fixedDeltaTime);

            playerAnimator.SetBool("BaltPhase02", false);

            this.playerController.IsAction = false;
            this.isAttack = false;
        }
        private IEnumerator PlayerLightningAction()
        {
            Debug.Log("PlayerAttackView : Lightning");

            this.isAttack = true;

            float LightningPhase01WaitTime = playerAnimator.runtimeAnimatorController.animationClips[7].length;
            float LightningPhase02WaitTime = playerAnimator.runtimeAnimatorController.animationClips[8].length;

            playerAnimator.SetBool("LightningPhase01", true);

            yield return new WaitForSeconds(LightningPhase01WaitTime);

            // Lightning 프리팹 생성.
            this.playerController.SpawnAttackObject(AttackObject.SkillName.Lightning);

            playerAnimator.SetBool("LightningPhase01", false);
            playerAnimator.SetBool("LightningPhase02", true);

            yield return new WaitForSeconds(LightningPhase02WaitTime);

            playerAnimator.SetBool("LightningPhase02", false);

            this.playerController.IsAction = false;
            this.isAttack = false;
        }

        public void UpdatePlayerStateObserver()
        {
            this.playerState = this.playerStateModel.GetPlayerState();
        }
        public void UpdatePlayerNextMovementObserver()
        {
            this.playerNextMovement = this.playerNextMovementModel.GetPlayerNextMovement();
        }
        public void UpdateMoveDirectionObserver()
        {
            this.moveDirection = this.moveDirectionModel.GetMoveDirection();
        }
    }
}