using Supervielle.BusinessLogic.Intefaces;
using Supervielle.DataAccess.Sql;
using ISql = Supervielle.DataAccess.Sql.Interfaces;
using ICrm = Supervielle.DataAccess.Crm.Interfaces;
using Sql = Supervielle.DataAccess.Sql;
using Crm = Supervielle.DataAccess.Crm;
using Supervielle.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supervielle.Domain.Sql;
using Microsoft.Xrm.Sdk.Query;
using Supervielle.Domain.Crm;
using Supervielle.BusinessLogic.Comparers;
using Supervielle.BusinessLogic.Mappers;
using Microsoft.Xrm.Sdk;

namespace Supervielle.BusinessLogic
{
    public class ProductMigrationService : BaseService, IProductMigrationService
    {
        private ISql.ICajaDeAhorroDao sqlCajaDeAhorroDao;
        private ISql.IRelacionCuentaPersonaDao sqlRelacionCuentaPersonaDao;
        private ICrm.ICuentaDao crmCuentaDao;
        private ICrm.IMonedaDao crmMonedaDao;
        private ICrm.IModuloDao crmModuloDao;
        private ICrm.IRelacionCuentaPersonaFisicaDao crmRelacionCuentaPersonaFisicaDao;
        private ICrm.ITipoOperacionDao crmTipoOperacionDao;
        private ICrm.ISucursalDao crmSucursalDao;
        private ICrm.ICanalDao crmCanalVentaDao;
        private ICrm.IEstadoCuentaDao crmEstadoCuentaDao;

        public ProductMigrationService()
        {
            this.sqlCajaDeAhorroDao = new Sql.CajaDeAhorroDao();
            this.sqlRelacionCuentaPersonaDao = new Sql.RelacionCuentaPersonaDao();
            this.crmCuentaDao = new Crm.CuentaDao();
            this.crmMonedaDao = new Crm.MonedaDao();
            this.crmModuloDao = new Crm.ModuloDao();
            this.crmTipoOperacionDao = new Crm.TipoOperacionDao();
            this.crmRelacionCuentaPersonaFisicaDao = new Crm.RelacionCuentaPersonaFisicaDao();
            this.crmSucursalDao = new Crm.SucursalDao();
            this.crmCanalVentaDao = new Crm.CanalDao();
            this.crmEstadoCuentaDao = new Crm.EstadoCuentaDao();
        }

        public void Migrate(List<AvailableValue> values)
        {
            IEnumerable<EntityReference> cuentas = new List<EntityReference>();

            if (values.Any(x => x.Name.ToUpper() == "Cuentas"))
            {
                cuentas = MigrateCuentas();
            }

            var relacionesCuentaPersonaFisica = MigrateRelacionesCuentaPersonaFisica();

#if DEBUG
            this.crmCuentaDao.DeleteMany(cuentas);
#endif
        }

        private IEnumerable<EntityReference> MigrateRelacionesCuentaPersonaFisica()
        {
            var sqlRelacionesCuentaPersonaFisica = this.sqlRelacionCuentaPersonaDao.RetrieveRecords();
            var crmRelacionesCuentaPersonaFisica = this.crmRelacionCuentaPersonaFisicaDao.GetFiltered(GetFiltersRelacionesCuentaPersonaFisica(sqlRelacionesCuentaPersonaFisica)).ToList();
            Dictionary<RelacionCuentaPersona, bsv_relacion_fisica_cuentas> crmRelacionesCuentaPersonaFisicaToCreate = new Dictionary<RelacionCuentaPersona, bsv_relacion_fisica_cuentas>();
            Dictionary<RelacionCuentaPersona, bsv_relacion_fisica_cuentas> crmRelacionesCuentaPersonaFisicaToUpdate = new Dictionary<RelacionCuentaPersona, bsv_relacion_fisica_cuentas>();
            AssignNewValues(out crmRelacionesCuentaPersonaFisicaToUpdate, out crmRelacionesCuentaPersonaFisicaToCreate, sqlRelacionesCuentaPersonaFisica, crmRelacionesCuentaPersonaFisica);

            this.crmRelacionCuentaPersonaFisicaDao.UpdateMany(crmRelacionesCuentaPersonaFisicaToUpdate.Values);
            var crmRelacionesCuentaPersonaFisicaCreated = this.crmRelacionCuentaPersonaFisicaDao.SaveMany(crmRelacionesCuentaPersonaFisicaToCreate.Values).ToList();
            return crmRelacionesCuentaPersonaFisicaToUpdate.Values.Concat(crmRelacionesCuentaPersonaFisicaCreated).Select(x => this.crmCuentaDao.GetEntityReference(x));
        }

