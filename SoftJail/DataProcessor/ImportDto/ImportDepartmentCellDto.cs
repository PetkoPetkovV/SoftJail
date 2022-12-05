using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentCellDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        [JsonProperty(nameof(Name))]
        public string Name { get; set; }
        [JsonProperty("Cells")]
        public CellImportDto[] Cells { get; set; }
    }

    public class CellImportDto
    {
        [Required]
        [Range(1, 1000)]
        [JsonProperty(nameof(CellNumber))]
        public int CellNumber { get; set; }
        [Required]
        [JsonProperty(nameof(HasWindow))]
        public bool HasWindow { get; set; }
    }
}

//{
//    "Name": "",
//    "Cells": [
//      {
//        "CellNumber": 101,
//        "HasWindow": true
//      },
