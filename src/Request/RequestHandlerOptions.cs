namespace EasyRequestHandlers.Request
{
    public class RequestHandlerOptions
    {
        internal bool EnableMediatorPattern { get; set; }

        internal bool EnableRequestHooks { get; set; }

        internal bool EnableHandlerInjection { get; set; } = true;
    }
}
