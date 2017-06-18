using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest.Collections
{
    public class Question
    {
        public ObjectId _id { get; set; }
        public string question { get; set; }
        public string[] answer { get; set; }
        public ObjectId questionTypeID { get; set; }
    }

}
