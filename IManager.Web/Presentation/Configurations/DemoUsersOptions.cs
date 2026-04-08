namespace IManager.Web.Presentation.Configurations;

public class DemoUsersOptions
{
    public DemoUserOptions Admin { get; set; } = default!;
    public DemoUserOptions Staff { get; set; } = default!;
    public DemoUserOptions User { get; set; } = default!;
}