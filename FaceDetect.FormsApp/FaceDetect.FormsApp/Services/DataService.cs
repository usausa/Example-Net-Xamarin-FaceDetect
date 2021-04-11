namespace FaceDetect.FormsApp.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using FaceDetect.FormsApp.Helpers;
    using FaceDetect.FormsApp.Models.Entity;

    using Microsoft.Data.Sqlite;

    using Smart.Data;
    using Smart.Data.Mapper;
    using Smart.Data.Mapper.Builders;

    public class DataServiceOptions
    {
        public string Path { get; set; }
    }

    public class DataService
    {
        private readonly DataServiceOptions options;

        private readonly DelegateDbProvider provider;

        public DataService(DataServiceOptions options)
        {
            this.options = options;

            var connectionString = $"Data Source={options.Path}";
            provider = new DelegateDbProvider(() => new SqliteConnection(connectionString));
        }

        public async ValueTask PrepareAsync()
        {
            if (File.Exists(options.Path))
            {
                return;
            }

            await provider.UsingAsync(async con =>
            {
                await con.ExecuteAsync("PRAGMA AUTO_VACUUM=1");
                await con.ExecuteAsync(SqlHelper.MakeCreate<PersonEntity>());
            });
        }

        public ValueTask<List<PersonEntity>> QueryPersonListAsync() =>
            provider.UsingAsync(con => con.QueryListAsync<PersonEntity>(SqlSelect<PersonEntity>.Build(order: "Name")));

        public ValueTask<int> InsertPerson(PersonEntity entity) =>
            provider.UsingAsync(con => con.ExecuteAsync(SqlInsert<PersonEntity>.Values(), entity));

        public ValueTask<int> DeletePerson(PersonEntity entity) =>
            provider.UsingAsync(con => con.ExecuteAsync(SqlDelete<PersonEntity>.ByKey(), entity));
    }
}
