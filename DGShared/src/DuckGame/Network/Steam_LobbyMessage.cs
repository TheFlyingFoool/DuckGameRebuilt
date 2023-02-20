// Decompiled with JetBrains decompiler
// Type: DuckGame.Steam_LobbyMessage
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Steam_LobbyMessage
    {
        public const string M_CommunicationFailure = "COM_FAIL";
        public const string M_ImOuttaHere = "IM_OUTTAHERE";
        public User from;
        public User context;
        public string message;
        private static long kLobbyMessageID = 10968107910803936;

        public static void Send(string pMessage, User pContext)
        {
            BitBuffer bitBuffer = new BitBuffer(false);
            bitBuffer.Write(kLobbyMessageID);
            if (pContext != null)
                bitBuffer.Write(pContext.id);
            else
                bitBuffer.Write(0UL);
            bitBuffer.Write(pMessage);
            Steam.SendLobbyMessage(Network.activeNetwork.core.lobby, bitBuffer.buffer, (uint)bitBuffer.lengthInBytes);
        }

        public static Steam_LobbyMessage Receive(User pFrom, byte[] pData)
        {
            try
            {
                Steam_LobbyMessage steamLobbyMessage = new Steam_LobbyMessage();
                BitBuffer bitBuffer = new BitBuffer(pData, false);
                if (bitBuffer.ReadLong() == kLobbyMessageID)
                {
                    steamLobbyMessage.from = pFrom;
                    ulong id = bitBuffer.ReadULong();
                    if (id != 0UL)
                        steamLobbyMessage.context = User.GetUser(id);
                    steamLobbyMessage.message = bitBuffer.ReadString();
                }
                return steamLobbyMessage;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
