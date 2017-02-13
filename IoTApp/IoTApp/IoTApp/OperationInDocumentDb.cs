using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTApp
{
    public class OperationInDocumentDb
    {
        static OperationInDocumentDb defaultInstance = new OperationInDocumentDb();

        const string accountURL = @"https://seyondocumentdb.documents.azure.com:443/";
        const string accountKey = @"aGrxrFjKxAVHSiwFzCkEqBf8SI2t52QqZIwS4PkBHKWMp5Y2KaJopZBgsgTUpO6rB4GCajIcEpQknMqS04YSMQ==";
        const string databaseId = @"DeviceData";
        const string collectionId = @"Items";

        private Uri collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

        private DocumentClient client;
        private static Database db;
        private static DocumentCollection collection;

        public OperationInDocumentDb()
        {
            client = new DocumentClient(new System.Uri(accountURL), accountKey);
        }

        public static OperationInDocumentDb DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public List<DeviceDetails> Items { get; private set; }

        public async Task<List<DeviceDetails>> GetDeviceDetailsAsync()
        {
            try
            {
                db = await databaseValidator();

                collection = await CreateCollection();

                // The query excludes completed TodoItems
                var query = client.CreateDocumentQuery<DeviceDetails>(collectionLink, new FeedOptions { MaxItemCount = -1 })
                      .Where(deviceDetail => deviceDetail.Action == false)
                      .AsDocumentQuery();

                Items = new List<DeviceDetails>();
                while (query.HasMoreResults)
                {
                    Items.AddRange(await query.ExecuteNextAsync<DeviceDetails>());
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"ERROR {0}", e.Message);
                return null;
            }

            return Items;
        }

        private async Task<DocumentCollection> CreateCollection()
        {

            DocumentCollection collection = client.CreateDocumentCollectionQuery(db.SelfLink).Where(c => c.Id == collectionId).AsEnumerable().FirstOrDefault();

            if (collection == null)
            {
                DocumentCollection c1 = await client.CreateDocumentCollectionAsync(db.SelfLink, new DocumentCollection { Id = collectionId });

                Console.WriteLine("\n1.1. Created Collection \n{0}", c1);

            }

            return collection;
        }

        private async Task<Database> databaseValidator()
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();
            Console.WriteLine("1. Query for a database returned: {0}", database == null ? "no results" : database.Id);

            //check if a database was returned
            if (database == null)
            {
                //**************************
                // 2 -  Create a Database
                //**************************
                database = await client.CreateDatabaseAsync(new Database { Id = databaseId });
                Console.WriteLine("\n2. Created Database: id - {0} and selfLink - {1}", database.Id, database.SelfLink);
            }

            return database;
        }
    }
}
