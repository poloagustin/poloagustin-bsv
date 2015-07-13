using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.Domain.Sql
{
    public class PersonaJuridica
    {
        public int PaisId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string NumeroDocumento { get; set; }
        public string CuitCuil { get; set; }
        public string RazonSocial { get; set; }
        public string CorreoElectronico { get; set; }
        public string Contacto { get; set; }
        public int SucursalId { get; set; }
        public int EjecutivoCuenta { get; set; }
        public int CanalVentaId { get; set; }
        public bool SegmentoId { get; set; }
        public DateTime FechaAlta { get; set; }
        public string TipoDomicilioId { get; set; }
        public int DomicilioPaisId { get; set; }
        public string DomicilioCalle { get; set; }
        public int DomicilioNumero { get; set; }
        public string DomicilioDepartamento { get; set; }
        public string DomicilioCodigoPostal { get; set; }
        public int LocalidadId { get; set; }
        public int DomicilioProvinciaId { get; set; }
        public bool TipoTelefonoId { get; set; }
        public int NumeroTelefonoPrincipal { get; set; }
        public int BancaId { get; set; }
        public int OficialResponsableId { get; set; }
        public int TipoClienteId { get; set; }
        public string ActividadId { get; set; }
        public int IvaId { get; set; }
        public bool Activo { get; set; }
        public bool Titular { get; set; }
    }
}
