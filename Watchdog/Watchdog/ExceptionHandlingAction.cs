namespace Watchdog
{
    public enum ExceptionHandlingAction
    {
        DefaultBehaviour,
        CallEventHandler,
        CallEventHandlerAndLeakException
    }
}
