using System.Collections;
using UnityEngine;

namespace GameSystem.ETCObject
{
    public interface ICrystalUseMessageView
    {
        public void InitialSettingCrystalView(Vector3 corpsePosition);
    }

    public class CrystalUseMessageView : MonoBehaviour, ICrystalUseMessageView
    {
        private Vector3 startPosition;

        public void InitialSettingCrystalView(Vector3  startPosition)
        {
            this.startPosition = startPosition;
        }

        private IEnumerator TextAction()
        {
            while (this.transform.position.y <= this.startPosition.y + 0.95f)
            {
                this.transform.position = Vector3.Lerp(this.startPosition, this.startPosition + new Vector3(0, 1, 0), 0.5f);

                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            this.OnDestroy();
        }

        private void OnDestroy()
        {
            Destroy(this.gameObject);
        }
    }
}