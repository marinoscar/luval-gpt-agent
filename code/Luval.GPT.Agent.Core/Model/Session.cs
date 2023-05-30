using Luval.GPT.Agent.Core.Activity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Model
{
    public class Session
    {
        public Session()
        {
            UtcModifiedOn = DateTime.UtcNow;
            Status = ExecutionStatus.Pending;
            UserId = Environment.UserName;
            MachineName = Environment.MachineName;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Agent")]
        public string? AgentId { get; set; }
        public Agent? Agent { get; set; }
        public DateTime? UtcStartedOn { get; set; }
        public DateTime? UtcStoppedOn { get; set; }
        public double? DurationInSeconds { get; set; }
        public ExecutionStatus Status { get; set; }
        public string? Result { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? UtcModifiedOn { get; set; }
        public string? UserId { get; set; }
        public string? MachineName { get; set; }
    }
}
