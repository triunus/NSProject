using UnityEngine;

namespace GameSystem.AttackObject
{
    public interface ITrapObjectView
    {
        public void InitialSetting(TrapObjectStruct trapObjectStruct, IAttackObjectManagerForViews attackObjectManager);
        public void OnDestroy();
    }

    public class TrapObjectView : MonoBehaviour, ITrapObjectView
    {
        private IAttackObjectManagerForViews attackObjectManager;

        private Rigidbody2D objectRigidbody2D;

        private TrapObjectStruct trapObjectStruct;
        private bool moveDirection;

        public void InitialSetting(TrapObjectStruct trapObjectStruct, IAttackObjectManagerForViews attackObjectManager)
        {
            this.trapObjectStruct = trapObjectStruct;
            this.attackObjectManager = attackObjectManager;
            this.attackObjectManager.RegisterTrapObjectView(this);
        }

        private void Start()
        {
            this.objectRigidbody2D = GetComponent<Rigidbody2D>();
            this.objectRigidbody2D.position = this.trapObjectStruct.PublicSkillStruct.StartPosition;

            this.gameObject.tag = this.trapObjectStruct.PublicSkillStruct.AttackObjectTag;
            this.gameObject.layer = this.trapObjectStruct.PublicSkillStruct.AttackObjectLayer;
            this.moveDirection = this.trapObjectStruct.PublicSkillStruct.MoveDirection;
        }

        private void FixedUpdate()
        {

        }

        public void OnDestroy()
        {
            this.attackObjectManager.RemoveTrapObjectView(this);
            Destroy(this.gameObject);
        }
    }
}