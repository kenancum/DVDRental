using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Transactions;
using Npgsql;

namespace DVDRental
{
    class Program
    {
        static void Main(string[] args)
        {
            string firstName, lastName, birthday, title;
            bool exit=false;
            ConsoleKeyInfo selection;
            int clientID,copyID, year, month, day,  movieYear, age_restriction;
            double price;
            while (!exit)
            {
                Console.WriteLine("******************************");
                Console.WriteLine("* WELCOME TO THE RENTAL APP! *");
                Console.WriteLine("******************************");
                Console.WriteLine("1) Available movies");
                Console.WriteLine("2) Rentals from a client");
                Console.WriteLine("3) Create a new rental");
                Console.WriteLine("4) Create a new return");
                Console.WriteLine("5) Create a new user");
                Console.WriteLine("6) Create a new movie");
                Console.WriteLine("7) Statistics between dates");
                Console.WriteLine("8) Overdued Rentals");
                Console.WriteLine("9) Exit");

                selection = Console.ReadKey();
                Console.Clear();
                switch (selection.KeyChar)
                {
                    case '1':
                        Console.WriteLine("Available movies\n");
                        getMovieTitleAndAvailable();

                        toMainMenu();
                        break;
                    case '2':
                        Console.WriteLine("Rentals from client\n");

                        Console.WriteLine("Client ID: ");
                        clientID = Int32.Parse(Console.ReadLine());

                        Console.Clear();

                        getClientsRentalsByID(clientID);

                        toMainMenu();
                        break;
                    case '3':
                        Console.WriteLine("Create a rental\n");

                        Console.WriteLine("Client ID: ");
                        clientID = Int32.Parse(Console.ReadLine());

                        Console.WriteLine("Copy ID: ");
                        copyID = Int32.Parse(Console.ReadLine());

                        Console.Clear();

                        addRentalbyClientCopyID(clientID,copyID);

                        toMainMenu();
                        break;
                    case '4':
                        Console.WriteLine("Create a return\n");

                        Console.WriteLine("Copy ID: ");
                        copyID = Int32.Parse(Console.ReadLine());

                        Console.Clear();

                        addRentalReturnbyCopyID(copyID);

                        toMainMenu();
                        break;

                    case '5':
                        Console.WriteLine("Create a new user\n");
                        Console.WriteLine("Name: ");
                        firstName = Console.ReadLine();

                        Console.WriteLine("Last Name: ");
                        lastName = Console.ReadLine();

                        Console.Clear();

                        Console.WriteLine("Birthday\n");

                        Console.WriteLine("Day: ");
                        day = Int32.Parse(Console.ReadLine());

                        Console.WriteLine("Month: ");
                        month = Int32.Parse(Console.ReadLine());

                        Console.WriteLine("Year: ");
                        year = Int32.Parse(Console.ReadLine());

                        addNewClient(firstName,lastName, dataToDate(year, month, day));
                        toMainMenu();
                        break;
                    case '6':
                        Console.WriteLine("Create a movie\n");
                        Console.WriteLine("Title: ");
                        title = Console.ReadLine();

                        Console.WriteLine("Year: ");
                        movieYear = Int32.Parse(Console.ReadLine());

                        Console.WriteLine("Year: ");
                        age_restriction = Int32.Parse(Console.ReadLine());

                        Console.WriteLine("Year: ");
                        price = double.Parse(Console.ReadLine());

                        addNewMovieandCopy(title,movieYear,age_restriction,price);
                        toMainMenu();
                        break;
                    case '7':
                        Console.WriteLine("Statistics between dates\n");

                        Console.WriteLine("Day: ");
                        day = Int32.Parse(Console.ReadLine());

                        Console.WriteLine("Month: ");
                        month = Int32.Parse(Console.ReadLine());

                        Console.WriteLine("Year: ");
                        year = Int32.Parse(Console.ReadLine());

                        getTotalPriceByRentalDate(dataToDate(year, month, day));
                        toMainMenu();
                        break;
                    case '8':
                        Console.WriteLine("Overdued Rentals");
                        getOverduedRenteals();
                        toMainMenu();
                        break;
                    case '9':
                        exit = true;
                        break;
                }
                Console.Clear();
                Console.WriteLine("Good bye!");
            }

        }



        static void getMovieTitleAndAvailable()
        {
            int counter;
            int lenght = MovieMapper.Instance.movieCount();
            Console.WriteLine("Title" + "{0,37}\t","Copies\n");
            for (int i = 0; i < lenght; i++)
            {
                counter = 0;
                Movie movie = MovieMapper.Instance.GetByID(i+1);
                foreach (var item in movie.Copies)
                {
                    if (item.available)
                        counter++;
                }
                Console.Write("{0,-32}\t",movie.title);
                Console.WriteLine("{0,2}\t", counter);
            }
            
            
            

        }
        static void getClientsRentalsByID(int clientID)
        {
            Client client = ClientMapper.Instance.GetByID(clientID);
            Console.WriteLine($"Client: {client.name} {client.surname}\n");
            List<Rental> rentals = RentalMapper.Instance.GetByClientId(clientID);

            List<string> active = new List<string>();
            List<string> passive = new List<string>();
            string movieTitle;
            foreach (var item in rentals)
            {
                movieTitle = MovieMapper.Instance.GetByID(CopyMapper.Instance.GetByID(item.copyId).movieId).title;
                if (string.IsNullOrEmpty(item.dateOfReturn))
                {
                    active.Add(movieTitle);
                }
                else
                    passive.Add(movieTitle);
            }
            Console.WriteLine("Active\n");
            foreach (var item in active)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();

            Console.WriteLine("Passive\n");
            foreach (var item in passive)
            {
                Console.WriteLine(item);
            }
        }
        static void addRentalbyClientCopyID(int clientID,int copyID)
        {
            Rental rental = new Rental(null, clientID, copyID);
            RentalMapper.Instance.Save(rental);
        }
        static void addRentalReturnbyCopyID(int copyID)
        {
            Rental rental = RentalMapper.Instance.getByCopyID(copyID);
            RentalMapper.Instance.Save(rental);
        }
        static void addNewClient(string firstName, string lastName,string birthday)
        {
            Client client = new Client(null, firstName, lastName, birthday);
            ClientMapper.Instance.Save(client);
        }
        static void addNewMovieandCopy(string title,int year, int age_restriction,double price)
        {
            Movie movie = new Movie(title,year,age_restriction,0,price);
            MovieMapper.Instance.Save(movie);
        }
        static void getOverduedRenteals()
        {
            List<Rental> overduedRentals = RentalMapper.Instance.overdueRentals();
            Console.WriteLine("Rental ID");
            foreach (var item in overduedRentals)
            {
                Console.WriteLine(item.id);
            }
        }
        static void toMainMenu()
        {
            Console.WriteLine("\nPress Enter to Main Menu");
            Console.ReadLine();
        }
        static int getTotalPriceByRentalDate(string date)
        {
            return RentalMapper.Instance.getSumPriceFromDate(date);
        }
        static string dataToDate(int year, int month, int day)
        {
            return year.ToString() + "-" + month.ToString() + "-" + day.ToString() + " 00:00:00.00 +01:00";
        }
    }
}
