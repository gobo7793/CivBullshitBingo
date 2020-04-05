using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CivBullshitBingo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly private Brush OpenPhraseColor = Brushes.Black;
        static readonly private Brush ClickedPhraseColor = Brushes.Red;
        static public Random Random = new Random();

        public List<string> PhraseList { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBlock;
            if (tb?.Foreground == OpenPhraseColor) tb.Foreground = ClickedPhraseColor;
            else if (tb?.Foreground == ClickedPhraseColor) tb.Foreground = OpenPhraseColor;
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            PhraseList.Clear();

            using (var sr = new StreamReader("phrases.txt"))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!String.IsNullOrWhiteSpace(line)) PhraseList.Add(line);
                }
            }

            if (PhraseList.Count < 24)
                return; // to lazy for output, that txt contains not enough phrases

            PhraseList.Shuffle();

            int phraseNo = 0;
            for (int x = 0; x < Grid.ColumnDefinitions.Count; x++)
                for (int y = 0; y < Grid.RowDefinitions.Count; y++)
                {
                    var element = Grid.Children.Cast<TextBlock>().
                        FirstOrDefault(e => Grid.GetColumn(e) == x && Grid.GetRow(e) == y);

                    if (x == 2 && y == 2) // add jokers
                    {
                        element.Text = element.Text = "Joker";
                        element.FontSize = 20;
                        element.FontWeight = FontWeights.Bold;
                    }
                    else
                        element.Text = PhraseList[phraseNo++];
                }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    static class MyExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = MainWindow.Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
