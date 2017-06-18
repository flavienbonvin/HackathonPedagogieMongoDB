using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest.Collections
{
    class QuestionType
    {
            public ObjectId _id { get; set; }
            public string name { get; set; }
    }
}
