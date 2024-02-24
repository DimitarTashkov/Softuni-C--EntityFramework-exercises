using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFIntro.Models;

public partial class EvilnessFactor
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    [InverseProperty("EvilnessFactor")]
    public virtual ICollection<Villain> Villains { get; set; } = new List<Villain>();
}
