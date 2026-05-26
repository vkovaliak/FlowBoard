namespace FlowBoard.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public required string EmailAddress { get; set; }

    public required string PasswordHash { get; set; }

    public DateTime SignupDate { get; set; }
}