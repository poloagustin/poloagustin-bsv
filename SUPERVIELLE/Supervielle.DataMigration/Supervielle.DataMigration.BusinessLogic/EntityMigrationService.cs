using Microsoft.Xrm.Sdk.Query;
using Supervielle.DataMigration.BusinessLogic.Intefaces;
using Supervielle.DataMigration.DataAccess.Crm;
using Crm = Supervielle.DataMigration.DataAccess.Crm.Interfaces;
using Supervielle.DataMigration.DataAccess.Sql;
using Sql = Supervielle.DataMigration.DataAccess.Sql.Interfaces;
using Supervielle.DataMigration.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supervielle.DataMigration.Domain.Crm;
using Supervielle.DataMigration.BusinessLogic.Mappers;
using Microsoft.Xrm.Sdk;

namespace Supervielle.DataMigration.BusinessLogic
{
    public class EntityMigrationService : BaseService, IEntityMigrationService
    {
        private Sql.IPersonaFisicaDao personaFisicaDao;
        private Sql.IPersonaJuridicaDao personaJuridicaDao;
        private Crm.IAccountDao accountDao;
        private Crm.IContactDao contactDao;
        private Crm.ICanalDao crmCanalDao;
        private Crm.IPaisDao crmPaisDao;
        private Crm.IActividadLaboralDao crmActividadLaboralDao;
        private Crm.IBancaDao crmBancaDao;
        private Crm.ILocalidadDao crmLocalidadDao;
        private Crm.ISucursalDao crmSucursalDao;
        private Crm.ISegmentoDao crmSegmentoDao;
        private Crm.IProvinciaDao crmProvinciaDao;
        private Crm.IProfesionDao crmProfesionDao;

        public EntityMigrationService()
        {
            this.personaFisicaDao = new PersonaFisicaDao();
            this.personaJuridicaDao = new PersonaJuridicaDao();
            this.accountDao = new AccountDao();
            this.contactDao = new ContactDao();
            this.crmCanalDao = new CanalDao();
            this.crmPaisDao = new PaisDao();
            this.crmActividadLaboralDao = new ActividadLaboralDao();
            this.crmBancaDao = new BancaDao();
            this.crmLocalidadDao = new LocalidadDao();
            this.crmSucursalDao = new SucursalDao();
            this.crmSegmentoDao = new SegmentoDao();
            this.crmProvinciaDao = new ProvinciaDao();
            this.crmProfesionDao = new ProfesionDao();
        }

        public void Migrate()
        {
            var personasFisicasReferences = MigratePersonasFisicas();
            var personasJuridicasReferences = MigratePersonasJuridicas();

#if DEBUG
            this.contactDao.DeleteMany(personasFisicasReferences);
            this.contactDao.DeleteMany(personasJuridicasReferences);
#endif
        }

        private IEnumerable<EntityReference> MigratePersonasJuridicas()
        {
            var sqlPersonasJuridicas = this.personaJuridicaDao.RetrieveRecords();
            var crmPersonasJuridicas = this.accountDao.GetFiltered(GetFiltersPersonasJuridicas(sqlPersonasJuridicas)).ToList();
            var crmPersonasJuridicasToCreate = new Dictionary<PersonaJuridica, Account>();
            var crmPersonasJuridicasToUpdate = new Dictionary<PersonaJuridica, Account>();
            AssignNewValues(out crmPersonasJuridicasToUpdate, out crmPersonasJuridicasToCreate, sqlPersonasJuridicas, crmPersonasJuridicas);

            this.accountDao.UpdateMany(crmPersonasJuridicasToUpdate.Values);
            var crmPersonasFisicasCreated = this.accountDao.SaveMany(crmPersonasJuridicasToCreate.Values).ToList();
            return crmPersonasJuridicasToUpdate.Values.Concat(crmPersonasFisicasCreated).Select(x => this.contactDao.GetEntityReference(x));
        }

