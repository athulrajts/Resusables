using System.Collections.Generic;

namespace KEI.Infrastructure.Utils
{
    public class CommandLineArguments
    {
        private readonly List<string> _args;
        public CommandLineArguments(string[] args)
        {
            _args = new List<string>(args);
        }

        public bool TryGetValue(string flag, ref string value)
        {
            var flagWithHiphen = $"-{flag}";

            if(_args.Contains(flagWithHiphen))
            {
                var pos = _args.IndexOf(flagWithHiphen);

                if(_args.Count > pos + 1)
                {
                    value = _args[pos + 1];
                    return true;
                }
            }

            if (_args.Contains(flag))
            {
                var pos = _args.IndexOf(flag);

                if (_args.Count > pos + 1)
                {
                    value = _args[pos + 1];
                    return true;
                }
            }

            return false;
        }
    }
}
