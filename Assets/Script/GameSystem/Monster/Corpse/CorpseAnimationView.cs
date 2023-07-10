using System.Collections;
using UnityEngine;

namespace GameSystem.Corpse
{


    public interface ICorpseAnimationView
    {
        public void RevivalAnimation();
        public void ExtractionAnimation();
//        public void ExplosionAnimation();
    }
    public class CorpseAnimationView : MonoBehaviour, ICorpseAnimationView
    {
        public void RevivalAnimation()
        {

        }

        public void ExtractionAnimation()
        {

        }

    }
}
