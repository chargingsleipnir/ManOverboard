namespace ZeroProgress.Interactions
{
    public interface IToggleable
    {
        void TurnOn();

        void TurnOff();

        void Toggle();

        bool IsOn();
    }
}
