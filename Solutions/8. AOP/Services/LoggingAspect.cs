using PostSharp.Aspects;
using System;

namespace WindowsService
{
    [Serializable]
    public class LoggingAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            base.OnEntry(args);
            Logger.LogBefore(args.Method.Name, args.Arguments.ToArray());
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            base.OnSuccess(args);
            Logger.LogAfter(args.Method.Name, args.ReturnValue);
        }
    }
}
