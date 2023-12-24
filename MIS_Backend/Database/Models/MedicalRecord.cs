using System;
using System.Collections.Generic;

namespace MIS_Backend.Database.Models;

public partial class MedicalRecord
{
    public Guid Id { get; set; }

    public int? Actual { get; set; }

    public string? MkbCode { get; set; }

    public string? MkbName { get; set; }

    public string? RecCode { get; set; }

    public Guid? IdParent { get; set; }

    public DateTime? Createtime { get; set; }

    public Guid? Root { get; set; }
}
