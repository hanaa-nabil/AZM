using AZM.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.User
{
    public class UpdateProfileRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? Username { get; set; }
        public string? Location { get; set; }
        public List<Sport>? SportsToAdd { get; set; }
        public List<Sport>? SportsToRemove { get; set; }
        public IFormFile? Photo { get; set; }          
        public bool RemovePhoto { get; set; } = false;
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
    }
}
