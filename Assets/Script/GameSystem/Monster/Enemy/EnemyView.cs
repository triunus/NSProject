using UnityEngine;

namespace GameSystem.Enemy
{
    public interface IEnemyView
    {
        public void InitialSettingEnemyView(Monster.MonsterName MonsterName, Vector3 startPosition, EnemySpawnType enemySpawnType);
        public void SetEnemySequenceIndex(int index);
        public void Destroy();

        public Monster.MonsterStruct MonsterStruct { get; }
        public Vector3 EnemyPosition { get; }
        public string EnemyTag { get; }
        public bool NextDirection { get; set; }

        public int EnemyCurrentHP { get; set; }
    }

    public class EnemyView : MonoBehaviour, IEnemyView
    {
        private IEnemyPresenter enemyPresenter;
        private Monster.IMonsterAnimationView monsterAnimationView;

        private Rigidbody2D enemyRigidbody2D;
        private SpriteRenderer monsterSpriteRenderer;

        private Monster.MonsterStruct monsterStruct;
        private EnemyState enemyState;
        private bool nextDirection = true;
        private int index;

        private int enemyCurrentHP;

        private bool isReady = false;
        private bool isCreateArrange = false;
        private bool isAction = false;
        private bool isHurt = false;
        private bool isDie = false;
        private bool isRevive = false;
        private bool isAttack = false;

        // IEnemyView 구현
        public Monster.MonsterStruct MonsterStruct { get { return this.monsterStruct; } }
        public Vector3 EnemyPosition { get { return this.enemyRigidbody2D.transform.position; } }
        public string EnemyTag { get { return this.gameObject.tag; } }
        public bool NextDirection { get { return this.nextDirection; } set { this.nextDirection = value; } }
        public int EnemyCurrentHP { get { return this.enemyCurrentHP; } set { this.enemyCurrentHP = value; } }

        private void Awake()
        {
            this.enemyRigidbody2D = GetComponent<Rigidbody2D>();
            this.monsterSpriteRenderer = GetComponent<SpriteRenderer>();
            this.monsterAnimationView = GetComponent<Monster.MonsterAnimationView>();

            this.enemyPresenter = new EnemyPresenter(this);
        }
        public void InitialSettingEnemyView(Monster.MonsterName MonsterName, Vector3 startPosition, EnemySpawnType enemySpawnType)
        {
            this.monsterStruct = new Monster.MonsterStruct(MonsterName);
            this.gameObject.transform.position = startPosition;

            this.enemyCurrentHP = this.monsterStruct.BaseHP;
            this.enemyPresenter.RegisterEnemyView();

            this.isReady = true;

            switch (enemySpawnType)
            {
                case EnemySpawnType.SpawnByTime:
                    this.enemyState = EnemyState.SpawnByTime;
                    break;
                case EnemySpawnType.SpawnByLoadFile:
                    this.enemyState = EnemyState.SpawnByLoadFile;
                    break;
                case EnemySpawnType.SpawnByObject:
                    this.enemyState = EnemyState.SpawnByObject;
                    break;
                default:
                    break;
            }
        }
        public void SetEnemySequenceIndex(int index)
        {
            this.index = index;
        }

        private void FixedUpdate()
        {
            if (isReady) { }
            else
            {
                if (isAction) { }
                else
                {
                    this.enemyPresenter.GetNextDirection();

                    if (this.enemyPresenter.DetermineWhetherOrNotAttack())
                    {
                        this.isAction = true;
                        this.enemyState = EnemyState.Attack;
                    }
                    else this.enemyState = EnemyState.Run;

                    monsterSpriteRenderer.flipX = this.nextDirection;
                }
            }

            switch (this.enemyState)
            {
                case EnemyState.SpawnByTime:
                    if(!this.isCreateArrange)
                    {
                        this.isCreateArrange = true;

                        StopCoroutine("InitialSetting_SpawnByTime");
                        StartCoroutine("InitialSetting_SpawnByTime");
                    }
                    break;
                case EnemyState.SpawnByLoadFile:
                    if (!this.isCreateArrange)
                    {
                        this.isCreateArrange = true;

                        StopCoroutine("InitialSetting_SpawnByLoadFile");
                        StartCoroutine("InitialSetting_SpawnByLoadFile");
                    }
                    break;
                case EnemyState.SpawnByObject:
                    break;
                case EnemyState.Revive:
                    if (!this.isRevive)
                    {
                        this.isRevive = true;
                        this.monsterAnimationView.ReviveAnimation();

                        StopCoroutine("RevivalRearrange");
                        StartCoroutine("RevivalRearrange");
                    }
                    break;
                case EnemyState.Idle:
                    this.monsterAnimationView.IdleAnimation();
                    break;
                case EnemyState.Walk:
                    this.monsterAnimationView.WalkAnimation();
                    this.enemyRigidbody2D.MovePosition(this.GetWalkMovement());
                    break;
                case EnemyState.Run:
                    this.monsterAnimationView.RunAnimation();
                    this.enemyRigidbody2D.MovePosition(this.GetRunMovement());
                    break;
                case EnemyState.Hurt:
                    if (!this.isHurt)
                    {
                        this.isHurt = true;
                        this.monsterAnimationView.HurtAnimation();

                        StopCoroutine("HurtReady");
                        StartCoroutine("HurtReady");
                    }
                    this.enemyRigidbody2D.MovePosition(this.GetHurtMovement());
                    break;
                case EnemyState.Die:
                    if (!this.isDie)
                    {
                        this.isDie = true;
                        this.monsterAnimationView.DieAnimation();

                        StopCoroutine("DieEnemy");
                        StartCoroutine("DieEnemy");
                    }
                    break;
                case EnemyState.Attack:
                    if (!this.isAttack)
                    {
                        this.isAttack = true;
                        this.monsterAnimationView.AttackAnimation();

                        StopCoroutine("AttackReady");
                        StartCoroutine("AttackReady");
                    }
                    break;
                default:
                    break;
            }
        }

