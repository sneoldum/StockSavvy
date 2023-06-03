using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Entitiy.Dtos
{
    public class UserForLoginDto:IDto
    {
        public string Password { get; set; }
        public string Email { get; set; }

    }
}
