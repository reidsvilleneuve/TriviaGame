using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriviaGame
{
    class Program
    {
        static void Main(string[] args)
        {
            //The logic for your trivia game happens here
            List<Trivia> AllQuestions = GetTriviaList();

            int score = 0;
            int highScore = 0;
            bool playAgain = true;

            while(playAgain)
            {
                score = triviaRound(AllQuestions);

                if (score > highScore) highScore = score;
                Console.WriteLine("Your score: {0}\nHigh score: {1}", score, highScore);
            }
            
        }


        //This functions gets the full list of trivia questions from the Trivia.txt document
        static List<Trivia> GetTriviaList()
        {
            //Get Contents from the file.  Remove the special char "\r".  Split on each line.  Convert to a list.
            List<string> contents = File.ReadAllText("trivia.txt").Replace("\r", "").Split('\n').ToList();

            //Each item in list "contents" is now one line of the Trivia.txt document.
            
            //make a new list to return all trivia questions
            List<Trivia> returnList = new List<Trivia>();


            foreach (string i in contents)
                returnList.Add(new Trivia(i));

            // TODO: go through each line in contents of the trivia file and make a trivia object.
            //       add it to our return list.
            // Example: Trivia newTrivia = new Trivia("what is my name?*question");
            //Return the full list of trivia questions
            return returnList;
        }

        /// <summary>
        /// Initiates a game of Trivia. Looping is handled here and score is returned.
        /// </summary>
        /// <param name="categorizedQuestions">An Object array in the format of: string catagory, Trivia(question/answer)</param>
        /// <returns>Score - answers correct / time.</returns>
        static int triviaRound(List<Trivia> questionList)
        {

            return 0;
        }
    }

    class Trivia
    {
        //TODO: Fill out the Trivia Object

        //The Trivia Object will have 2 properties
        // at a minimum, Question and Answer

        //The Constructor for the Trivia object will
        // accept only 1 string parameter.  You will
        // split the question from the answer in your
        // constructor and assign them to the Question
        // and Answer properties


        //Properties

        private string _question;
        public string Question
        {
            get { return _question; }
            set { _question = value; }
        }

        private string _answer;
        public string Answer
        {
            get { return _answer; }
            set { _answer = value; }
        }

        //Constructors

        public Trivia()
        {
            this.Question = "Undefined";
            this.Answer = "Undefined";
        }

        public Trivia(string question, string answer)
        {
            this.Question = question;
            this.Answer = answer;
        }

        public Trivia(string questionAsteriskAnswer)
        {
            List<string> splitList = questionAsteriskAnswer.Split('*').ToList();

            this.Question = splitList[0];
            this.Answer = splitList[1];
        }

        public Trivia(List<string> questionAnswer)
        {
            this.Question = questionAnswer[0];
            this.Answer = questionAnswer[1];
        }

        public Trivia(Trivia triviaEntry)
        {
            this.Question = triviaEntry.Question;
            this.Answer = triviaEntry.Answer;
        }


    }
}
