using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Models
{
    public static class SessionExtensions
    {
        public static byte[] Get(this ISession session, string key)
        {
            byte[] value;
            session.TryGetValue(key, out value);
            return value;
        }
    }
}
