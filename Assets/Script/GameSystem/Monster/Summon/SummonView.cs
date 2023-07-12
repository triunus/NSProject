using UnityEngine;

namespace GameSystem.Summon
{
    public interface ISummonView
    {
        public void InitialSettingSummonView(Monster.MonsterName MonsterName, Vector3 startPosition, SummonSpawnType summonSpawnType);
        public void SetSummonSequenceIndex(int index);
        public void Destroy();

        public Monster.MonsterStruct MonsterStruct { get; }
        public Vector3 SummonPosition { get; }

        public string SummonTag { get; }
        public bool NextDirection { get; set; }
        public int SummonCurrentHP { get; set; }

        public Player.PlayerState PlayerState { set; }
    }

    public class SummonView : MonoBehaviour, ISummonView
    {
        private ISummonPresenter summonPresenter;
        private Monster.IMonsterAnimationView monsterAnimationView;

        private Rigidbody2D summonRigidbody2D;
        private SpriteRenderer summonSpriteRenderer;

        private Monster.MonsterStruct monsterStruct;
        private SummonState summonState;
        private bool nextDirection = false;
        private float destinationPosition;
        private int index;

        // View에서만 사용.
        private Player.PlayerState playerState;
        private int summonCurrentHP;

        private bool isReady = false;
        private bool isCreateArrange = false;
        private bool isAction = false;
        private bool isHurt = false;
        private bool isDie = false;
        private bool isAttack = false;

        public Monster.MonsterStruct MonsterStruct { get { return this.monsterStruct; } }
        public Vector3 SummonPosition { get { return this.summonRigidbody2D.position; } }
        public string SummonTag { get { return this.gameObject.tag; } }
        public bool NextDirection { get { return this.nextDirection; } set { this.nextDirection = value; } }
        public int SummonCurrentHP { get { return this.summonCurrentHP; } set { this.summonCurrentHP = value; } }

        public Player.PlayerState PlayerState { set { this.playerState = value; } }

        private void Awake()
        {
            this.summonRigidbody2D = GetComponent<Rigidbody2D>();
            this.summonSpriteRenderer = GetComponent<SpriteRenderer>();
            this.monsterAnimationView = GetComponent<Monster.MonsterAnimationView>();

            this.summonPresenter = new SummonPresenter(this);
        }

        // ISummonViewForSummonManager 구현
        public void InitialSettingSummonView(Monster.MonsterName MonsterName, Vector3 startPosition, SummonSpawnType summonSpawnType)
        {
            // ready animation 진행 후, 3 초후 tag와 layer 변경.
            this.monsterStruct = new Monster.MonsterStruct(MonsterName);
            this.gameObject.transform.position = startPosition;

            this.summonCurrentHP = this.monsterStruct.BaseHP;
            this.summonPresenter.RegisterSummonView();

            this.isReady = true;

            switch (summonSpawnType)
            {
                case SummonSpawnType.SpawnByRevival:
                    this.summonState = SummonState.SpawnByRevival;
                    break;
                case SummonSpawnType.SpawnByLoadFile:
                    this.summonState = SummonState.SpawnByLoadFile;
                    break;
                case SummonSpawnType.SpawnByObject:
                    this.summonState = SummonState.SpawnByObject;
                    break;
                default:
                    break;
            }

            // Summon이 지정된 위치로 가기 전까지, 보호된다.
            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;
        }
        public void SetSummonSequenceIndex(int index)
        {
            this.index = index;
        }

