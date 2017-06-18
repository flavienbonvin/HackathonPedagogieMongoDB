using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest.Collections
{
    class Answer
    {
        public ObjectId _id { get; set; }
        public ObjectId userID { get; set; }
        public ObjectId questionID { get; set; }
        public String[] answer { get; set; }
    }

}