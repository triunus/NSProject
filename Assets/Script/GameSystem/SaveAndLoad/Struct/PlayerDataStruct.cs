using System;

namespace GameSystem.SaveAndLoad
{
    [Serializable]
    public class PlayerDataStruct
    {
        private Player.PlayerState playerState;
        private float playerPositionX;
        private float playerPositionY;
        private bool moveDirection;
        private int currentHP;

        private int ownManaStone;

        public PlayerDataStruct(Player.PlayerState playerState = Player.PlayerState.Idle, float playerPositionX = -6f, float playerPositionY = -3f, bool moveDirection = false,
            int currentHP = 10, int ownManaStone = 0)
        {
            this.playerState = playerState;
            this.playerPositionX = playerPositionX;
            this.playerPositionY = playerPositionY;
            this.moveDirection = moveDirection;
            this.currentHP = currentHP;

            this.ownManaStone = ownManaStone;
        }

        public Player.PlayerState PlayerState { get { return this.playerState; } set { this.playerState = value; } }
        public float PlayerPositionX { get { return this.playerPositionX; } set { this.playerPositionX = value; } }
        public float PlayerPositionY { get { return this.playerPositionY; } set { this.playerPositionY = value; } }
        public bool MoveDirection { get { return this.moveDirection; } set { this.moveDirection = value; } }
        public int CurrentHP { get { return this.currentHP; } set { this.currentHP = value; } }


        public int OwnManaStone { get { return this.ownManaStone; } set { this.ownManaStone = value; } }
    }
}