        private void FixedUpdate()
        {
            // Model에 저장된 주변 객체들의 정보를 이용하여 수행할 행동을 정한다.
            if (isReady) { }
            else
            {
                if (isAction) { }
                else
                {
                    if (this.summonPresenter.RecognizeTheEnemy()) // Enemy 인지로 변경.
                    {
                        // enemy가 있는 위치를 바라보기.
                        this.summonPresenter.GetNextDirection();
                        this.summonSpriteRenderer.flipX = this.nextDirection;

                        if (this.summonPresenter.DetermineWhetherOrNotAttack())
                        {
                            this.isAction = true;
                            this.summonState = SummonState.Attack;
                        }
                        else this.summonState = SummonState.Run;
                    }
                    else
                    {
                        this.GetNextDirection();
                        this.summonSpriteRenderer.flipX = this.nextDirection;

                        this.GetDestinationPosition();
                        if (Mathf.Abs(this.summonRigidbody2D.position.x - this.destinationPosition) < 0.1f) this.summonState = SummonState.Idle;
                        else if (Mathf.Abs(this.summonRigidbody2D.position.x - this.destinationPosition) < 2.1f) this.summonState = SummonState.Walk;
                        else this.summonState = SummonState.Run;
                    }
                }
            }
            // --------------------------------------

            switch (this.summonState)
            {
                case SummonState.SpawnByRevival:
                    if (!this.isCreateArrange)
                    {
                        this.isCreateArrange = true;
                        this.monsterAnimationView.ReviveAnimation();

                        StopCoroutine("InitialSetting_SpawnByRevival");
                        StartCoroutine("InitialSetting_SpawnByRevival");
                    }
                    break;
                case SummonState.SpawnByLoadFile:
                    if (!this.isCreateArrange)
                    {
                        this.isCreateArrange = true;

                        StopCoroutine("InitialSetting_SpawnByLoadFile");
                        StartCoroutine("InitialSetting_SpawnByLoadFile");
                    }
                    break;
                case SummonState.Rearrange:
                    this.GetNextDirection();
                    this.summonSpriteRenderer.flipX = this.nextDirection;

                    this.GetDestinationPosition();
                    float summonNextMovementX = this.summonRigidbody2D.position.x;

                    if (Mathf.Abs(this.summonRigidbody2D.position.x - this.destinationPosition) < 0.1f)
                    {
                        this.monsterAnimationView.IdleAnimation();
                    }
                    else if (Mathf.Abs(this.summonRigidbody2D.position.x - this.destinationPosition) < 2.1f)
                    {
                        this.monsterAnimationView.WalkAnimation();
                        summonNextMovementX = summonNextMovementX + (this.nextDirection == true ? -1 : 1) * this.monsterStruct.WalkSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        this.monsterAnimationView.RunAnimation();
                        summonNextMovementX = summonNextMovementX + (this.nextDirection == true ? -1 : 1) * this.monsterStruct.RunSpeed * Time.fixedDeltaTime;
                    }

                    this.summonRigidbody2D.MovePosition(new Vector2(summonNextMovementX, this.summonRigidbody2D.position.y));

                    break;
                case SummonState.Idle:
                    this.monsterAnimationView.IdleAnimation();
                    break;
                case SummonState.Walk:
                    this.monsterAnimationView.WalkAnimation();

                    this.summonRigidbody2D.MovePosition(new Vector2(
                        this.summonRigidbody2D.position.x + (this.nextDirection == true ? -1 : 1) * this.monsterStruct.WalkSpeed * Time.fixedDeltaTime,
                        this.summonRigidbody2D.position.y));
                    break;
                case SummonState.Run:
                    this.monsterAnimationView.RunAnimation();

                    this.summonRigidbody2D.MovePosition(new Vector2(
                        this.summonRigidbody2D.position.x + (this.nextDirection == true ? -1 : 1) * this.monsterStruct.RunSpeed * Time.fixedDeltaTime,
                        this.summonRigidbody2D.position.y));
                    break;
                case SummonState.Hurt:
                    if (!this.isHurt)
                    {
                        this.isHurt = true;
                        this.monsterAnimationView.HurtAnimation();

                        StopCoroutine("HurtRearrange");
                        StartCoroutine("HurtRearrange");
                    }
                    this.summonRigidbody2D.MovePosition(new Vector2(this.summonRigidbody2D.position.x + (this.nextDirection == true ? 1 : -1) * (this.monsterStruct.HurtSpeed * Time.fixedDeltaTime), this.summonRigidbody2D.position.y));
                    break;
                case SummonState.Die:
                    if (!this.isDie)
                    {
                        this.isDie = true;
                        this.monsterAnimationView.DieAnimation();

                        StopCoroutine("DieSummon");
                        StartCoroutine("DieSummon");
                    }
                    break;
                case SummonState.Attack:
                    if (!this.isAttack)
                    {
                        this.isAttack = true;
                        this.monsterAnimationView.AttackAnimation();

                        StopCoroutine("AttackRearrange");
                        StartCoroutine("AttackRearrange");
                    }
                    break;
                case SummonState.Skill01:
                    break;
                case SummonState.Skill02:
                    break;
                default:
                    break;
            }
        }

