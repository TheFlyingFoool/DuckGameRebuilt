namespace DuckGame
{
    public interface IEngineUpdatable
    {
        void PreUpdate();

        void Update();

        void PostUpdate();

        void OnDrawLayer(Layer pLayer);
    }
}
