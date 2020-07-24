using System;
using System.Runtime.CompilerServices;
using KEI.Infrastructure.Service;

namespace KEI.Infrastructure
{
    public interface ILogManager
    {
        ILogger DefaultLogger { get; }
        ILogger GetLogger([CallerFilePath]string p_strName = "");
        ILogger GetLogger(Type p_Type);
    }
}