        private void DecideSummonBehavior()
        {
            this.summonPresenter.GetPlayerState();

            switch (this.playerState)
            {
                case Player.PlayerState.Idle:
                    this.summonState = SummonState.Idle;
                    break;
                case Player.PlayerState.Walk:
                    this.summonState = SummonState.Walk;
                    break;
                case Player.PlayerState.Run:
                    this.summonState = SummonState.Run;
                    break;
                case Player.PlayerState.Die:
                    this.summonState = SummonState.Die;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 재배치될 방향을 가리킨다.
        /// </summary>
        private void GetNextDirection()
        {
            this.GetDestinationPosition();

            if (this.destinationPosition > this.summonRigidbody2D.position.x) this.nextDirection = false;
            else this.nextDirection = true;
        }

        private void GetDestinationPosition()
        {
            this.destinationPosition = this.summonPresenter.GetPlayerPositionX() + 1f - (this.index * 0.25f);
        }

        private System.Collections.IEnumerator InitialSetting_SpawnByRevival()
        {
            this.GetNextDirection();
            this.summonSpriteRenderer.flipX = this.nextDirection;

            yield return new WaitForSeconds(this.monsterAnimationView.GetRevivalAnimationTime());

            this.summonState = SummonState.Idle;

            yield return new WaitForSeconds(1.0f);

            this.summonState = SummonState.Rearrange;

            yield return new WaitForSeconds(1.0f);

            this.isCreateArrange = false;
            this.isReady = false;

            this.gameObject.tag = "Summon";
            this.gameObject.layer = 8;
        }
        private System.Collections.IEnumerator InitialSetting_SpawnByLoadFile()
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);

            this.GetNextDirection();
            this.summonSpriteRenderer.flipX = this.nextDirection;

            this.summonState = SummonState.Rearrange;

            yield return new WaitForSeconds(Time.fixedDeltaTime * 3);

            this.isCreateArrange = false;
            this.isReady = false;

            this.gameObject.tag = "Summon";
            this.gameObject.layer = 8;
        }
        private System.Collections.IEnumerator HurtRearrange()
        {
            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;

            yield return new WaitForSeconds(this.monsterAnimationView.GetHurtAnimationTime() + Time.fixedDeltaTime);

            this.isHurt = false;
            this.isReady = true;

            if (this.monsterStruct.MonsterAttackType.Equals(Monster.MonsterAttackType.MeleeAttack)) this.summonState = SummonState.Walk;
            else this.summonState = SummonState.Rearrange;

            yield return new WaitForSeconds(1.0f);

            this.isReady = false;
            this.isAction = false;

            this.gameObject.tag = "Summon";
            this.gameObject.layer = 8;
        }
        private System.Collections.IEnumerator AttackRearrange()
        {
            // 공격 모션 수행 후, Attack Object 생성.
            yield return new WaitForSeconds(this.monsterAnimationView.GetAttackPhase01AnimationTime());

            this.summonPresenter.SpawnNormalAttack();

            yield return new WaitForSeconds(this.monsterAnimationView.GetAttackPhase02AnimationTime());

            // 공격 종료 후 재배치 작업을 갖는다.
            this.isAttack = false;
            this.isReady = true;

            if (this.monsterStruct.MonsterAttackType.Equals(Monster.MonsterAttackType.MeleeAttack)) this.summonState = SummonState.Idle;
            else this.summonState = SummonState.Rearrange;

            yield return new WaitForSeconds(1.0f);

            this.isReady = false;
            this.isAction = false;
        }
        private System.Collections.IEnumerator DieSummon()
        {
            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;

            yield return new WaitForSeconds(this.monsterAnimationView.GetDieAnimationTime());

            this.summonPresenter.SpawnCorpse();

            this.Destroy();
        }
        private void StopAttack()
        {
            StopCoroutine("AttackReady");
            this.monsterAnimationView.StopAttackAnimation();

            this.isReady = false;
            this.isAttack = false;
        }

        // AttackObjectView에서 충돌 시, 사용한다.
        public void OnCollisionWithAttackObject(int damage)
        {
            this.isAction = true;

            this.summonCurrentHP = this.summonCurrentHP - damage;

            if (this.summonCurrentHP > 0)
            {
                this.summonState = SummonState.Hurt;
            }
            else
            {
                this.summonState = SummonState.Die;
            }

            this.StopAttack();
        }

        public void Destroy()
        {
            this.summonPresenter.RemoveSummonView();
            this.summonPresenter = null;
            Destroy(this.gameObject);
        }
    }
}
