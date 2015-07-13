using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.Domain.Sql
{
    public class RelacionCuentaPersona
    {
        public bool EmpresaId { get; set; }
        public int CuentaNumero { get; set; }
        public int PaisId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string NumeroDocumento { get; set; }
        public int TipoTitularidadId { get; set; }
        public string TipoTitularidad { get; set; }
        public bool PersonaRepresentativa { get; set; }
        public bool Empleado { get; set; }
    }
}
