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
            Console.WindowWidth = Console.LargestWindowWidth - 30;

            int score = 0;
            int highScore = 0;
            bool playAgain = true;

            while (playAgain)
            {
                //The logic for your trivia game happens here
                List<Trivia> AllQuestions = GetTriviaList();
                List<List<Trivia>> categorizedQuestions = new List<List<Trivia>>();


                /*
                 * Sort each trivia question by category. Each new list is a new category.
                 * I.E: [ [lyric, lyrics, lyrics] [science, science science] [art, art, art] ]
                 */


                for (int i = 0; i < AllQuestions.Count; i++)
                {
                    bool noCategoryFound = true;
                    for (int j = 0; j < categorizedQuestions.Count; j++)
                        if (categorizedQuestions[j][0].Category == AllQuestions[i].Category) // If matching category found...
                        {
                            noCategoryFound = false;
                            categorizedQuestions[j].Add(AllQuestions[i]); //...add to list it was found in....
                        }

                    if (noCategoryFound)
                        categorizedQuestions.Add(new List<Trivia>() { AllQuestions[i] }); //...otherwise add to a new list.
                }


                /*
                 * Eliminate categories that have less than 20 questions.
                 */


                categorizedQuestions = categorizedQuestions.OrderBy(x => x.Count).ToList(); //Sort list in ascending order by length.

                while (categorizedQuestions[0].Count < 20) //Remove first entries until newest first entry has 20 questions.
                    categorizedQuestions.RemoveAt(0);

                /*
                 * TEMPORARY: Remove the category with the MOST questions. These are the Bonus questions, and they will be
                 * implemented on a later date. (TODO)
                 */

                categorizedQuestions.RemoveAt(categorizedQuestions.Count - 1); //List still sorted - this removes the last of this list.
            
                /*
                 * Play a round of Trivia. A round is composed of multiple questions and all input is taken care of by
                 * the function.
                 */


                score = TriviaRound(categorizedQuestions);
                Console.Clear();

                if (score > highScore) highScore = score; // Update high score if needed.
                Console.Write("Your score: {0}\nHigh score: {1}\n\nPress Y to try again, or any other key to exit. ", 
                    score, highScore);

                if (char.ToLower(Console.ReadKey(true).KeyChar) != 'y') //If user presses anything other than the Y key...
                    playAgain = false;                              //End the loop, closing the console.
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

            return returnList;
        }

        /// <summary>
        /// Initiates a game of Trivia. Looping is handled here and score is returned.
        /// </summary>
        /// <param name="categorizedQuestions">A 2-dimensional list, with like categories grouped in their own list.</param>
        /// <returns>Score - answers correct - answers incorrect - time.</returns>
        static int TriviaRound(List<List<Trivia>> categorizedQuestions)
        {
            // Will be used to calculate time spent during questions.
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now;
            double elapsed = 0;
            
            bool playing = true;
            
            /*
             * Game States:
             *  1- Intro
             *      Tells the user the rules.
             *  2- Category selection
             *      Randomly selects categories - any 5 can pop up.
             *  3- Question phase
             *      Multiple question answers pulled from like 
             *  4- Bonus question phase (TO DO!)
             *      Non-categorized questions that are NOT multiple choice.
             *  5- End of game. Points calculated and returned.
             *  
             * The game consists of 3 category selections phases, with 5 questions in-between.
             * 
             * Score is calculated like so: (Questions correct * 5) + (Bonus Questions correct * 3) <--- TO DO
             *      - (Questions incorrect * 2) - (Time in seconds)
             *      
             * Note that bonus questions answered incorrectly do NOT count against the player's score.
             * 
             */

            int gameState = 1;

            int categoryPhasesLeft = 3;
            int questionsLeft = 5;

            char input = ' '; //Will be used when input is required by player.
            bool inputCorrect = false; //Flags if input if valid.

            int correctAnswers = 0;
            int incorrectAnswers = 0;

            //Will be populated with questions from category of user's choice by categorySelect function.
            List<Trivia>questionsInCategory = new List<Trivia>(); 

            while(playing)
            {
                Console.Clear();
                Console.WriteLine();

                switch(gameState)
                {
                    case 1:

                        // Intro game state. Loops until user presses R or r.

                        input = ' '; // To avoid infinate loops on a non-'r' keypress.
                        Console.Clear();
                        Console.Write("\nWelcome to That Trivia Game!\n\nThis game has three rounds. Each round allows you to " +
                            "select from five randomly\nchosen categories, and each category will give you 5 multiple-choice " +
                            "questions.\n\nCareful! Answering a " + // Space left for Bonus Question instructions.
                            "question incorrectly will subtract from your score!\n\nThese rounds are timed! The " +
                            "faster you choose categories and answer the\nquestions, the higher your score will be!\n\nPress " +
                            "R when you are ready to start the timer, and begin!");

                        input = char.ToLower(Console.ReadKey().KeyChar); //Capture user keypress, convert to lowercase.

                        if (input == 'r')
                        {
                            startTime = DateTime.Now; // Start the timer.
                            gameState = 2;
                        }
                        break;

                    case 2:

                        // Category selection game state. CategoryList function handles input and loops.
                        
                        questionsInCategory = CategoryList(ref categorizedQuestions); //Will remove categories as they are used.
                        categoryPhasesLeft--;

                        gameState = 3; //Go to Question game state.
                        
                        break;

                    case 3:

                        // Question game state. Pass in list generated by CategoryList funcion. Increment correct/incorrect counters.

                        if (AskQuestion(ref questionsInCategory)) //Returns true on correct answer. Removed when asked.
                            correctAnswers++;
                        else
                            incorrectAnswers++;

                        questionsLeft--;

                        if (questionsLeft == 0)
                        {
                            if (categoryPhasesLeft == 0) //No more category phases left?
                                gameState = 5; //Go to End game state
                            else
                            {
                                gameState = 2; // Go to Category game state.
                                questionsLeft = 5;
                            }

                        }

                        //Will loop back here if questionsLeft > 0.
                        break;

                    case 4:

                        // Bonus question state. This is TODO and is not active on this release.

                        break;

                    case 5:
                  
                        // End of Game. Calculate totals and return the player score.

                        endTime = DateTime.Now;
                        elapsed = Math.Round(endTime.Subtract(startTime).TotalSeconds, 0);
                        
                        //(Questions correct * 5) - (Questions incorrect * 2) - (Time in seconds)
                        Console.Clear();
                        Console.WriteLine("\n---------------------COMPLETE!!---------------------\n\n" +
                            "Total correct: {0}.\nTotal incorrect: {1}.\nTime spent: {2} seconds.\n\nPress any key to continue...", 
                            correctAnswers, incorrectAnswers, elapsed);
                        
                        Console.ReadKey();

                        return Convert.ToInt32((correctAnswers * 20) - (incorrectAnswers * 2) - elapsed);
                }
            }
            

            Console.ReadKey(); 

            return 0;     
        }

        /// <summary>
        /// Asks user which category they want to select, returns the selected category list, and removes it from the refrenced
        /// 2-dimensional list.
        /// </summary>
        /// <param name="categorizedQuestions">2-diensinal array of Lists of Trivias, separated by category.</param>
        /// <returns>List of Trivias related to the category that the user selects.</returns>
        static List<Trivia> CategoryList(ref List<List<Trivia>> categorizedQuestions)
        {
            bool inputWrong = true; //Will be set to true when input if sucessfully validated.
            Random rng = new Random();
            int randomIndex = 0;

            char input = ' '; //User input.
            int selectedIndex; //User input converted to int.

            List<int> pullFromList = new List<int>(); //Random Number acquiring lists - see below.
            List<int> selectionList = new List<int>();

            List<Trivia> returnList = new List<Trivia>(); //List of Trivias that will be returned.

            //Generate list of ints from 0 to the Count of categorizedQuestion - 1.
            for (int i = 0; i < categorizedQuestions.Count; i++)
                pullFromList.Add(i);

            //Transer 5 random values from pullFromList to selectionList.
            for(int i = 0; i < 5; i++)
            {
                randomIndex = rng.Next(0, pullFromList.Count);
                selectionList.Add(pullFromList[randomIndex]);
                pullFromList.RemoveAt(randomIndex);
            }

            //Output random category selection to user and allow them to select one.
            do
            {
                Console.Clear();
                Console.WriteLine("\nSelect your category!\n---------------------");
                for (int i = 0; i < selectionList.Count; i++)
                {
                    //Category at 0 represents entire List. Pull random number as selected by selectionList's value of index i.
                    string categoryName = ProperCase(categorizedQuestions[selectionList[i]][0].Category);
                    Console.WriteLine("{0}: {1}", i + 1, categoryName);
                }
                input = Console.ReadKey().KeyChar;

                if(int.TryParse(input.ToString(), out selectedIndex)) // Is input a number?
                {
                    if(selectedIndex > 0 && selectedIndex <= selectionList.Count) //If so, is it between 1 and 5?
                    {
                        inputWrong = false; //Stops from looping again.
                        
                        //Store the category list chosen by the user into the variable that we will return.
                        returnList = categorizedQuestions[selectionList[selectedIndex - 1]]; //"- 1": Index is 0-based.
                    }
                }

            } while (inputWrong);

            //Remove the category from ref categorizedQuestions so it doesn't appear again
            categorizedQuestions.RemoveAt(selectionList[selectedIndex - 1]);

            return returnList;
        }

        /// <summary>
        /// Prompts the user with a question, in a multiple-choice format.
        /// </summary>
        /// <param name="questions">List of Trivia objects. It is recommended, but not required, that they be
        /// of the same category.</param>
        /// <returns>True if answered correctly, false if not.</returns>
        static bool AskQuestion(ref List<Trivia> questions)
        {
            bool inputWrong = true;
            Random rng = new Random();
            int randomIndex = 0;

            char input = ' '; //User input.
            int selectedIndex; //User input converted to int.


            List<int> pullFromList = new List<int>(); //Random Number acquiring lists - see below.
            List<int> selectionList = new List<int>();

            //Generate list of ints from 0 to the Count of questions - 1.
            for (int i = 0; i < questions.Count; i++)
                pullFromList.Add(i);

            //Transer 5 random values from pullFromList to selectionList.
            for (int i = 0; i < 5; i++)
            {
                randomIndex = rng.Next(0, pullFromList.Count);
                selectionList.Add(pullFromList[randomIndex]);
                pullFromList.RemoveAt(randomIndex);
            }

            //The values of selectionList now represent index numbers of the questions parameter.

            //Randomly choose 1 of the 5 Trivias to be the correct answer.
            randomIndex = rng.Next(0, selectionList.Count);

            //Output: Display category, ask the correct answer's question, and display all answers.  Loop on incorrect input.

            do
            {
                Console.Clear();
                Console.WriteLine("\nCategory: {0}\n---------------------\n{1}\n", ProperCase(questions[selectionList[randomIndex]].Category),
                    ProperCase(questions[selectionList[randomIndex]].Question));

                for (int i = 0; i < selectionList.Count; i++ )
                    Console.WriteLine("{0}: {1}", i + 1, ProperCase(questions[selectionList[i]].Answer));
                
                input = Console.ReadKey().KeyChar;

                if (int.TryParse(input.ToString(), out selectedIndex)) // Is input a number?
                {
                    if (selectedIndex > 0 && selectedIndex <= selectionList.Count) //If so, is it between 1 and 5?
                    {
                        questions.RemoveAt(selectionList[randomIndex]); //Remove question from list so it is not asked again.
                        if (selectedIndex == randomIndex + 1) //User was correct
                        {
                            Console.Clear();
                            Console.Write("\nCorrect!\n\nPress any key to continue...");
                            Console.ReadKey();
                            return true;
                        }
                        else
                        {
                            Console.Clear();
                            Console.Write("\nWrong!\n\nPress any key to continue...");
                            Console.ReadKey();
                            return false;
                        }
                    }
                } // Will loop again if input is incorrect.

            } while (inputWrong);

            return true;
            
        }

        /// <summary>
        /// Returns the given string with the first character capitalized.
        /// </summary>
        /// <param name="input">String to apply proper capitalization to.</param>
        /// <returns>Properly capitalized string.</returns>
        static string ProperCase(string input)
        {
            if (input.Length == 1) return input.ToUpper();
            return input.Substring(0, 1).ToUpper() + input.Substring(1, input.Length - 1);
        }
    }

    /// <summary>
    /// A single Trivia question. Contains a category, a question, and an answer.
    /// </summary>
    class Trivia
    {
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

        private string _category;
        public string Category
        {
            get { return _category.ToLower(); }
            set { _category = value.ToLower(); }
        }

        //Constructors

        public Trivia()
        {
            this.Category = "Undefined";
            this.Question = "Undefined";
            this.Answer = "Undefined";
        }

        public Trivia(string category, string question, string answer)
        {
            this.Category = category;
            this.Question = question;
            this.Answer = answer;
        }

        public Trivia(string questionAsteriskAnswer)
        {
            //Split the question and answer to a seperate entry in a list and set each to the
            //appropriate property.

            List<string> splitList = questionAsteriskAnswer.Split('*').ToList();

            this.Question = splitList[0];
            this.Answer = splitList[1];

            //Pull the category (Text that is before the ":") and set it as this Trivia's property.
            //Entries without a category are a bonus question.

            if (this.Question.Contains(':'))
            {
                this.Category = this.Question.Substring(0, this.Question.IndexOf(':'));
                this.Question = this.Question.Remove(0, this.Question.IndexOf(':') + 2); //Remove category + " " from Question.
            }
            else
                this.Category = "Bonus Questions"; // These won't show up in this version.
        }

        public Trivia(List<string> categoryQuestionAnswer)
        {
            this.Category = categoryQuestionAnswer[0];
            this.Question = categoryQuestionAnswer[1];
            this.Answer = categoryQuestionAnswer[2];
        }

        public Trivia(Trivia triviaEntry)
        {
            this.Question = triviaEntry.Question;
            this.Answer = triviaEntry.Answer;
            this.Category = triviaEntry.Category;
        }


    }
}
