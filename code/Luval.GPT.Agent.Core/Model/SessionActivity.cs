using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luval.GPT.Agent.Core.Activity;

namespace Luval.GPT.Agent.Core.Model
{
    public class SessionActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }
        public Session? Session { get; set; }

        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateTime? UtcStartedOn { get; set; }
        public DateTime? UtcCompletedOn { get; set; }
        public double? DurationInSeconds { get; set; }
        public ExecutionStatus Status { get; set; }
        public int RetryCount { get; set; }
        public string? InputParameters { get; set; }
        public string? Result { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? UtcModifiedOn { get; set; }

        public int Version { get; set; }
    }
}
