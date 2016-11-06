﻿using Ninject.Modules;
using TeamVesper.UI.Contracts;
using Ninject.Extensions.Factory;
using TeamVesper.DbCreate.Contracts;
using TeamVesper.DbCreate;
using TeamVesper.SqlServerData;
using TeamVesper.MongoDbData;
using TeamVesper.Models;
using TeamVesper.Repositories.Contracts;
using Ninject;
using Ninject.Parameters;
using TeamVesper.Repositories;
using TeamVesper.SQLiteData;
using TeamVesper.Importers;
using TeamVesper.XmlDataReader;
using TeamVesper.ExportToJson;
using TeamVesper.ExportToExcel;
using TeamVesper.ExportToPdf;
using TeamVesper.ExportToXML;

namespace TeamVesper.UI.Modules
{
    public class TeamVesperModule : NinjectModule
    {
        private const string CreateDbForm = "CreateDbForm";
        private const string ImportForm = "ImportForm";
        private const string TransferForm = "TransferForm";
        private const string ExportForm = "ExportForm";

        private const string MongoDbInitializer = "MongoDbInitializer";
        private const string SqlServerInitializer = "SqlServerInitializer";
        private const string SQLiteInitializer = "SQLiteInitializer";

        private const string MongoDeveloperReadableRepository = "MongoDeveloperReadableRepository";
        private const string SQLiteReadableRepository = "SQLiteReadableRepository";
        private const string XmlReadableRepository = "XmlReadableRepository";
        private const string ExcelReadableRepository = "ExcelReadableRepository";

        private const string SqlServerRepository = "SqlServerRepository";
        private const string MySqlRepository = "MySqlRepository";

        private const string JsonReporter = "JsonReporter";
        private const string XmlReporter = "XmlReporter";
        private const string PdfReporter = "PdfReporter";
        private const string ExcelReporter = "ExcelReporter";

        public override void Load()
        {
            Bind<MainForm>().To<MainForm>().InSingletonScope();

            Bind<IFormFactory>().ToFactory().InSingletonScope();
            Bind<CreateDBForm>().To<CreateDBForm>().Named(CreateDbForm);
            Bind<ImportForm>().To<ImportForm>().Named(ImportForm);
            Bind<TransferForm>().To<TransferForm>().Named(TransferForm);
            Bind<ExportForm>().To<ExportForm>().Named(ExportForm);

            Bind<IDbInitializerFactory>().ToFactory().InSingletonScope();
            Bind<IDbInitializer>().To<MongoDbInitializer>().Named(MongoDbInitializer);
            Bind<IDbInitializer>().To<SqlServerInitializer>().Named(SqlServerInitializer);
            Bind<IDbInitializer>().To<SQLiteInitializer>().Named(SQLiteInitializer);

            Bind<IRepositoryFactory>().ToFactory().InSingletonScope();

            Bind<IReadableRepository<Company>>().To<SQLiteImport>().InSingletonScope().Named(SQLiteReadableRepository);
            Bind<IReadableRepository<Bug>>().To<ExcelImporter>().InSingletonScope().Named(ExcelReadableRepository);
            Bind<IReadableRepository<DTOEducation>>().To<XmlImporter>().InSingletonScope().Named(XmlReadableRepository);
            Bind<IReadableRepository<MongoDeveloper>>().ToMethod(ctx =>
            {
                var connector = ctx.Kernel.Get<MongoConnector<MongoDeveloper>>();
                var param = new ConstructorArgument("dbSet", connector.Dbset);

                var repo = ctx.Kernel.Get<MongoRepository<MongoDeveloper>>(param);

                return repo;

            }).InSingletonScope().Named(MongoDeveloperReadableRepository);

            Bind(typeof(IRepository<>)).To(typeof(SqlServerRepository<>)).Named(SqlServerRepository);
            Bind<IRepository<DeveloperBugsInfo>>().To<MySqlRepository<DeveloperBugsInfo>>().Named(MySqlRepository);

            Bind<IReporterFactory>().ToFactory().InSingletonScope();

            Bind<IReporter<MongoDeveloper>>().To<JsonToFile>().Named(JsonReporter);
            Bind<IReporter<CompanyOverview>>().To<CompanyExcelExporter>().Named(ExcelReporter);
            Bind<IReporter<BugInfo>>().To<PdfExporter>().Named(PdfReporter);
            Bind<IReporter<BugReport>>().To<BugReportToXml>().Named(XmlReporter);

            // TODO Dependencies bindings

            Bind<SqlServerDbContext>().To<SqlServerDbContext>().InSingletonScope();
            Bind<MongoConnector<MongoDeveloper>>().To<MongoConnector<MongoDeveloper>>().InSingletonScope();
        }
    }
}