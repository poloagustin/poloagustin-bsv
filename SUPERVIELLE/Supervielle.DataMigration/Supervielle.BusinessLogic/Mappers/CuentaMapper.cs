using Microsoft.Xrm.Sdk;
using Supervielle.DataMigration.Domain.Crm;
using Supervielle.DataMigration.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.BusinessLogic.Mappers
{
    public static class CuentaMapper
    {
        public static bsv_cuentas GetCuenta(
            CajaDeAhorro sqlCuenta, 
            string nroCuenta,
            EntityReference modulo,
            EntityReference moneda,
            EntityReference sucursal,
            //EntityReference operacion,
            EntityReference tipoOperacion,
            EntityReference canalVenta,
            EntityReference estadoCuenta)
        {
            var cuenta = new bsv_cuentas();

            MapCuenta(
                sqlCuenta, 
                nroCuenta, 
                modulo,
                moneda,
                sucursal,
                tipoOperacion,
                canalVenta,
                estadoCuenta, 
                cuenta);

            return cuenta;
        }

        public static void MapCuenta(
            CajaDeAhorro sqlCuenta, 
            string nroCuenta, 
            EntityReference modulo,
            EntityReference moneda,
            EntityReference sucursal,
            //EntityReference operacion,
            EntityReference tipoOperacion,
            EntityReference canalVenta,
            EntityReference estadoCuenta, 
            bsv_cuentas crmCuenta)
        {
        crmCuenta.bsv_cuenta_empresa = sqlCuenta. EmpresaId ? 1: 0;
        crmCuenta.bsv_numero_de_cuenta = nroCuenta;
        crmCuenta.bsv_operacion_mdoulo = modulo;
        crmCuenta.bsv_operacion_moneda = moneda;
        crmCuenta.bsv_operacion_papel= sqlCuenta.PapelId;
        crmCuenta.bsv_operacion_sucursal = sucursal;
        crmCuenta.bsv_operacion_operacion = sqlCuenta.Operacion;
        crmCuenta.bsv_operacion_suboperacion = sqlCuenta. SubOperacion;
        crmCuenta.bsv_operacion_tipo_operacion = tipoOperacion;
        //crmCuenta.bsv_ = sqlCuenta. PaisId;
        //crmCuenta.bsv_ = sqlCuenta. TipoDocumentoId;
        //crmCuenta.bsv_ = sqlCuenta. NumeroDocumento;
        crmCuenta.bsv_canal_venta = canalVenta;
        //crmCuenta.bsv_ = sqlCuenta. OficialId;
        //crmCuenta.bsv_ = sqlCuenta. TipoCuenta;
        crmCuenta.bsv_estado_cuenta = estadoCuenta;
        //crmCuenta.bsv_embargos = sqlCuenta.Embargos;
        crmCuenta.bsv_cbu = sqlCuenta. Cbu;
        //crmCuenta.bsv_ = sqlCuenta. CodigoProducto;
        //crmCuenta.bsv_pa = sqlCuenta. PaqueteId;
        //crmCuenta.bsv_resumen_online = sqlCuenta. MarcaResumenOnline;
        //crmCuenta.bsv_tarjeta_de_debito_relacionada = sqlCuenta. TarjetaDebitoRelacionada;
        crmCuenta.bsv_plan_sueldo = sqlCuenta. PlanSueldo;
        //crmCuenta.bsv_bonificacion = sqlCuenta. Bonificacion;
        //crmCuenta.bsv_ = sqlCuenta. CodigoPromocion;
        crmCuenta.bsv_convenio = sqlCuenta. Convenio.ToString();
        //crmCuenta.bsv_tenencia_debitos_en_cuenta = sqlCuenta. TenenciaDebitoCuenta;
        crmCuenta.bsv_cantidad_debitos_en_cuenta = sqlCuenta. CantidadDebitoCuenta;
        //crmCuenta.bsv_mora = sqlCuenta. Mora;
        //crmCuenta.bsv_fecha_saldo_deudor = sqlCuenta. FechaSaldoDeudor;
        crmCuenta.bsv_tasa_interes_asociada = sqlCuenta. TasaInteresAsociada;
        //crmCuenta.bsv_comisionada = sqlCuenta. Comisionada;
        //crmCuenta.bsv_sobregiro = sqlCuenta. Sobregiro;
        }
    }
}
