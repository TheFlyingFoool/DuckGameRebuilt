using Steamworks;
using System;
using System.Text;

public class SharpSteam : IDisposable {

    public unsafe SharpSteam() {
    }

    public bool Init() {
        return SteamAPI.Init();
    }

    public void Shutdown() {
        SteamAPI.Shutdown();
    }

    public void SetTempDirectory(string dir) {
        // _tempDirectory was only used in FormatName..?!
        throw new NotImplementedException();
    }

    public unsafe bool SetAchievement(string name) {
        return SteamUserStats.SetAchievement(name);
    }

    public unsafe bool StoreStats() {
        return SteamUserStats.StoreStats();
    }

    public unsafe bool SetStat(string name, int value) {
        return SteamUserStats.SetStat(name, value);
    }

    public unsafe bool IndicateAchievementProgress(string name, int cur, int maxval) {
        return SteamUserStats.IndicateAchievementProgress(name, (uint) cur, (uint) maxval);
    }

    public unsafe string FormatName(string name) {
        // Invalid signature anyway...
        throw new NotImplementedException();
    }

    public unsafe bool FileExists(string name) {
        return SteamRemoteStorage.FileExists(name);
    }

    public unsafe bool FileWrite(string managedname, string manageddata) {
        byte[] data = Encoding.ASCII.GetBytes(manageddata);
        return SteamRemoteStorage.FileWrite(managedname, data, data.Length);
    }

    public unsafe string FileRead(string managedname) {
        int size = SteamRemoteStorage.GetFileSize(managedname);
        byte[] data = new byte[size];
        int rv = SteamRemoteStorage.FileRead(managedname, data, size);
        return Encoding.ASCII.GetString(data, 0, size);
    }

    public unsafe string GetUsername() {
        return SteamFriends.GetPersonaName();
    }

    public unsafe bool LoadLeaderboard(string board, string type, int minRange, int maxRange) {
        throw new NotImplementedException();
    }

    public unsafe bool DownloadedLeaderboard(string board) {
        throw new NotImplementedException();
    }

    public unsafe bool UploadScoreFailed() {
        throw new NotImplementedException();
    }

    public unsafe bool UploadScoreComplete() {
        throw new NotImplementedException();
    }

    public unsafe bool DownloadScoresFailed() {
        throw new NotImplementedException();
    }

    public unsafe string GetLeaderboardName(string board, int index) {
        throw new NotImplementedException();
        // SteamUserStats.GetLeaderboardName(/*... this requires a SteamLeaderboard_t!*/);
    }

    public unsafe int GetLeaderboardScore(string board, int index) {
        throw new NotImplementedException();
    }

    public unsafe int GetLeaderboardPosition(string board, int index) {
        throw new NotImplementedException();
    }

    public unsafe int GetLeaderboardDetails(string board, int index) {
        throw new NotImplementedException();
    }

    public unsafe bool UploadScore(string board, int score, int details) {
        throw new NotImplementedException();
        // SteamUserStats.UploadLeaderboardScore(/*... this requires a SteamLeaderboard_t!*/, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, /*???*/, /*???*/);
    }

    public void RunCallbacks() 
    {
        SteamAPI.RunCallbacks();
    }

    public unsafe void ThrowException(string message, string error) {
        throw new NotSupportedException();
        // NativeMethods isn't exposed, but those would need to be called:
        // Steamworks.NativeMethods.SteamAPI_SetMiniDumpComment
        // Steamworks.NativeMethods.SteamAPI_WriteMiniDump
    }

    public unsafe void OpenInviteDialogue() {
        throw new NotImplementedException();
        // SteamFriends.ActivateGameOverlayInviteDialog(/*requires current lobby*/);
    }

    public void CreateLobby(int lobbyType, int maxMembers) {
        SteamMatchmaking.CreateLobby((ELobbyType) lobbyType, maxMembers);
    }

    public unsafe bool CreateLobbyComplete() {
        throw new NotImplementedException();
    }

    public unsafe ulong GetCreatedLobbyID() {
        throw new NotImplementedException();
    }

    protected virtual unsafe void Dispose(bool flag) {
    }

    public void Dispose() {
        Dispose(true);
    }

}
