using Steamworks;

// THIS TYPE DOESN'T EXIST IN THE ORIGINAL Steam.dll!
public class OfflineSelfUser : User {

    public override ulong id => 0;

    public override unsafe string name => "UNKNOWN";

    public override unsafe byte[] avatarSmall => null;

    public override unsafe byte[] avatarMedium => null;

    public override unsafe bool inGame => true;

    public override unsafe bool inCurrentGame => true;

    protected override unsafe bool inLobby => false;

    public override unsafe bool inCurrentLobby => false;

    public override unsafe SteamUserState state => SteamUserState.Offline;

    public override unsafe FriendRelationship relationship => FriendRelationship.None;

    internal OfflineSelfUser()
        : base(new CSteamID()) {
    }

}
