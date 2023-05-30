using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Model
{
    public class Agent
    {
        public Agent()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? FullQualifiedName { get; set; }
        public string? InputParameters { get; set; }

    }
}
