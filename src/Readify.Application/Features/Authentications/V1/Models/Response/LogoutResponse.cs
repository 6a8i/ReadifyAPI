using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readify.Application.Features.Authentications.V1.Models.Response
{
    public class LogoutResponse
    {
        public Guid Token { get; set; }
        public bool TokenHasExpired { get; set; }
    }
}
