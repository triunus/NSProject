using System.Collections;
using UnityEngine;

namespace GameSystem.Monster
{
    public interface IMonsterAnimationView
    {
        public void IdleAnimation();
        public void WalkAnimation();
        public void RunAnimation();
        public void HurtAnimation();
        public void DieAnimation();
        public void ReviveAnimation();
        public void AttackAnimation();
        public void StopAttackAnimation();

        public float GetHurtAnimationTime();
        public float GetDieAnimationTime();
        public float GetRevivalAnimationTime();
        public float GetAttackPhase01AnimationTime();
        public float GetAttackPhase02AnimationTime();
    }

    public class MonsterAnimationView : MonoBehaviour, IMonsterAnimationView
    {
        private SpriteRenderer monsterSpriteRenderer;
        private Animator monsterAnimator;

        // Idle : 0, Walk : 1, Run : 2, Hurt : 3, Die : 4, Revive : 5, AttackPhase01 : 6, AttackPhase02 : 7, Skill01 : 8, Skill02 : 9
        private void Start()
        {
            this.monsterSpriteRenderer = GetComponent<SpriteRenderer>();
            this.monsterAnimator = GetComponent<Animator>();

/*            Debug.Log("animationClips 0 : " + monsterAnimator.runtimeAnimatorController.animationClips[0]);
            Debug.Log("animationClips 1 : " + monsterAnimator.runtimeAnimatorController.animationClips[1]);
            Debug.Log("animationClips 2 : " + monsterAnimator.runtimeAnimatorController.animationClips[2]);
            Debug.Log("animationClips 3 : " + monsterAnimator.runtimeAnimatorController.animationClips[3]);
            Debug.Log("animationClips 4 : " + monsterAnimator.runtimeAnimatorController.animationClips[4]);
            Debug.Log("animationClips 5 : " + monsterAnimator.runtimeAnimatorController.animationClips[5]);
            Debug.Log("animationClips 6 : " + monsterAnimator.runtimeAnimatorController.animationClips[6]);
            Debug.Log("animationClips 7 : " + monsterAnimator.runtimeAnimatorController.animationClips[7]);*/
        }

        public void IdleAnimation()
        {
            monsterAnimator.SetBool("isWalking", false);
            monsterAnimator.SetBool("isRunning", false);
        }
        public void WalkAnimation()
        {
            monsterAnimator.SetBool("isWalking", true);
            monsterAnimator.SetBool("isRunning", false);

        }
        public void RunAnimation()
        {
            monsterAnimator.SetBool("isWalking", false);
            monsterAnimator.SetBool("isRunning", true);
        }
        public void HurtAnimation()
        {
            StopCoroutine("HurtAction");
            StartCoroutine("HurtAction");
        }
        public void DieAnimation()
        {
            StopCoroutine("AttackAction");

            StopCoroutine("DieAction");
            StartCoroutine("DieAction");
        }
        public void ReviveAnimation()
        {
            StopCoroutine("ReviveAction");
            StartCoroutine("ReviveAction");
        }
        public void AttackAnimation()
        {
            StopCoroutine("AttackAction");
            StartCoroutine("AttackAction");
        }

        public void StopAttackAnimation()
        {
            this.monsterAnimator.SetBool("AttackPhase01", false);
            this.monsterAnimator.SetBool("AttackPhase02", false);

            StopCoroutine("AttackAction");
        }

        public float GetHurtAnimationTime()
        {
            return monsterAnimator.runtimeAnimatorController.animationClips[3].length;
        }
        public float GetDieAnimationTime()
        {
            return monsterAnimator.runtimeAnimatorController.animationClips[4].length;
        }
        public float GetRevivalAnimationTime()
        {
            return monsterAnimator.runtimeAnimatorController.animationClips[5].length;
        }
        public float GetAttackPhase01AnimationTime()
        {
            return monsterAnimator.runtimeAnimatorController.animationClips[6].length;
        }
        public float GetAttackPhase02AnimationTime()
        {
            return monsterAnimator.runtimeAnimatorController.animationClips[7].length;
        }

        private IEnumerator HurtAction()
        {
            float waitTime = monsterAnimator.runtimeAnimatorController.animationClips[4].length;

            this.monsterSpriteRenderer.color = Color.red;
            this.monsterAnimator.SetBool("isHurt", true);

            yield return new WaitForSeconds(waitTime);

            this.monsterSpriteRenderer.color = Color.white;
            this.monsterAnimator.SetBool("isHurt", false);
        }
        private IEnumerator DieAction()
        {
            float waitTime = monsterAnimator.runtimeAnimatorController.animationClips[5].length;

            this.monsterAnimator.SetBool("isDead", true);

            yield return new WaitForSeconds(waitTime);

            this.monsterSpriteRenderer.color = Color.black;
            this.monsterAnimator.SetBool("isDead", false);
        }
        private IEnumerator ReviveAction()
        {
            float waitTime = monsterAnimator.runtimeAnimatorController.animationClips[6].length;

            this.monsterAnimator.SetBool("isRevived", true);

            yield return new WaitForSeconds(waitTime);

            this.monsterSpriteRenderer.color = Color.white;
            this.monsterAnimator.SetBool("isRevived", false);
        }
        private IEnumerator AttackAction()
        {
            float AttackPhase01WaitTime = monsterAnimator.runtimeAnimatorController.animationClips[6].length;
            float AttackPhase02WaitTime = monsterAnimator.runtimeAnimatorController.animationClips[7].length;

            this.monsterAnimator.SetBool("AttackPhase01", true);

            yield return new WaitForSeconds(AttackPhase01WaitTime);

            this.monsterAnimator.SetBool("AttackPhase01", false);
            this.monsterAnimator.SetBool("AttackPhase02", true);

            yield return new WaitForSeconds(AttackPhase02WaitTime);

            this.monsterAnimator.SetBool("AttackPhase02", false);
        }
    }
}

