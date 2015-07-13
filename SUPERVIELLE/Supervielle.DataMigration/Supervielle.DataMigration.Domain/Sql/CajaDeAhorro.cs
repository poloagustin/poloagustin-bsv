using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.Domain.Sql
{
    public class CajaDeAhorro
    {
        public bool EmpresaId { get; set; }
        public int CuentaNumero { get; set; }
        public int ModuloId { get; set; }
        public int MonedaId { get; set; }
        public int PapelId { get; set; }
        public int SucursalId { get; set; }
        public int Operacion { get; set; }
        public int SubOperacion { get; set; }
        public int TipoOperacionId { get; set; }
        public int PaisId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string NumeroDocumento { get; set; }
        public int CanalVentaId { get; set; }
        public int OficialId { get; set; }
        public string TipoCuenta { get; set; }
        public int EstadoId { get; set; }
        public string Embargos { get; set; }
        public string Cbu { get; set; }
        public int CodigoProducto { get; set; }
        public int PaqueteId { get; set; }
        public string MarcaResumenOnline { get; set; }
        public string TarjetaDebitoRelacionada { get; set; }
        public bool PlanSueldo { get; set; }
        public string Bonificacion { get; set; }
        public int CodigoPromocion { get; set; }
        public int Convenio { get; set; }
        public string TenenciaDebitoCuenta { get; set; }
        public int CantidadDebitoCuenta { get; set; }
        public string Mora { get; set; }
        public DateTime FechaSaldoDeudor { get; set; }
        public decimal TasaInteresAsociada { get; set; }
        public int Comisionada { get; set; }
        public int Sobregiro { get; set; }
    }
}
