using Sfa.Core.Reflection;

namespace Sfa.$safeprojectname$.Domain.Builders
{
    public class RootBuilder : ObjectBuilder<Root>
    {
        public RootBuilder()
            : base(Default)
        {

        }

        private static Root Default => new Root();
    }
}