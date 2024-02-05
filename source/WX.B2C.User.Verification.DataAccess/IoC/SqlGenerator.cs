using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using WX.B2C.User.Verification.DataAccess.EF.DbConfig;

namespace WX.B2C.User.Verification.DataAccess.IoC
{
    internal class SqlGenerator : SqlServerMigrationsSqlGenerator
    {
        private readonly bool _isLocal;

        public SqlGenerator(MigrationsSqlGeneratorDependencies dependencies, IMigrationsAnnotationProvider migrationsAnnotations) : base(
        dependencies,
        migrationsAnnotations)
        {
            _isLocal = MigrationConfig.IsLocal();
        }

        /// <summary>
        ///     Builds commands for the given <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Operations.CreateIndexOperation" /> by making calls on the given
        ///     <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected override void Generate(CreateIndexOperation operation,
                                         IModel model,
                                         MigrationCommandListBuilder builder,
                                         bool terminate = true)
        {
            RemoveOnlineAnnotationForLocalBuild(operation);
            base.Generate(operation, model, builder, terminate);
        }

        private void RemoveOnlineAnnotationForLocalBuild(CreateIndexOperation operation)
        {
            if (!_isLocal)
                return;

            var checkConstraint = operation.FindAnnotation("SqlServer:Online");
            if (checkConstraint == null)
                return;

            Console.WriteLine($"Removing annotation {checkConstraint.Name}");
            operation.RemoveAnnotation("SqlServer:Online");
        }
    }
}