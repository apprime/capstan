namespace Capstan.Core
{
    /// <summary>
    /// An activist is a class that is scheduled to do things.
    /// These things are not reaction to other things, 
    /// they are simply created by the system.
    /// </summary>
    public interface Activist
    {
        //Todo: Make async
        bool Condition();
        void Activate();
    }
}
