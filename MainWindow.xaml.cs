using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
   
    public partial class MainWindow : Window

    {
        const int Cell = 20, Cols = 20, Rows = 20; //maga a jatektér, 20px egy cella és akk 20x20 cella ennyi az egesz

        readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(120) };//120ms-enként meghivja a Step() methodot
        readonly Random random = new();
        List<Point> snake = new();     //cellakbol allo kigyo, nulladik elem a feje [0]
        Point food; //kaja pozicioja
        Point direction = new(1, 0), nextDirection = new(1, 0);   //kigyo mozgasi iranya, a kovetkezo iranyt a amit adunk a nextDirection tarolja
        int score;

        public MainWindow() //konstruktor
        {
            InitializeComponent(); //xaml fileban levo dolgokat inicializalja azaz elinditja i guess betölti
            timer.Tick += (_, _) => Step(); //timer tick esemenyhez rendeli a Step() methodot
            Start(); //elinditja a jatekot
        }

        void Start()
        {
            snake = new() {  new(10,10), new(9,10), new(8,10) }; //alapbol 3 cellas a kigyo 10,10-es poziciobol indulva  x és y koordinatak
            direction = nextDirection = new(1,0);  //beallitja a kigyo mozgasi iranyat jobbra
            score = 0;
            PlaceFood();
            timer.Start();
            Draw();
        }

        void PlaceFood()
        {
            do food = new Point(random.Next(Cols), random.Next(Rows)); //random pozicioba rakja a kajat és addig csinalja amig (while) a kigyo nem tartalmazza azt a poziciot
            while (snake.Contains(food));
        }

        void Step() //egy lepes
        {
            direction = nextDirection; // beallitja a direction-t a nextDirection-re
            var head = new Point(snake[0].X + direction.X, snake[0].Y + direction.Y); //a fej poziciojat a kigyo elso elemebol es a mozgasi iranybol szamolja ki


            if (head.X < 0 || head.X >= Cols || head.Y < 0 || head.Y >= Rows || snake.Contains(head)) //ha a fej kimegy a jatekterrol vagy a kigyo magaba megy bele
            {
                timer.Stop();
                if (MessageBox.Show($"Vége! Pont: {score}\nÚj játék?", "Snake",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes) Start();
                else Close();
                return;
            }

            snake.Insert(0,head); //hozzaadja a fej poziciojat a kigyo elejere
            if (head == food)
            {
                score++;
                PlaceFood();
            }
            else snake.RemoveAt(snake.Count - 1); //ha nem evett a kigyo akkor eltavolitja a vegso elemet a kigyobol, igy mozog
            Draw(); //ujrarajzolja a jatekteret
        }

        void Draw() //torli a canvas tartalmat, kiirja a scoret és kirajzolja a kajat es a kigyot
        { 
            Board.Children.Clear();  //torli a canvas tartalmat
            ScoreText.Text = $"Pont: {score}";
            AddRect(food, Brushes.Red); //kirajzolja a kajat
            foreach (var segment in snake) AddRect(segment, Brushes.LimeGreen); //kirajzolja a kigyot
        }

        void AddRect(Point p, Brush color) //kirajzol egy cellat a canvasra a megadott pozicioba es szinnel ez esetben a kaja piros a kigyo zold
        {
            var rectangle = new Rectangle
            {
                Width = Cell -1,
                Height = Cell -1,
                Fill = color
            };
            Canvas.SetLeft(rectangle, p.X * Cell);
            Canvas.SetTop(rectangle, p.Y * Cell);
            Board.Children.Add(rectangle);
        }

        void Window_KeyDown(object sender, KeyEventArgs e) //mozgas
        {
            switch (e.Key)
            {
                case Key.Up when direction.Y == 0: nextDirection = new(0, -1); break; //ha a kigyo nem felfele megy akkor beallitja a kovetkezo iranyt felfele
                case Key.Down when direction.Y == 0: nextDirection = new(0, 1); break; //ha a kigyo nem lefele megy akkor beallitja a kovetkezo iranyt lefele
                case Key.Left when direction.X == 0: nextDirection = new(-1, 0); break; //ha a kigyo nem balra megy akkor beallitja a kovetkezo iranyt balra
                case Key.Right when direction.X == 0: nextDirection = new(1, 0); break; //ha a kigyo nem jobbra megy akkor beallitja a kovetkezo iranyt jobbra
            }
            
        }
    }
}