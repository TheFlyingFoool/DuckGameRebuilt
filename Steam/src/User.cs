using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

public class User : IDisposable {

    private static Dictionary<ulong, User> _users;

    internal static User GetUser(CSteamID id) {
        return GetUser(id.m_SteamID);
    }

    public static User GetUser(ulong id) {
        if (id == 0) {
            return null;
        }
        if (_users == null) {
            _users = new Dictionary<ulong, User>();
        }
        using (Lock _lock = new Lock(_users)) {
            User user;
            if (!_users.TryGetValue(id, out user)) {
                user = new User(id);
                _users[id] = user;
            }
            return user;
        }
    }

    private CSteamID _id;
    public virtual ulong id => _id.m_SteamID;

    public virtual unsafe string name {
        get {
            if (id == 0 || !Steam.initialized)
                return "";
            return SteamFriends.GetFriendPersonaName(_id);
        }
    }

    private byte[] _avatarDataSmall;
    public virtual unsafe byte[] avatarSmall {
        get {
            if (id == 0 || !Steam.initialized)
                return null;
            if (_avatarDataSmall != null)
                return _avatarDataSmall;
            return _avatarDataSmall = SteamHelper.GetImageRGBA(SteamFriends.GetSmallFriendAvatar(_id));
        }
    }

    private byte[] _avatarDataMedium;
    public virtual unsafe byte[] avatarMedium {
        get {
            if (id == 0 || !Steam.initialized)
                return null;
            if (_avatarDataMedium != null)
                return _avatarDataMedium;
            return _avatarDataMedium = SteamHelper.GetImageRGBA(SteamFriends.GetMediumFriendAvatar(_id));
        }
    }

    public virtual unsafe bool inGame {
        get {
            if (id == 0 || !Steam.initialized)
                return false;
            FriendGameInfo_t game;
            return SteamFriends.GetFriendGamePlayed(_id, out game);
        }
    }

    public virtual unsafe bool inCurrentGame {
        get {
            if (id == 0 || !Steam.initialized)
                return false;
            FriendGameInfo_t game;
            return SteamFriends.GetFriendGamePlayed(_id, out game) && game.m_gameID.AppID() == SteamUtils.GetAppID();
        }
    }

    protected virtual unsafe bool inLobby {
        get {
            if (id == 0 || !Steam.initialized)
                return false;
            FriendGameInfo_t game;
            return SteamFriends.GetFriendGamePlayed(_id, out game) && game.m_steamIDLobby.m_SteamID != 0;
        }
    }

    public virtual unsafe bool inCurrentLobby {
        get {
            if (id == 0 || Steam.lobby == null || !Steam.initialized)
                return false;
            FriendGameInfo_t game;
            return SteamFriends.GetFriendGamePlayed(_id, out game) && game.m_steamIDLobby.m_SteamID != Steam.lobby.id;
        }
    }

    public virtual unsafe UserInfo info {
        get {
            return new UserInfo() {
                inGame = inGame,
                inCurrentGame = inCurrentGame,
                inLobby = inLobby,
                inMyLobby = inCurrentLobby,
                state = state,
                relationship = relationship
            };
        }
    }

    public virtual unsafe SteamUserState state {
        get {
            if (id == 0 || !Steam.initialized)
                return SteamUserState.Offline;
            return (SteamUserState) SteamFriends.GetFriendPersonaState(_id);
        }
    }

    public virtual unsafe FriendRelationship relationship {
        get {
            if (id == 0 || !Steam.initialized)
                return FriendRelationship.None;
            return (FriendRelationship) SteamFriends.GetFriendRelationship(_id);
        }
    }

    private User(ulong id) {
        _id = new CSteamID(id);
    }

    internal User(CSteamID id) {
        _id = id;
    }

    [HandleProcessCorruptedStateExceptions]
    protected virtual void Dispose(bool flag) {
    }

    public void Dispose() {
        Dispose(true);
    }

}
