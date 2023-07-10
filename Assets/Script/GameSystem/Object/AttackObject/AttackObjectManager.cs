using UnityEngine;

namespace GameSystem.AttackObject
{
    public enum SkillName
    {
        Lightning = 1,
        SwordSlash = 51,
        LightningBalt = 101,
        ArrowShooting = 151
    }

    public enum SkillType
    {
        ImmediateAttack,
        Projectile,
        Trap
    }

    public interface IAttackObjectManagerForGameManager
    {
        public void Clearing();
    }

    public interface IAttackObjectManager
    {
        public void SpawnAttackObject(PublicSkillStruct publicSkillStruct);
    }

    public interface IAttackObjectManagerForViews
    {
        public void RegisterImmediateAttackObjectView(IImmediateAttackObjectView view);
        public void RemoveImmediateAttackObjectView(IImmediateAttackObjectView view);

        public void RegisterProjectileObjectView(IProjectileObjectView view);
        public void RemoveProjectileObjectView(IProjectileObjectView view);

        public void RegisterTrapObjectView(ITrapObjectView view);
        public void RemoveTrapObjectView(ITrapObjectView view);
    }


    public class AttackObjectManager : MonoBehaviour, IAttackObjectManagerForGameManager, IAttackObjectManager, IAttackObjectManagerForViews
    {
        private System.Collections.Generic.List<IImmediateAttackObjectView> immediateAttackObjectViews = new System.Collections.Generic.List<IImmediateAttackObjectView>();
        private System.Collections.Generic.List<IProjectileObjectView> projectileObjectViews = new System.Collections.Generic.List<IProjectileObjectView>();
        private System.Collections.Generic.List<ITrapObjectView> trapObjectViews = new System.Collections.Generic.List<ITrapObjectView>();

        public void RegisterImmediateAttackObjectView(IImmediateAttackObjectView view)
        {
            this.immediateAttackObjectViews.Add(view);
        }
        public void RemoveImmediateAttackObjectView(IImmediateAttackObjectView view)
        {
            this.immediateAttackObjectViews.Remove(view);
        }

        public void RegisterProjectileObjectView(IProjectileObjectView view)
        {
            this.projectileObjectViews.Add(view);
        }
        public void RemoveProjectileObjectView(IProjectileObjectView view)
        {
            this.projectileObjectViews.Remove(view);
        }

        public void RegisterTrapObjectView(ITrapObjectView view)
        {
            this.trapObjectViews.Add(view);
        }
        public void RemoveTrapObjectView(ITrapObjectView view)
        {
            this.trapObjectViews.Remove(view);
        }

        private void Awake()
        {
            this.immediateAttackObjectViews = new System.Collections.Generic.List<IImmediateAttackObjectView>();
            this.projectileObjectViews = new System.Collections.Generic.List<IProjectileObjectView>();
            this.trapObjectViews = new System.Collections.Generic.List<ITrapObjectView>();
        }

        // IAttackObjectManagerForGameManager 구현
        public void Clearing()
        {
            if (this.immediateAttackObjectViews is null) { }
            else
            {
                while (this.immediateAttackObjectViews.Count != 0)
                {
                    this.immediateAttackObjectViews[0].OnDestroy();
                }
            }

            if (this.projectileObjectViews is null) { }
            else
            {
                while (this.projectileObjectViews.Count != 0)
                {
                    this.projectileObjectViews[0].OnDestroy();
                }
            }

            if (this.trapObjectViews is null) { }
            else
            {
                while (this.trapObjectViews.Count != 0)
                {
                    this.trapObjectViews[0].OnDestroy();
                }
            }
        }

        // IAttackObjectManager 구현
        public void SpawnAttackObject(PublicSkillStruct publicSkillStruct)
        {
            switch (publicSkillStruct.SkillType)
            {
                case SkillType.ImmediateAttack:
                    ImmediateAttackObjectStruct immediateAttackObjectStruct = new ImmediateAttackObjectStruct(publicSkillStruct);

                    IImmediateAttackObjectView immediateAttackObjectView = Instantiate(Resources.Load<GameObject>("Prefab/AttackObject/" + publicSkillStruct.SkillName.ToString())).GetComponent<ImmediateAttackObjectView>();
                    immediateAttackObjectView.InitialSetting(immediateAttackObjectStruct, this);
                    break;
                case SkillType.Projectile:
                    ProjectileObjectStruct projectileObjectStruct = new ProjectileObjectStruct(publicSkillStruct);

                    IProjectileObjectView projectileObjectView = Instantiate(Resources.Load<GameObject>("Prefab/AttackObject/" + publicSkillStruct.SkillName.ToString())).GetComponent<ProjectileObjectView>();
                    projectileObjectView.InitialSetting(projectileObjectStruct, this);
                    break;
                case SkillType.Trap:
                    TrapObjectStruct trapObjectStruct = new TrapObjectStruct(publicSkillStruct);

                    ITrapObjectView trapObjectView = Instantiate(Resources.Load<GameObject>("Prefab/AttackObject/" + publicSkillStruct.SkillName.ToString())).GetComponent<TrapObjectView>();
                    trapObjectView.InitialSetting(trapObjectStruct, this);
                    break;
                default:
                    break;
            }
        }

        // 해당 부분은 공격 패턴이 다양하고 복잡해지면 사용되고, 점점 복잡하고 비대해지는 부분이다.
        // 차후 다른 클래스에 위임하여 사용할 수도 있다.
        // private void SpawnComplicatedAttackObject(AttackType attackType, Vector3 startPosition) { }
    }
}