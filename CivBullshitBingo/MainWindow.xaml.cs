using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CivBullshitBingo
{
    /// <summary>
    /// Display Modes (White/Dark Mode)
    /// </summary>
    enum DisplayModes { White, Dark }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DisplayModes DisplayMode = DisplayModes.White;

        static readonly Brush WhiteWindowBackgroundColor = Brushes.White;
        static readonly Brush WhiteOpenPhraseColor = Brushes.Black;
        static readonly Brush WhiteMarkedPhraseColor = Brushes.Red;

        static readonly Brush DarkWindowBackgroundColor = Brushes.Black;
        static readonly Brush DarkOpenPhraseColor = Brushes.White;
        static readonly Brush DarkMarkedPhraseColor = Brushes.Red;

        Brush WindowBackgroundColor => DisplayMode == DisplayModes.White ? WhiteWindowBackgroundColor : DarkWindowBackgroundColor;
        Brush OpenPhraseColor => DisplayMode == DisplayModes.White ? WhiteOpenPhraseColor : DarkOpenPhraseColor;
        Brush MarkedPhraseColor => DisplayMode == DisplayModes.White ? WhiteMarkedPhraseColor : DarkMarkedPhraseColor;

        static public Random Random = new Random();

        public List<string> PhraseList { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ReadPhrases(string fileName)
        {
            PhraseList.Clear();

            using (var sr = new StreamReader(fileName))
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
        }

        public void CreateBingo()
        {
            int phraseNo = 0;
            for (int x = 0; x < Grid.ColumnDefinitions.Count; x++)
                for (int y = 0; y < Grid.RowDefinitions.Count; y++)
                {
                    var element = Grid.Children.Cast<TextBlock>().
                        FirstOrDefault(tb => Grid.GetColumn(tb) == x && Grid.GetRow(tb) == y);

                    element.Foreground = OpenPhraseColor;

                    if (x == 2 && y == 2) // add static jokers
                        SetJokerField(element);
                    else
                    {
                        var phraseText = PhraseList[phraseNo++].Trim();

                        if (phraseText.ToLower() == "joker") // add dynamic jokers
                            SetJokerField(element);
                        else
                            SetPhraseField(element, phraseText);
                    }
                }
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBlock;
            if (!Grid.Children.Cast<TextBlock>().Contains(tb)) return;

            if (tb?.Foreground == OpenPhraseColor) tb.Foreground = MarkedPhraseColor;
            else if (tb?.Foreground == MarkedPhraseColor) tb.Foreground = OpenPhraseColor;
        }

        private void SetPhraseField(TextBlock element, String phrase)
        {
            SetTextBlockStyle(element, "PhraseTextBlock");
            element.Text = phrase;
        }

        private void SetJokerField(TextBlock element)
        {
            SetTextBlockStyle(element, "JokerTextBlock");
            element.Text = "Joker";
        }

        private void SetTextBlockStyle(TextBlock element, String styleName)
        {
            var style = FindResource(styleName) as Style;
            if (style != null)
                element.Style = style;
        }

        private void ButtonsNew_Click(object sender, RoutedEventArgs e)
        {
            var file = String.Empty;
            if (sender == ButtonNewCiv)
                file = "civPhrases.txt";
            else if (sender == ButtonNewLol)
                file = "lolPhrases.txt";

            if (!String.IsNullOrWhiteSpace(file))
            {
                ReadPhrases(file);
                CreateBingo();
            }
        }

        private void ButtonUpdatePhrases_Click(object sender, RoutedEventArgs e)
        {
            // todo
        }

        private void ToggleMode_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tb in Grid.Children.Cast<TextBlock>())
            {
                if (tb.Foreground == WhiteOpenPhraseColor) tb.Foreground = DarkOpenPhraseColor;
                else if (tb.Foreground == DarkOpenPhraseColor) tb.Foreground = WhiteOpenPhraseColor;

                else if (tb.Foreground == WhiteMarkedPhraseColor) tb.Foreground = DarkMarkedPhraseColor;
                else if (tb.Foreground == DarkMarkedPhraseColor) tb.Foreground = WhiteMarkedPhraseColor;
            }

            DisplayMode = (DisplayMode == DisplayModes.White) ? DisplayModes.Dark : DisplayModes.White;
            Background = WindowBackgroundColor;
            Notes.Foreground = OpenPhraseColor;
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
