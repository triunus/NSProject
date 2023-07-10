using UnityEngine;

namespace GameSystem.AttackObject
{
    public interface IProjectileObjectView
    {
        public void InitialSetting(ProjectileObjectStruct projectileObjectStruct, IAttackObjectManagerForViews attackObjectManager);
        public void OnDestroy();
    }

    public class ProjectileObjectView : MonoBehaviour, IProjectileObjectView
    {
        private IAttackObjectManagerForViews attackObjectManager;

        private Rigidbody2D objectRigidbody2D;

        private ProjectileObjectStruct projectileObjectStruct;
        private bool inDestroy;
        private bool moveDirection;

        public void InitialSetting(ProjectileObjectStruct projectileObjectStruct, IAttackObjectManagerForViews attackObjectManager)
        {
            this.projectileObjectStruct = projectileObjectStruct;
            this.attackObjectManager = attackObjectManager;
            this.attackObjectManager.RegisterProjectileObjectView(this);
        }

        private void Start()
        {
            this.objectRigidbody2D = GetComponent<Rigidbody2D>();

            this.objectRigidbody2D.position = this.projectileObjectStruct.PublicSkillStruct.StartPosition;
            this.gameObject.tag = this.projectileObjectStruct.PublicSkillStruct.AttackObjectTag;
            this.gameObject.layer = this.projectileObjectStruct.PublicSkillStruct.AttackObjectLayer;
            this.moveDirection = this.projectileObjectStruct.PublicSkillStruct.MoveDirection;
        }

        private void FixedUpdate()
        {
            if (!inDestroy)
            {
                if (Mathf.Abs(this.objectRigidbody2D.position.x - this.projectileObjectStruct.PublicSkillStruct.StartPosition.x) > this.projectileObjectStruct.LimitedRagne)
                {
                    this.OnDestroy();
                }
                else
                {
                    this.objectRigidbody2D.MovePosition(new Vector2(this.objectRigidbody2D.position.x + (this.projectileObjectStruct.PublicSkillStruct.MoveDirection == true ? -1 : 1) * this.projectileObjectStruct.Speed * Time.fixedDeltaTime, this.objectRigidbody2D.position.y));
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            this.projectileObjectStruct.HitCount--;

            if (!this.inDestroy)
            {
                if (collision.gameObject.tag.Equals("Enemy"))
                {
                    collision.gameObject.GetComponent<Enemy.EnemyView>().OnCollisionWithAttackObject(this.projectileObjectStruct.Damage);
                }
                else if (collision.gameObject.tag.Equals("Summon"))
                {
                    collision.gameObject.GetComponent<Summon.SummonView>().OnCollisionWithAttackObject(this.projectileObjectStruct.Damage);
                }
                else if (collision.gameObject.tag.Equals("Player"))
                {
                    collision.gameObject.GetComponent<Player.PlayerController>().OnCollisionWithAttackObject(this.projectileObjectStruct.Damage);
                }

                if (this.projectileObjectStruct.HitCount == 0)
                {
                    this.inDestroy = true;
                    this.OnDestroy();           // 나중에 각 Projectile와 연결된 Effect 애니메이션과 연결되는 곳으로 변경할할 수 있다.
                }
            }
        }

        public void OnDestroy()
        {
            this.attackObjectManager.RemoveProjectileObjectView(this);
            Destroy(this.gameObject);
        }
    }
}