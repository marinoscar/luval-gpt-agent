using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Luval.GPT.Chatbot.Data.Entities
{
    [Index(nameof(UserId))]
    public class ValidUser
    {
        public ValidUser()
        {
            UtcCreatedOn = DateTime.UtcNow;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public DateTime UtcCreatedOn { get; set; }
    }
}
