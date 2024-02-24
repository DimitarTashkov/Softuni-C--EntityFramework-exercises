using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFIntro.Models;

public partial class Town
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    public int? CountryCode { get; set; }

    [ForeignKey("CountryCode")]
    [InverseProperty("Towns")]
    public virtual Country? CountryCodeNavigation { get; set; }

    [InverseProperty("Town")]
    public virtual ICollection<Minion> Minions { get; set; } = new List<Minion>();
}
