namespace DinoCars.Web.Components.Pages.Admin.Listings
{
    public enum PhotoActionKind
    {
        MoveLeft,
        MoveRight,
        MakeCover,
        Remove
    }

    public sealed record PhotoAction(Guid PhotoId, PhotoActionKind Kind);
}