        private IEnumerable<EntityReference> MigratePersonasFisicas()
        {
            var sqlPersonasFisicas = this.personaFisicaDao.RetrieveRecords();
            var crmPersonasFisicas = this.contactDao.GetFiltered(GetFiltersPersonasFisicas(sqlPersonasFisicas)).ToList();
            Dictionary<PersonaFisica, Contact> crmPersonasFisicasToCreate = new Dictionary<PersonaFisica, Contact>();
            Dictionary<PersonaFisica, Contact> crmPersonasFisicasToUpdate = new Dictionary<PersonaFisica, Contact>();
            AssignNewValues(out crmPersonasFisicasToUpdate, out crmPersonasFisicasToCreate, sqlPersonasFisicas, crmPersonasFisicas);

            this.contactDao.UpdateMany(crmPersonasFisicasToUpdate.Values);
            var crmPersonasFisicasCreated = this.contactDao.SaveMany(crmPersonasFisicasToCreate.Values).ToList();
            return crmPersonasFisicasToUpdate.Values.Concat(crmPersonasFisicasCreated).Select(x => this.contactDao.GetEntityReference(x));
        }

        private void AssignNewValues(out Dictionary<PersonaFisica, Contact> crmPersonasFisicasToUpdate, out Dictionary<PersonaFisica, Contact> crmPersonasFisicasToCreate, IEnumerable<PersonaFisica> sqlPersonasFisicas, IEnumerable<Contact> crmPersonasFisicas)
        {
            crmPersonasFisicasToCreate = new Dictionary<PersonaFisica, Contact>();
            crmPersonasFisicasToUpdate = new Dictionary<PersonaFisica, Contact>();

            foreach (var sqlPersonaFisica in sqlPersonasFisicas)
            {
                var crmPersonaFisica = crmPersonasFisicas.FirstOrDefault(x =>
                    x.bsv_Pais.Id == this.crmPaisDao.GetObjectByCode(sqlPersonaFisica.PaisId).Id &&
                    x.bsv_tipo_de_documento.Value == sqlPersonaFisica.TipoDocumentoId &&
                    x.bsv_no_de_documento == sqlPersonaFisica.NumeroDocumento);

                var domicilioPais = this.crmPaisDao.GetEntityReferenceByCode(sqlPersonaFisica.DomicilioPaisId.ToString());
                var actividadLaboral = this.crmActividadLaboralDao.GetEntityReferenceByCode(sqlPersonaFisica.ActividadId.ToString());
                var banca = this.crmBancaDao.GetEntityReferenceByCode(sqlPersonaFisica.BancaId.ToString());
                var pais = this.crmPaisDao.GetEntityReferenceByCode(sqlPersonaFisica.PaisEmpresaId.ToString());
                var localidad = this.crmLocalidadDao.GetEntityReferenceByCode(sqlPersonaFisica.DomicilioLocalidadId.ToString());
                var sucursal = this.crmSucursalDao.GetEntityReferenceByCode(sqlPersonaFisica.SucursalId.ToString());
                var segmento = this.crmSegmentoDao.GetEntityReferenceByCode(sqlPersonaFisica.SegmentoId.ToString());
                var provincia = this.crmProvinciaDao.GetEntityReferenceByCode(sqlPersonaFisica.DomicilioProvinciaId.ToString());
                var profesion = this.crmProfesionDao.GetEntityReferenceByCode(sqlPersonaFisica.ProfesionId.ToString());

                if (crmPersonaFisica == null)
                {
                    var crmPersonaFisicaToCreate = PersonaFisicaMapper.GetContact(
                        sqlPersonaFisica,
                        domicilioPais,
                        actividadLaboral,
                        banca,
                        pais,
                        localidad,
                        sucursal,
                        segmento,
                        provincia,
                        profesion);

                    crmPersonasFisicasToCreate.Add(sqlPersonaFisica, crmPersonaFisicaToCreate);
                }
                else
                {
                    PersonaFisicaMapper.MapContact(
                        sqlPersonaFisica,
                        domicilioPais,
                        actividadLaboral,
                        banca,
                        pais,
                        localidad,
                        sucursal,
                        segmento,
                        provincia,
                        profesion,
                        crmPersonaFisica);

                    crmPersonasFisicasToUpdate.Add(sqlPersonaFisica, crmPersonaFisica);
                }
            }
        }

