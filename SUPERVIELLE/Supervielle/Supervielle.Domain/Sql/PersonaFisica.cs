using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.Domain.Sql
{
    public class PersonaFisica
    {
        public int PaisId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string NumeroDocumento { get; set; }
        public string CuitCuil { get; set; }
        public string Nombre { get; set; }
        public char Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string CorreoElectronico { get; set; }
        public int SucursalId { get; set; }
        public int EjecutivoCuentaId { get; set; }
        public int CanalVentaId { get; set; }
        public bool SegmentoId { get; set; }
        public DateTime FechaAlta { get; set; }
        public string TipoDomicilioId { get; set; }
        public int DomicilioPaisId { get; set; }
        public string DomicilioCalle { get; set; }
        public int DomicilioNumero { get; set; }
        public string DomicilioDepartamento { get; set; }
        public string DomicilioCodigoPostal { get; set; }
        public int DomicilioLocalidadId { get; set; }
        public int DomicilioProvinciaId { get; set; }
        public bool TipoTelefonoId { get; set; }
        public string NumeroTelefonoPrincipal { get; set; }
        public string NumeroTelefonoLaboral { get; set; }
        public int EstadoCivilId { get; set; }
        public int BancaId { get; set; }
        public int ResponsableOficialId { get; set; }
        public int TipoClienteId { get; set; }
        public string ActividadId { get; set; }
        public int IvaId { get; set; }
        public int ProfesionId { get; set; }
        public int OcupacionId { get; set; }
        public bool Activo { get; set; }
        public bool Titular { get; set; }
        public int PaisEmpresaId { get; set; }
        public int TipoDocumentoEmpresaId { get; set; }
        public string NumeroDocumentoEmpresa { get; set; }
    }
}
