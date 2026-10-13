using System;
using System.Collections.Generic;

namespace Northwind.DbAccess.Models;

public partial class UserAccount
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? EmployeeId { get; set; }

    public string? CustomerId { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public int CountLog { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }
}
