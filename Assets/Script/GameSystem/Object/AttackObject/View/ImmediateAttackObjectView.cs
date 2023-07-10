using UnityEngine;

namespace GameSystem.AttackObject
{
    public interface IImmediateAttackObjectView
    {
        public void InitialSetting(ImmediateAttackObjectStruct immediateAttackObjectStruct, IAttackObjectManagerForViews attackObjectManager);
        public void OnDestroy();
    }

    public class ImmediateAttackObjectView : MonoBehaviour, IImmediateAttackObjectView
    {
        private IAttackObjectManagerForViews attackObjectManager;

        private Rigidbody2D objectRigidbody2D;

        private ImmediateAttackObjectStruct immediateAttackObjectStruct;
        private bool inDestroy;
        private bool moveDirection;

        public void InitialSetting(ImmediateAttackObjectStruct immediateAttackObjectStruct, IAttackObjectManagerForViews attackObjectManager)
        {
            this.immediateAttackObjectStruct = immediateAttackObjectStruct;
            this.attackObjectManager = attackObjectManager;
            this.attackObjectManager.RegisterImmediateAttackObjectView(this);
        }

        private void Start()
        {
            this.objectRigidbody2D = GetComponent<Rigidbody2D>();
            this.objectRigidbody2D.position = this.immediateAttackObjectStruct.PublicSkillStruct.StartPosition;

            this.gameObject.tag = this.immediateAttackObjectStruct.PublicSkillStruct.AttackObjectTag;
            this.gameObject.layer = this.immediateAttackObjectStruct.PublicSkillStruct.AttackObjectLayer;
            this.moveDirection = this.immediateAttackObjectStruct.PublicSkillStruct.MoveDirection;

            StopCoroutine("Attack");
            StartCoroutine("Attack");
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            this.immediateAttackObjectStruct.HitCount--;

            if (!this.inDestroy)
            {
                if (collision.gameObject.tag.Equals("Enemy"))
                {
                    collision.gameObject.GetComponent<Enemy.EnemyView>().OnCollisionWithAttackObject(this.immediateAttackObjectStruct.Damage);
                }
                else if (collision.gameObject.tag.Equals("Summon"))
                {
                    collision.gameObject.GetComponent<Summon.SummonView>().OnCollisionWithAttackObject(this.immediateAttackObjectStruct.Damage);
                }
                else if (collision.gameObject.tag.Equals("Player"))
                {
                    collision.gameObject.GetComponent<Player.PlayerController>().OnCollisionWithAttackObject(this.immediateAttackObjectStruct.Damage);
                }

                if (this.immediateAttackObjectStruct.HitCount == 0)
                {
                    this.inDestroy = true;
                    this.OnDestroy();           // 나중에 각 Projectile와 연결된 Effect 애니메이션과 연결되는 곳으로 변경할할 수 있다.
                }
            }
        }

        private System.Collections.IEnumerator Attack()
        {
            yield return new WaitForSeconds(this.immediateAttackObjectStruct.InAttackTime);

            this.OnDestroy();
        }

        public void OnDestroy()
        {
            this.attackObjectManager.RemoveImmediateAttackObjectView(this);
            Destroy(this.gameObject);
        }
    }
}
