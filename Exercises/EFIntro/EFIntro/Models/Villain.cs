using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFIntro.Models;

public partial class Villain
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    public int? EvilnessFactorId { get; set; }

    [ForeignKey("EvilnessFactorId")]
    [InverseProperty("Villains")]
    public virtual EvilnessFactor? EvilnessFactor { get; set; }

    [ForeignKey("VillainId")]
    [InverseProperty("Villains")]
    public virtual ICollection<Minion> Minions { get; set; } = new List<Minion>();
}
