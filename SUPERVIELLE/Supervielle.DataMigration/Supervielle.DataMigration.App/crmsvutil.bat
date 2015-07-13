SET serverName=10.241.241.11
SET organizationName=SVDESAMIG
SET outputFilename=Entities
SET username=svcdynmcrmadmindesa
SET password=Yano.123
SET domainName=servicios
SET outputNamespace=Supervielle.DataMigration.Domain.Crm
SET crmsvcutil="C:\crm2015-sdk\SDK\Bin\CrmSvcUtil.exe"
%crmsvcutil% /url:http://%serverName%/%organizationName%/XRMServices/2011/Organization.svc /out:%outputFilename%.cs /username:%username% /password:%password% /domain:%domainName% /namespace:%outputNamespace%