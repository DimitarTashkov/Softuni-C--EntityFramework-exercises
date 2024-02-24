using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFIntro.Models;

public partial class Minion
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? Name { get; set; }

    public int? Age { get; set; }

    public int? TownId { get; set; }

    [ForeignKey("TownId")]
    [InverseProperty("Minions")]
    public virtual Town? Town { get; set; }

    [ForeignKey("MinionId")]
    [InverseProperty("Minions")]
    public virtual ICollection<Villain> Villains { get; set; } = new List<Villain>();
}
