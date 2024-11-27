using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM2E2GRUPO2.Models
{
    public class Sitio
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public Byte[] Video { get; set; } 
        public Byte[] Audio { get; set; }
    }
}
