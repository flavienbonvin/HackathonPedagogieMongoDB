using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest.Collections
{
    class User
    {
            public ObjectId Id { get; set; }
            public string name { get; set; }
            public string lastname { get; set; }
            public string email { get; set; }
    }
}
