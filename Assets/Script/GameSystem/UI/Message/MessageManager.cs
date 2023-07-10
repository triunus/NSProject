
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameSystem.Message
{
    public interface IMessageManager
    {
        public void SpawnMessage(int messageCode, int fileIndex = 0);
    }

    public class MessageManager : MonoBehaviour, IMessageManager
    {
        private JArray messageTable;
        private int messageCode;
        private string[] messageRow = new string[3];

        private int fileIndex;

        private void Awake()
        {
            TextAsset messageText = Resources.Load<TextAsset>("GameSystem/MessageData/MessageData");
            this.messageTable = JArray.Parse(messageText.ToString());
        }

        public void SpawnMessage(int messageCode, int fileIndex = 0)
        {
            this.messageCode = messageCode;
            this.fileIndex = fileIndex;
            this.FindMessage();

            RectTransform messageView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/ErrorMessage"));
            messageView.GetComponent<IMessageView>().InitialSetting(this.messageRow, this.fileIndex);
        }

        private void FindMessage()
        {
            for (int i = 0; i < this.messageTable.Count; i++)
            {
                if (System.Convert.ToInt32(this.messageTable[i]["MessageCode"]) == this.messageCode)
                {
                    messageRow[0] = (string)this.messageTable[i]["MessageCode"];
                    messageRow[1] = (string)this.messageTable[i]["Title"];
                    messageRow[2] = (string)this.messageTable[i]["Detail"];
                }
            }
        }
    }
}