        private IEnumerable<FilterExpression> GetFiltersRelacionesCuentaPersonaFisica(IEnumerable<RelacionCuentaPersona> sqlRelacionesCuentaPersonaFisica)
        {
            var filters = GetFilters<RelacionCuentaPersona>(
                sqlRelacionesCuentaPersonaFisica,
                sqlRelacionCuentaPersonaFisica =>
                {
                    var filter = new FilterExpression(LogicalOperator.And);
                    var nroCuenta = BuildNumeroCuenta(sqlRelacionCuentaPersonaFisica);
                    filter.AddCondition(new ConditionExpression("bsv_numero_de_cuenta", ConditionOperator.Equal, nroCuenta));
                    var moneda = this.crmMonedaDao.GetObjectByCode(sqlCajaDeAhorro.MonedaId.ToString());
                    filter.AddCondition(new ConditionExpression("bsv_moneda", ConditionOperator.Equal, moneda.Id));
                    var modulo = this.crmModuloDao.GetObjectByCode(sqlCajaDeAhorro.ModuloId.ToString());
                    filter.AddCondition(new ConditionExpression("bsv_mdoulo", ConditionOperator.Equal, modulo));
                    var tipoOperacion = this.crmTipoOperacionDao.GetObjectByCode(sqlCajaDeAhorro.TipoOperacionId.ToString());
                    filter.AddCondition(new ConditionExpression("bsv_tipo_operacion", ConditionOperator.Equal, tipoOperacion));

                    return filter;
                });

            return filters;
        }

        private IEnumerable<EntityReference> MigrateCuentas()
        {
            var sqlCajasDeAhorro = this.sqlCajaDeAhorroDao.RetrieveRecords();
            var crmCuentas = this.crmCuentaDao.GetFiltered(GetFiltersCuentas(sqlCajasDeAhorro)).ToList();
            Dictionary<CajaDeAhorro, bsv_cuentas> crmCuentasToCreate = new Dictionary<CajaDeAhorro, bsv_cuentas>();
            Dictionary<CajaDeAhorro, bsv_cuentas> crmCuentasToUpdate = new Dictionary<CajaDeAhorro, bsv_cuentas>();
            AssignNewValues(out crmCuentasToUpdate, out crmCuentasToCreate, sqlCajasDeAhorro, crmCuentas);

            this.crmCuentaDao.UpdateMany(crmCuentasToUpdate.Values);
            var crmCuentasCreated = this.crmCuentaDao.SaveMany(crmCuentasToCreate.Values).ToList();
            return crmCuentasToUpdate.Values.Concat(crmCuentasCreated).Select(x => this.crmCuentaDao.GetEntityReference(x));
        }

        private void AssignNewValues(out Dictionary<CajaDeAhorro, bsv_cuentas> crmCuentasToUpdate, out Dictionary<CajaDeAhorro, bsv_cuentas> crmCuentasToCreate, IEnumerable<CajaDeAhorro> sqlCuentas, List<bsv_cuentas> crmCuentas)
        {
            crmCuentasToCreate = new Dictionary<CajaDeAhorro, bsv_cuentas>();
            crmCuentasToUpdate = new Dictionary<CajaDeAhorro, bsv_cuentas>();

            foreach (var sqlCuenta in sqlCuentas)
            {
                var nroCuenta = BuildNumeroCuenta(sqlCuenta);
                var moneda = this.crmMonedaDao.GetEntityReferenceByCode(sqlCuenta.MonedaId);
                var modulo = this.crmModuloDao.GetEntityReferenceByCode(sqlCuenta.ModuloId);
                var tipoOperacion = this.crmTipoOperacionDao.GetEntityReferenceByCode(sqlCuenta.TipoOperacionId);
                var sucursal = this.crmSucursalDao.GetEntityReferenceByCode(sqlCuenta.SucursalId);
                var canalVenta = this.crmCanalVentaDao.GetEntityReferenceByCode(sqlCuenta.CanalVentaId);
                var estadoCuenta = this.crmEstadoCuentaDao.GetEntityReferenceByCode(sqlCuenta.EstadoId);

                var crmCuenta = crmCuentas.FirstOrDefault(x => CuentaComparer.CompareCrm(x, nroCuenta, modulo.Id, moneda.Id, tipoOperacion.Id));

                if (crmCuenta == null)
                {
                    var crmPersonaFisicaToCreate = CuentaMapper.GetCuenta(
                        sqlCuenta,
                        nroCuenta,
                        modulo,
                        moneda,
                        sucursal,
                        tipoOperacion,
                        canalVenta,
                        estadoCuenta);

                    crmCuentasToCreate.Add(sqlCuenta, crmPersonaFisicaToCreate);
                }
                else
                {
                    CuentaMapper.MapCuenta(
                        sqlCuenta,
                        nroCuenta,
                        modulo,
                        moneda,
                        sucursal,
                        tipoOperacion,
                        canalVenta,
                        estadoCuenta,
                        crmCuenta);

                    crmCuentasToUpdate.Add(sqlCuenta, crmCuenta);
                }
            }
        }

