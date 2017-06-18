using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest
{
    class Program
    {
        private static MongoClient clientMongo;
        private static IMongoDatabase databaseMongo;
        private static IMongoCollection<Collections.User> collUser;
        private static IMongoCollection<Collections.QuestionType> collQuestionType;
        private static IMongoCollection<Collections.Question> collQuestion;
        private static IMongoCollection<Collections.Answer> collAnswer;

        static void Main(string[] args)
        {
            //Connect to local mongoDB and create all the collections needed
            ConnectToLocalMongoDB();

            //Main program asking looping on actions available for the user
            MainProgram();

            Console.WriteLine("End of the program");
            Console.ReadKey();
        }

        private static void ConnectToLocalMongoDB()
        {
            //Don't have to tell where to connect because the database is the local 
            clientMongo = new MongoClient();

            //Connect to the database I created for the test
            databaseMongo = clientMongo.GetDatabase("surveyHack");

            //Connect to all collections that I need 
            collUser = databaseMongo.GetCollection<Collections.User>("user");
            collQuestionType = databaseMongo.GetCollection<Collections.QuestionType>("questionType");
            collQuestion = databaseMongo.GetCollection<Collections.Question>("questions");
            collAnswer = databaseMongo.GetCollection<Collections.Answer>("answer");
        }

        private static void MainProgram()
        {
            
            int command = 0;

            while (command != 10)
            {
                String operation = "" +
                    "What operation do you want to perform?" +
                    "\n\t(1) to list all the users available" +
                    "\n\t(2) create a new user" +
                    "\n\t(3) list all the question" +
                    "\n\t(4) take the survey" + 
                    "\n\t(5) show all the answer" + 
                    "\n\t(6) add a question to the survey" +
                    "\n\t(99) list all question types" +
                    "\n\t(10) quit the program\nCommand: ";

                Console.Write(operation);

                var temp = Console.ReadLine();
                command = Convert.ToInt32(temp);

                Console.WriteLine();
                switch (command)
                {
                    case 1:
                        ListAllUsers();
                        break;

                    case 2:
                        CreateNewUser();
                        break;

                    case 3:
                        ListAllQuestions();
                        break;

                    case 4:
                        TakeSurvey();
                        break;

                    case 5:
                        ListAllAnswer();
                        break;

                    case 6:
                        AddNewQuestion();
                        break;

                    case 99:
                        ListAllQuestionTypes();
                        break;
                }
                Console.WriteLine();
            }
        }


        /****************************************************************************************************************************
         *
         *  LIST ALL THE USER OF THE DB
         * 
         */
        private static void ListAllUsers()
        {
            Console.WriteLine("List of all the users:");

            var user = collUser.Find(b => true).ToListAsync().Result;
            
            foreach (var u in user)
            {
                Console.WriteLine("Fistname {0}, \t Lastname {1}, \t email {2}", u.name, u.lastname, u.email);
            }
        }


        /****************************************************************************************************************************
         *
         *  CREATE A NEW USER IN THE DB
         *  
         */
        private static void CreateNewUser()
        {
            var user = new Collections.User();

            Console.Write("Creating a new user\nFirstname of the new user: ");
            user.name = Console.ReadLine();

            Console.Write("Lastname of the new user: ");
            user.lastname = Console.ReadLine();

            Console.Write("Email of the new user: ");
            user.email = Console.ReadLine();

            collUser.InsertOne(user);
        }


        /****************************************************************************************************************************
         *
         *  LIST ALL THE QUESTIONS OF THE DB
         * 
         */
        private static void ListAllQuestions()
        {
            var question = collQuestion.Find(b => true).ToListAsync().Result;

            foreach (var q in question)
            {
                Console.WriteLine("Question: {0}, Answer: ", q.question);

                foreach (String s in q.answer)
                {
                    Console.WriteLine("\t" + s);
                }

                var questionType = collQuestionType.Find(b => b._id == q.questionTypeID).ToListAsync().Result;

                foreach (var qType in questionType)
                {
                    Console.WriteLine("Question type: {0}", qType.name);
                }

                Console.WriteLine();
            }
        }


        /****************************************************************************************************************************
         *
         *  ALLOW A PARTICIPANT TO TAKE THE SURVEY
         * 
         */
        private static void TakeSurvey()
        {
            Console.Write("Enter the name of the participant: ");
            var nameTemp = Console.ReadLine();
            var user = collUser.Find(b => b.name == Convert.ToString(nameTemp)).Limit(1).ToListAsync().Result;

            if (user.Count != 1)
            {
                Console.WriteLine("The user isn't in the database");
                return;
            }

            var question = collQuestion.Find(b => true).ToListAsync().Result;

            foreach (var q in question)
            {
                Console.WriteLine("Question: {0} \n\tAnswer:", q.question);

                for (int i = 0; i < q.answer.Length; i++)
                {
                    Console.WriteLine("\t{0} : {1}", i, q.answer[i]);
                }
                
                //Set the known information about the user and the question
                var answer = new Collections.Answer();
                answer.questionID = q._id;
                foreach (var u in user)
                {
                    answer.userID = u.Id;
                }

                var questionType = collQuestionType.Find(b => b._id == q.questionTypeID).ToListAsync().Result;

                foreach (var qType in questionType)
                {
                    Console.WriteLine("Question type: {0}", qType.name);

                    //This switch is used to choose the right method used when answering during the survey depending on the type of answer wanted
                    switch (qType.name)
                    {
                        case "One answer possible":
                            Console.Write("Your answer is: ");
                            var temp = Console.ReadLine();
                            int command = Convert.ToInt32(temp);

                            answer.answer = new String[1];
                            answer.answer[0] = q.answer[command];
                            break;

                        case "Multiple answers possible":
                            Console.Write("How many answer you have? ");
                            temp = Console.ReadLine();
                            int numberOfAnswer = Convert.ToInt32(temp);

                            answer.answer = new String[numberOfAnswer];
                            for (int i = 0; i < numberOfAnswer; i++)
                            {
                                Console.Write("Answer number {0}: ", i);
                                var temp2 = Console.ReadLine();
                                int answerTemp = Convert.ToInt32(temp2);
                                answer.answer[i] = q.answer[answerTemp];
                            }
                            break;
                    }
                }
                collAnswer.InsertOne(answer);
            }
        }


        /****************************************************************************************************************************
         *
         *  LIST ALL THE ANSWER OF THE DB
         * 
         */
        private static void ListAllAnswer()
        {
            var answer = collAnswer.Find(b => true).ToListAsync().Result;

            foreach (var a in answer)
            {
                var user = collUser.Find(b => b.Id == a.userID).Limit(1).ToListAsync().Result;

                Console.WriteLine("The answer is from: ");
                foreach (var u in user)
                {
                    Console.WriteLine("Fistname {0}, \t Lastname {1}, \t email {2}", u.name, u.lastname, u.email);
                }

                var question = collQuestion.Find(b => b._id == a.questionID).Limit(1).ToListAsync().Result;

                Console.WriteLine("\nThe question was: ");
                foreach (var q in question)
                {
                    Console.WriteLine("Question: {0}", q.question);
                }

                Console.WriteLine("\nThe participant answered: ");
                for (int i = 0; i < a.answer.Length; i++)
                {
                    Console.WriteLine("\t" + a.answer[i]);
                }

                Console.WriteLine();
            }
        }


        /****************************************************************************************************************************
         *
         *  CREATE A NEW QUESTION
         * 
         */
        private static void AddNewQuestion()
        {
            string temp;
            Console.Write("What's the question you want to add? ");
            var readConsole = Console.ReadLine();
            string stringConsole = Convert.ToString(readConsole);

            var questionToAdd = new Collections.Question();
            questionToAdd.question = stringConsole;

            Console.Write("What's the type of question? (entrer the number)\n");
            var questionType = collQuestionType.Find(b => true).ToListAsync().Result;

            int i = 0;
            foreach (var q in questionType)
            {
                Console.WriteLine("({0}) Type : {1}", i, q.name);
                i++;
            }

            i = 0;
            readConsole = Console.ReadLine();
            i = Convert.ToInt32(readConsole);

            int j = 0;
            foreach (var q in questionType)
            {
                if (j == i)
                {
                    questionToAdd.questionTypeID = q._id;
                }
                j++;
            }

            Console.Write("How many answer there is to this question? ");

            temp = Console.ReadLine();
            int numberOfAnswer = Convert.ToInt32(temp);

            questionToAdd.answer = new String[numberOfAnswer];
            for (int k = 0; k < numberOfAnswer; k++)
            {
                Console.Write("Answer number {0}: ", k);

                readConsole = Console.ReadLine();
                stringConsole = Convert.ToString(readConsole);

                questionToAdd.answer[k] = stringConsole;
            }

            collQuestion.InsertOne(questionToAdd);
        }


        /****************************************************************************************************************************
         *
         *  LIST ALL THE QUESTION TYPE OF THE DB
         * 
         */
        private static void ListAllQuestionTypes()
        {
            var questionType = collQuestionType.Find(b => true).ToListAsync().Result;

            foreach (var q in questionType)
            {
                Console.WriteLine("Name : {0}", q.name);
            }
        }
    }
}