namespace Capstan.Events
{
    public interface IEventResult
    {
        EventResolutionType Resolution { get; set; }
        string Message { get; set; }
    }
}
