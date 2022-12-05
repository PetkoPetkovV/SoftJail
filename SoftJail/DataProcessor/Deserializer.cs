namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var departmetCells = JsonConvert.DeserializeObject<ImportDepartmentCellDto[]>(jsonString);

            var validDepartments = new List<Department>();

            foreach (var department in departmetCells)
            {
                if (!IsValid(department) || !department.Cells.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (department.Cells.Length == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var newDepartment = new Department()
                {
                    Name = department.Name,
                };

                foreach (var cell in department.Cells)
                {
                    var newCell = new Cell()
                    {
                        CellNumber = cell.CellNumber,
                        HasWindow = cell.HasWindow
                    };

                    newDepartment.Cells.Add(newCell);
                }

                validDepartments.Add(newDepartment);

                sb.AppendLine($"Imported {newDepartment.Name} with {newDepartment.Cells.Count} cells");

            }
            context.Departments.AddRange(validDepartments);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            ImportPrisonersMailsDto[] prisonerMails = JsonConvert.DeserializeObject<ImportPrisonersMailsDto[]>(jsonString);

            var validPrisoners = new List<Prisoner>();

            foreach (var prisoner in prisonerMails)
            {
                if (!IsValid(prisoner) || !prisoner.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (string.IsNullOrEmpty(prisoner.FullName) || string.IsNullOrEmpty(prisoner.FullName))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var validIncarcerationDate = DateTime.TryParseExact(prisoner.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime incDate);

             

                if (!validIncarcerationDate) 
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime? releaseDate = null;

                if (!string.IsNullOrEmpty(prisoner.ReleaseDate))
                {
                    var validReleaseDate = DateTime.TryParseExact(prisoner.ReleaseDate, "dd/MM/yyyy",
                 CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime realReleaseDate);
                    if (!validReleaseDate)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    releaseDate = realReleaseDate;
                }

                var newPrisoner = new Prisoner()
                {
                    FullName = prisoner.FullName,
                    Nickname = prisoner.Nickname,
                    Age = prisoner.Age,
                    IncarcerationDate = incDate,
                    ReleaseDate = releaseDate,
                    Bail = prisoner.Bail,
                    CellId = prisoner.CellId.Value,
                };

                foreach (var mail in prisoner.Mails)
                {
                    var newMail = new Mail()
                    {
                        Description = mail.Description,
                        Sender = mail.Sender,
                        Address = mail.Address,
                    };

                    newPrisoner.Mails.Add(newMail); 
                }

                validPrisoners.Add(newPrisoner);
                sb.AppendLine($"Imported {newPrisoner.FullName} {newPrisoner.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var textReader = new StringReader(xmlString);

            var serializer = new XmlSerializer(typeof(ImportOfficerPrisonerDto[]), new XmlRootAttribute("Officers"));

            var officerPrisonersDto = serializer.Deserialize(textReader) as ImportOfficerPrisonerDto[];

            var validOfficers = new List<Officer>();

            foreach (var officer in officerPrisonersDto)
            {
                if (!IsValid(officer))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (officer.Salary < 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var offPosition = Enum.TryParse(typeof(Position), officer.Position, out var position);
                var offWeapon = Enum.TryParse(typeof(Weapon), officer.Weapon, out var weapon);

                if (!offPosition || !offWeapon)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (!context.Departments.Any(d => d.Id == officer.DepartmentId))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var newOfficer = new Officer()
                {
                    FullName = officer.FullName,
                    Salary = officer.Salary,
                    Position = (Position)position,
                    Weapon = (Weapon)weapon,
                    DepartmentId = officer.DepartmentId,
                };

                foreach (var prisoner in officer.Prisoners)
                {
                    var newPrisoner = new OfficerPrisoner()
                    {
                        Officer = newOfficer,
                        PrisonerId = prisoner.Id
                    };
                    newOfficer.OfficerPrisoners.Add(newPrisoner);
                }

                validOfficers.Add(newOfficer);

                sb.AppendLine($"Imported {newOfficer.FullName} ({newOfficer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(validOfficers);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}