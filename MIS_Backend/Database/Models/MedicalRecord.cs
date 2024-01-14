using System;
using System.Collections.Generic;

namespace MIS_Backend.Database.Models;

public partial class MedicalRecord
{
    public Guid Id { get; set; }

    public int Actual { get; set; }

    public string MkbCode { get; set; } = null!;

    public string MkbName { get; set; } = null!;

    public string RecCode { get; set; } = null!;

    public Guid? IdParent { get; set; }

    public DateTime Createtime { get; set; }

    public Guid Root { get; set; }
}