        private void AssignNewValues(
            out Dictionary<RelacionCuentaPersona, bsv_relacion_fisica_cuentas> crmRelacionesCuentaPersonaToUpdate,
            out Dictionary<RelacionCuentaPersona, bsv_relacion_fisica_cuentas> crmRelacionesCuentaPersonaToCreate,
            IEnumerable<RelacionCuentaPersona> sqlRelacionesCuentaPersona,
            List<bsv_relacion_fisica_cuentas> crmRelacionesCuentaPersona)
        {
            crmRelacionesCuentaPersonaToCreate = new Dictionary<RelacionCuentaPersona, bsv_relacion_fisica_cuentas>();
            crmRelacionesCuentaPersonaToUpdate = new Dictionary<RelacionCuentaPersona, bsv_relacion_fisica_cuentas>();

            foreach (var sqlRelacionCuentaPersona in sqlRelacionesCuentaPersona)
            {
                var crmRelacionCuentaPersona = crmRelacionesCuentaPersona.FirstOrDefault(x => RelacionCuentaPersonaComparer.CompareCrm(x));

                if (crmRelacionCuentaPersona == null)
                {
                    var crmRelacionCuentaPersonaToCreate = RelacionCuentaPersonaMapper.GetRelacionCuentaPersona(
                        sqlRelacionCuentaPersona);

                    crmRelacionesCuentaPersonaToCreate.Add(sqlRelacionCuentaPersona, crmRelacionCuentaPersonaToCreate);
                }
                else
                {
                    RelacionCuentaPersonaMapper.MapRelacionCuentaPersona(
                        sqlRelacionCuentaPersona);

                    crmRelacionesCuentaPersonaToUpdate.Add(sqlRelacionCuentaPersona, crmRelacionCuentaPersona);
                }
            }
        }

        private IEnumerable<FilterExpression> GetFiltersCuentas(IEnumerable<CajaDeAhorro> sqlCajasDeAhorro)
        {
            var filters = GetFilters<CajaDeAhorro>(
                sqlCajasDeAhorro,
                sqlCajaDeAhorro =>
                {
                    var filter = new FilterExpression(LogicalOperator.And);
                    var nroCuenta = BuildNumeroCuenta(sqlCajaDeAhorro);
                    filter.AddCondition(new ConditionExpression("bsv_numero_de_cuenta", ConditionOperator.Equal, nroCuenta));
                    var moneda = this.crmMonedaDao.GetObjectByCode(sqlCajaDeAhorro.MonedaId.ToString());
                    filter.AddCondition(new ConditionExpression("bsv_moneda", ConditionOperator.Equal, moneda.Id));
                    var modulo = this.crmModuloDao.GetObjectByCode(sqlCajaDeAhorro.ModuloId.ToString());
                    filter.AddCondition(new ConditionExpression("bsv_mdoulo", ConditionOperator.Equal, modulo));
                    var tipoOperacion = this.crmTipoOperacionDao.GetObjectByCode(sqlCajaDeAhorro.TipoOperacionId.ToString());
                    filter.AddCondition(new ConditionExpression("bsv_tipo_operacion", ConditionOperator.Equal, tipoOperacion));

                    return filter;
                });

            return filters;
        }

        private string BuildNumeroCuenta(CajaDeAhorro sqlCajaDeAhorro)
        {
            var builder = new StringBuilder();
            builder.Append(sqlCajaDeAhorro.SucursalId);
            builder.Append('-');
            builder.Append(sqlCajaDeAhorro.CuentaNumero);
            builder.Append('/');
            builder.Append(sqlCajaDeAhorro.SubOperacion);
            return builder.ToString();
        }
    }
}
