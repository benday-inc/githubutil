using Benday.GitHubUtil.Api.Messages.ListProjectIterations;

namespace Benday.GitHubUtil.Api;
public static class ExtensionMethods
{
    public static bool IsCurrentIteration(this Iteration iteration)
    {
        var now = DateTime.Now;

        if (!DateTime.TryParse(iteration.StartDate, out var iterationStartDate))
        {
            return false;
        }

        if (iterationStartDate <= now && iterationStartDate.AddDays(iteration.Duration) >= now)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}