using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonersMailsDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [JsonProperty(nameof(FullName))]
        public string FullName { get; set; }
        [Required]
        [RegularExpression("^The [A-Z][a-z]+$")]
        [JsonProperty(nameof(Nickname))]

        public string Nickname { get; set; }
        [Range(18, 65)]
        [JsonProperty(nameof(Age))]

        public int Age { get; set; }
        [Required]
        [JsonProperty(nameof(IncarcerationDate))]

        public string IncarcerationDate { get; set; }
        [JsonProperty(nameof(ReleaseDate))]

        public string ReleaseDate { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [JsonProperty(nameof(Bail))]
        public decimal? Bail { get; set; }
        [JsonProperty(nameof(CellId))]

        public int? CellId { get; set; }
        [JsonProperty("Mails")]

        public MailImportDto[] Mails { get; set; }
    }

    public class MailImportDto
    {
        [Required]
        [JsonProperty(nameof(Description))]

        public string Description { get; set; }
        [Required]
        [JsonProperty(nameof(Sender))]

        public string Sender { get; set; }
        [Required]
        [JsonProperty(nameof(Address))]
        [RegularExpression(@"^([A-Za-z\s0-9]+?)(\sstr\.)$")]
        public string Address { get; set; }
    }
}

//{
//    "FullName": "",
//    "Nickname": "The Wallaby",
//    "Age": 32,
//    "IncarcerationDate": "29/03/1957",
//    "ReleaseDate": "27/03/2006",
//    "Bail": null,
//    "CellId": 5,
//    "Mails": [
//      {
//        "Description": "Invalid FullName",
//        "Sender": "Invalid Sender",
//        "Address": "No Address"
//      },
