using System.ComponentModel.DataAnnotations;

namespace Macellum.Models
{
    [MetadataType(typeof(PasswordMetaData))]
    public partial class Password
    {
        public string ConfirmPassword { get; set; }
    }

    public class PasswordMetaData
    {
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
// ReSharper disable InconsistentNaming
        public string pass { get; set; }
// ReSharper restore InconsistentNaming

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("pass", ErrorMessage = "Fields \"Password\" and \"Confirm Password\" must be equal.")]
        public string ConfirmPassword { get; set; }
    }
}