        private void AssignNewValues(out Dictionary<PersonaJuridica, Account> crmPersonasJuridicasToUpdate, out Dictionary<PersonaJuridica, Account> crmPersonasJuridicasToCreate, IEnumerable<PersonaJuridica> sqlPersonasJuridicas, List<Account> crmPersonasJuridicas)
        {
            crmPersonasJuridicasToCreate = new Dictionary<PersonaJuridica, Account>();
            crmPersonasJuridicasToUpdate = new Dictionary<PersonaJuridica, Account>();

            foreach (var sqlPersonaJuridica in sqlPersonasJuridicas)
            {
                var crmPersonaJuridica = crmPersonasJuridicas.FirstOrDefault(x => EqualsPersonaJuridica(x, sqlPersonaJuridica.RazonSocial));

                var domicilioPais = this.crmPaisDao.GetEntityReferenceByCode(sqlPersonaJuridica.DomicilioPaisId);
                var actividad = this.crmActividadLaboralDao.GetEntityReferenceByCode(sqlPersonaJuridica.ActividadId);
                var banca = this.crmBancaDao.GetEntityReferenceByCode(sqlPersonaJuridica.BancaId);
                var pais = this.crmPaisDao.GetEntityReferenceByCode(sqlPersonaJuridica.PaisId);
                var localidad = this.crmLocalidadDao.GetEntityReferenceByCode(sqlPersonaJuridica.LocalidadId);
                var sucursal = this.crmSucursalDao.GetEntityReferenceByCode(sqlPersonaJuridica.SucursalId);
                var segmento = this.crmSegmentoDao.GetEntityReferenceByCode(sqlPersonaJuridica.SegmentoId);
                var provincia = this.crmProvinciaDao.GetEntityReferenceByCode(sqlPersonaJuridica.DomicilioProvinciaId);
                var canalVenta = this.crmCanalDao.GetEntityReferenceByCode(sqlPersonaJuridica.CanalVentaId);

                if (crmPersonaJuridica == null)
                {
                    var crmPersonaFisicaToCreate = PersonaJuridicaMapper.GetAccount(
                        sqlPersonaJuridica,
                        pais,
                        sucursal,
                        canalVenta,
                        segmento,
                        domicilioPais,
                        localidad,
                        provincia,
                        banca,
                        actividad);

                    crmPersonasJuridicasToCreate.Add(sqlPersonaJuridica, crmPersonaFisicaToCreate);
                }
                else
                {
                    PersonaJuridicaMapper.MapAccount(
                        sqlPersonaJuridica,
                        crmPersonaJuridica,
                        pais,
                        sucursal,
                        canalVenta,
                        segmento,
                        domicilioPais,
                        localidad,
                        provincia,
                        banca,
                        actividad);

                    crmPersonasJuridicasToUpdate.Add(sqlPersonaJuridica, crmPersonaJuridica);
                }
            }
        }

        private IEnumerable<FilterExpression> GetFiltersPersonasFisicas(IEnumerable<PersonaFisica> sqlPersonasFisicas)
        {
            var filters = GetFilters<PersonaFisica>(
                sqlPersonasFisicas,
                sqlPersonaFisica =>
                {
                    var filter = new FilterExpression(LogicalOperator.And);

                    var pais = this.crmPaisDao.GetObjectByCode(sqlPersonaFisica.PaisId.ToString());
                    filter.AddCondition(new ConditionExpression("bsv_pais", ConditionOperator.Equal, pais.Id));
                    var tipoDocumentoId = sqlPersonaFisica.TipoDocumentoId;
                    filter.AddCondition(new ConditionExpression("bsv_tipo_de_documento", ConditionOperator.Equal, tipoDocumentoId));
                    var numeroDocumento = sqlPersonaFisica.NumeroDocumento;
                    filter.AddCondition(new ConditionExpression("bsv_no_de_documento", ConditionOperator.Equal, numeroDocumento));

                    return filter;
                });

            return filters;
        }

        private IEnumerable<FilterExpression> GetFiltersPersonasJuridicas(IEnumerable<PersonaJuridica> sqlPersonasJuridicas)
        {
            var filters = GetFilters<PersonaJuridica>(
                sqlPersonasJuridicas,
                x =>
                {
                    var filter = new FilterExpression(LogicalOperator.And);

                    filter.AddCondition(new ConditionExpression("name", ConditionOperator.Equal, x.RazonSocial));

                    return filter;
                });
            
            return filters;
        }

        private static bool EqualsPersonaJuridica(Account account, string razonSocial)
        {
            return account.Name == razonSocial;
        }
    }
}
