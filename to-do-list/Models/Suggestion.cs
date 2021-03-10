using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class Suggestion
    {
        public int SuggestionId { get; set; }

        [Required(ErrorMessage = "Please enter your Feedback")]
        public string Details { get; set; }

        public string User { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime PostDate { get; set; }
    }
}
