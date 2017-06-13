using Castle.DynamicProxy;

namespace WindowsService
{
    public class LoggingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            ExecuteBefore(invocation);
            invocation.Proceed();
            ExecuteAfter(invocation);
        }

        private void ExecuteAfter(IInvocation invocation)
        {
            Logger.LogAfter(invocation.Method.Name, invocation.ReturnValue);
        }

        private void ExecuteBefore(IInvocation invocation)
        {
            Logger.LogBefore(invocation.Method.Name, invocation.Arguments);
        }
    }
}
