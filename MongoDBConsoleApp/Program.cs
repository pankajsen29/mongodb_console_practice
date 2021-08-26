using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDBConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new MongoCRUD("AddressBook");

            //CREATE
            //before adding the AddressModel property
            //db.InsertDocument<PersonModel>("Users", new PersonModel { FirstName = "Anaika", LastName = "Sen" });

            //var person = new PersonModel
            //{
            //    FirstName = "Sarbari",
            //    LastName = "Sen",
            //    PrimaryAddress = new AddressModel
            //    {
            //        StreetAddress = "101 B G Road",
            //        State = "KA",
            //        City = "Blore",
            //        ZipCode = "560076"
            //    }
            //};
            //db.InsertDocument<PersonModel>("Users", person);

            //READ
            //var documents = db.ReadDocuments<PersonModel>("Users");
            //foreach (var doc in documents)
            //{
            //    Console.WriteLine($"{ doc.Id}: { doc.FirstName } {doc.LastName}");
            //    if (doc.PrimaryAddress != null)
            //    {
            //        Console.WriteLine(doc.PrimaryAddress.City);
            //    }
            //    Console.WriteLine();
            //}

            //READ - single document
            //take the GUID from the previous code by debugging
            //var docu = db.ReadDocumentById<PersonModel>("Users", new Guid("653c0fa3-ad24-4d58-b854-3cdac397ed42"));
            //Console.WriteLine($"{docu.FirstName} {docu.LastName}");

            //UPDATE
            //docu.DateOfBirth = new DateTime(2019,6,21,0,0,0,DateTimeKind.Utc );
            //db.UpsertDocument<PersonModel>("Users", docu.Id, docu);

            //DELETE
            //db.DeleteDocument<PersonModel>("Users", docu.Id);


            //READ - to a different model
            var documents = db.ReadDocuments<NameModel>("Users");
            foreach (var doc in documents)
            {
                Console.WriteLine($"{ doc.Id}: { doc.FirstName } {doc.LastName}");                
            }

            Console.ReadLine();
        }
    }

    public class MongoCRUD
    {
        IMongoDatabase db;
        public MongoCRUD(string database)
        {
            var client = new MongoClient(); //connects to localhost by default
            db = client.GetDatabase(database);
        }

        public void InsertDocument<T>(string collectionName, T document)
        {
            var collection = db.GetCollection<T>(collectionName);
            collection.InsertOne(document);
        }

        public List<T> ReadDocuments<T>(string collectionName)
        {
            var collection = db.GetCollection<T>(collectionName);
            return collection.Find(new BsonDocument()).ToList();
        }

        public T ReadDocumentById<T>(string collectionName, Guid id)
        {
            var collection = db.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("Id", id);
            return collection.Find(filter).First();
        }

        //Update if existing or Insert if not existing
        public void UpsertDocument<T>(string collectionName, Guid id, T document)
        {
            var collection = db.GetCollection<T>(collectionName);
            var filter = new BsonDocument("_id", new BsonBinaryData(id, GuidRepresentation.Standard));
            var result = collection.ReplaceOne(
                filter,
                document,
                new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteDocument<T>(string collectionName, Guid id)
        {
            var collection = db.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("Id",id);
            collection.DeleteOne(filter);
        }
    }

    [BsonIgnoreExtraElementsAttribute]
    public class NameModel
    {
        [BsonId]
        public Guid Id { get; set; } //_id
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class PersonModel
    {
        [BsonId]
        public Guid Id { get; set; } //_id
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public AddressModel PrimaryAddress { get; set; }

        [BsonElement("dob")]
        public DateTime DateOfBirth { get; set; }
    }

    public class AddressModel
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
