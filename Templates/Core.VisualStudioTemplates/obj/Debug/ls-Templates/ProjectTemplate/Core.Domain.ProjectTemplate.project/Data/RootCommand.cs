using $safeprojectname$.Domain;

namespace $safeprojectname$.Data
{
    /// <summary>
    /// Base class for all commands that have the root as the target. I.e., those that have no logical parent.
    /// </summary>
    /// <typeparam name="TResult">The type of result that the command creates.</typeparam>
    public abstract class RootCommand<TResult> : DomainCommand<Root, TResult>
    {
        protected override void OnInitialiseTarget()
        {
            base.OnInitialiseTarget();
            Target = new Root();
        }
    }
}