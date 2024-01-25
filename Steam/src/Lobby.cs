using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

public class Lobby : IDisposable {

    public int randomID; // Duck Game uses this... BUT IT ISN'T USED ANYWHERE IN Steam.dll?!

    public delegate void UserStatusChangeDelegate(User user, SteamLobbyUserStatusFlags flags, User responsibleUser);



    public event UserStatusChangeDelegate UserStatusChange;
    public delegate void ChatMessageDelegate(User user, byte[] data);
    public event ChatMessageDelegate ChatMessage;


    private CSteamID _id;
    public ulong id => _id.m_SteamID;

    public SteamLobbyJoinResult joinResult { get; private set; }

    public unsafe User owner {
        get {
            try
            {
                if (id != 0 && Steam.initialized)
                {
                    return User.GetUser(SteamMatchmaking.GetLobbyOwner(_id));
                }
            }
            catch { }
            return null;
        }
        set
        {
            SteamMatchmaking.SetLobbyOwner((CSteamID)this.id, (CSteamID)value.id);
        }
    }
    public unsafe void SetLobbyModsData(string value)
    {
        if (id != 0 && Steam.initialized)
        {
            SteamMatchmaking.SetLobbyData(_id, "mods", value);
        }
    }
    public unsafe string name {
        get
        {
            if (id != 0 && Steam.initialized)
            {
                return SteamMatchmaking.GetLobbyData(_id, "name");
            }
            return "";
        }
        set {
            if (id != 0 && Steam.initialized) {
                SteamMatchmaking.SetLobbyData(_id, "name", value);
            }
        }
    }

    private bool _joinable;
    public unsafe bool joinable {
        get {
            return id != 0 && _joinable;
        }
        set {
            if (id != 0 && Steam.initialized) {
                SteamMatchmaking.SetLobbyJoinable(_id, value);
                _joinable = value;
            }
        }
    }

    private SteamLobbyType _type;
    public unsafe SteamLobbyType type {
        get {
            return _type;
        }
        set {
            if (id != 0 && Steam.initialized) {
                SteamMatchmaking.SetLobbyType(_id, (ELobbyType) value);
                // This isn't set by the original Steam.dll...
                _type = value;
            }
        }
    }

    private int _maxMembers;
    public unsafe int maxMembers {
        get
        {
            if (id != 0 && Steam.initialized)
            {
                return _maxMembers = SteamMatchmaking.GetLobbyMemberLimit(_id);
            }
            return 0;
        }
        set {
            if (id != 0 && Steam.initialized) {
                SteamMatchmaking.SetLobbyMemberLimit(_id, _maxMembers = value);
            }
        }
    }

    public unsafe List<User> users {
        get
        {
            if (id != 0 && Steam.initialized)
            {
                return SteamHelper.GetList(SteamMatchmaking.GetNumLobbyMembers(_id), i => User.GetUser(SteamMatchmaking.GetLobbyMemberByIndex(_id, i)));
            }

            return new List<User>();
        }
    }
    public bool processing { get; private set; }

    public Lobby(ulong lobbyID)
        : this(new CSteamID(lobbyID)) {
    }

    internal Lobby(CSteamID lobbyID) {
        _type = SteamLobbyType.FriendsOnly;
        processing = true;
        _id = lobbyID;
        _joinable = true;
    }

    public Lobby(SteamLobbyType lobbyTypeVal, int maxMembersVal) {
        _type = lobbyTypeVal;
        // This is set to this.maxMembers by the original Steam.dll...
        _maxMembers = maxMembersVal;
        processing = true;
        _id = new CSteamID();
        _joinable = true;
    }

    public void OnProcessingComplete(ulong idVal, SteamLobbyJoinResult result) {
        _id = new CSteamID(idVal);
        joinResult = result;
        processing = false;
    }

    public void OnUserStatusChange(User user, SteamLobbyUserStatusFlags flags, User responsibleUser) {
        UserStatusChange?.Invoke(user, flags, responsibleUser);
    }
    public void OnChatMessage(User user, byte[] data)
    {
        ChatMessage?.Invoke(user, data);
    }

    public unsafe void SetLobbyData(string name, string value) {
        if (id != 0 && Steam.initialized)
            SteamMatchmaking.SetLobbyData(_id, name, value);
    }

    public unsafe string GetLobbyData(string name)
    {
        if (id != 0 && Steam.initialized)
            return SteamMatchmaking.GetLobbyData(_id, name);
        return "";
    }

    [HandleProcessCorruptedStateExceptions]
    protected virtual void Dispose(bool flag) {
    }

    public void Dispose() {
        Dispose(true);
    }

}
