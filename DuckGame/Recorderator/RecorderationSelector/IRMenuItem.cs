namespace DuckGame
{
    public interface IRMenuItem
    {
        Rectangle Draw(Vec2 position, bool selected);
        void UpdateHovered();
        void OnSelect();
        void OnHover();
        void OnUnhover();
        int FolderSub { get; set; }
        IRMenuItem Parent { get; set; }
    }
}