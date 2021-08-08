using DevExpress.Xpf.Core;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace DXRichTextControlHighlightText01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

            editor.ApplyTemplate();
            // editor.ActiveView.ZoomFactor = 0.5f;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // editor.HorizontalRulerOptions = new DevExpress.Xpf.RichEdit.DXRichEditHorizontalRulerOptions() { Visibility = RichEditRulerVisibility.Hidden };
            // editor.VerticalRulerOptions = new DevExpress.Xpf.RichEdit.DXRichEditVerticalRulerOptions { Visibility = RichEditRulerVisibility.Hidden };
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                myStream = openFileDialog.OpenFile();
                if (myStream != null)
                {
                    string fn = openFileDialog.FileName;
                    string txt = File.ReadAllText(fn);
                    editor.Text = txt;
                }
            }
        }

        DocumentRange[] finds;
        int findIndex = 0;
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ClearHighlight(finds);

            findIndex = 0;

            finds = editor.Document.FindAll(searchTxt.Text.Trim(), DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord);

            foreach (var find in finds)
            {
                Highlight(find, System.Drawing.Color.Yellow);
            }

            ScrollToDocumentRange(finds);
        }

        internal void ClearHighlight(DocumentRange[] finds)
        {
            if (finds == null)
                return;

            foreach (var find in finds)
            {
                Highlight(find, System.Drawing.Color.Transparent);
            }
        }

        internal void Highlight(DocumentRange find, System.Drawing.Color color)
        {
            if (find == null)
                return;

            var doc = find.BeginUpdateDocument();
            var charprop = doc.BeginUpdateCharacters(find);
            charprop.BackColor = color;
            doc.EndUpdateCharacters(charprop);
            find.EndUpdateDocument(doc);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            ScrollToDocumentRange(finds, "Backward");
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            ScrollToDocumentRange(finds, "Forward");
        }

        internal void ScrollToDocumentRange(DocumentRange[] finds, string token = "")
        {
            if (finds.Length == 0)
                return;

            if (token == "Forward")
            {
                findIndex++;

                if (findIndex >= finds.Length)
                    findIndex = 0;

                ISearchResult searchResult = editor.Document.StartSearch(searchTxt.Text.Trim(), SearchOptions.WholeWord, SearchDirection.Forward, finds[findIndex]);
                searchResult.FindNext();
                editor.Document.CaretPosition = searchResult.CurrentResult.Start;
                editor.ScrollToCaret();
                editor.Document.Selection = finds[findIndex];
            }
            else if (token == "Backward")
            {
                findIndex--;

                if (findIndex < 0)
                    findIndex = finds.Length - 1;

                ISearchResult searchResult = editor.Document.StartSearch(searchTxt.Text.Trim(), SearchOptions.WholeWord, SearchDirection.Forward, finds[findIndex]);
                searchResult.FindNext();
                editor.Document.CaretPosition = searchResult.CurrentResult.Start;
                editor.ScrollToCaret();
                editor.Document.Selection = finds[findIndex];
            }
            else
            {
                ISearchResult searchResult = editor.Document.StartSearch(searchTxt.Text.Trim(), SearchOptions.WholeWord, SearchDirection.Forward, finds[findIndex]);
                searchResult.FindNext();
                editor.Document.CaretPosition = searchResult.CurrentResult.Start;
                editor.ScrollToCaret();
                editor.Document.Selection = finds[findIndex];
            }
        }
    }
}
