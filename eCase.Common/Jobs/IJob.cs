using System;
using System.Threading;

namespace eCase.Common.Jobs
{
    public interface IJob : IDisposable
    {
        string Name { get; }
        TimeSpan Period { get; }
        void Action(CancellationToken token);
    }
}