        private Vector3 GetWalkMovement()
        {
            return this.enemyRigidbody2D.transform.position +
                new Vector3((this.nextDirection == true ? -1 : 1) * this.monsterStruct.WalkSpeed * Time.fixedDeltaTime, 0, 0);
        }
        private Vector3 GetRunMovement()
        {
            return this.enemyRigidbody2D.transform.position +
                new Vector3((this.nextDirection == true ? -1 : 1) * this.monsterStruct.RunSpeed * Time.fixedDeltaTime, 0, 0);
        }
        private Vector3 GetHurtMovement()
        {
            return this.enemyRigidbody2D.transform.position +
                new Vector3((nextDirection == true ? 1 : -1) * this.monsterStruct.HurtSpeed * Time.fixedDeltaTime, 0, 0);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Player.PlayerController>().OnCollisionWithAttackObject(1);
            }
        }

        private System.Collections.IEnumerator InitialSetting_SpawnByTime()
        {
            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;

            yield return new WaitForSeconds(10 * Time.fixedDeltaTime);

            this.enemyPresenter.GetNextDirection();
            monsterSpriteRenderer.flipX = this.nextDirection;

            float pastedTime = 0f;
            float destinationPositionX = 0f;
            // 최초 생성 시, 대기 위치.
            if (this.nextDirection) destinationPositionX = this.enemyRigidbody2D.position.x - 2f + (this.index * 0.25f);
            else destinationPositionX = this.enemyRigidbody2D.position.x + 2f - (this.index * 0.25f);

            while (true)
            {
                if (this.nextDirection)
                {
                    if (this.enemyRigidbody2D.position.x <= destinationPositionX) break;
                }
                else
                {
                    if (this.enemyRigidbody2D.position.x >= destinationPositionX) break;
                }

                this.enemyState = EnemyState.Walk;

                pastedTime += Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            this.enemyState = EnemyState.Idle;

            yield return new WaitForSeconds(5 - pastedTime - this.index * Time.fixedDeltaTime);

            this.isCreateArrange = false;
            this.isReady = false;

            this.gameObject.tag = "Enemy";
            this.gameObject.layer = 7;
        }
        private System.Collections.IEnumerator InitialSetting_SpawnByLoadFile()
        {
            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;

            yield return new WaitForSeconds(Time.fixedDeltaTime);

            this.enemyPresenter.GetNextDirection();
            monsterSpriteRenderer.flipX = this.nextDirection;

            this.enemyState = EnemyState.Idle;

            yield return new WaitForSeconds(Time.fixedDeltaTime * 3);

            this.isCreateArrange = false;
            this.isReady = false;

            this.gameObject.tag = "Enemy";
            this.gameObject.layer = 7;
        }
        private System.Collections.IEnumerator HurtReady()
        {
            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;

            yield return new WaitForSeconds(this.monsterAnimationView.GetHurtAnimationTime() + Time.fixedDeltaTime);

            this.isReady = true;
            this.enemyState = EnemyState.Idle;
            this.monsterAnimationView.IdleAnimation();

            yield return new WaitForSeconds(1.0f);

            this.isReady = false;
            this.isAction = false;
            this.isHurt = false;

            this.gameObject.tag = "Enemy";
            this.gameObject.layer = 7;
        }
        private System.Collections.IEnumerator AttackReady()
        {
            yield return new WaitForSeconds(this.monsterAnimationView.GetAttackPhase01AnimationTime());

            this.enemyPresenter.SpawnNormalAttack();

            yield return new WaitForSeconds(this.monsterAnimationView.GetAttackPhase02AnimationTime());

            this.isReady = true;
            this.enemyState = EnemyState.Idle;
            this.monsterAnimationView.IdleAnimation();

            yield return new WaitForSeconds(1.5f);

            this.isReady = false;
            this.isAction = false;
            this.isAttack = false;
        }
        private System.Collections.IEnumerator DieEnemy()
        {
            this.gameObject.tag = "NonCollisionObject";
            this.gameObject.layer = 10;

            this.enemyPresenter.RemoveEnemyView();

            yield return new WaitForSeconds(this.monsterAnimationView.GetDieAnimationTime());

            this.enemyPresenter.SpawnCorpse();

            this.enemyPresenter = null;
            Destroy(this.gameObject);
        }
        private void StopAttack()
        {
            StopCoroutine("AttackReady");
            this.monsterAnimationView.StopAttackAnimation();

            this.isReady = false;
            this.isAttack = false;
        }

        public void OnCollisionWithAttackObject(int damage)
        {
            this.isAction = true;

            this.enemyCurrentHP = this.enemyCurrentHP - damage;

            if (this.enemyCurrentHP > 0)
            {
                this.enemyState = EnemyState.Hurt;
            }
            else
            {
                this.enemyState = EnemyState.Die;
            }

            this.StopAttack();
        }

        public void Destroy()
        {
            this.enemyPresenter.RemoveEnemyView();
            this.enemyPresenter = null;
            Destroy(this.gameObject);
        }
    }
}