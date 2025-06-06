﻿using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.Permission;

public class PermissionRequestParams
{
    public int? Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}