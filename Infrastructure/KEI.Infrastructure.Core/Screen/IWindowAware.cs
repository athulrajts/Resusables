using System;

namespace KEI.Infrastructure
{
    public interface IWindowAware : IClosable, IMaximizable, IMinimizable
    {
    }

    public interface IClosable
    {
        public Action Close { get; set; }
    }

    public interface IMaximizable
    {
        public Action Maximize { get; set; }
        public Action RestoreDown { get; set; }
    }

    public interface IMinimizable
    {
        public Action Minimize { get; set; }
    }
}
