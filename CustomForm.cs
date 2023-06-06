using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WordPad
{
    public partial class CustomForm : Form
    {
        static string saveFileName = "";  // путь к файлу который сейчас открыт
        static string sPathToPicture = "";  // путь к файлу изображения
        static int iNextSearchPoint = 0;  // указывает на символ, с которого будет искаться строка
        static int iReplacesCount = 0;  // кол-во замен в тексте

        static string sEtalon = "";  // строка для хранения исходного содержимого загруженного файла
        static bool isChanged = false;  // изменился ли текст
        static bool isFirstSave = true;  // первый ли раз сохранили файл

        // переменные для richTextBox1.SelectionFont
        static int size = 8;
        static string fontFamily = "Ariel";
        static FontStyle fontStile = FontStyle.Regular;

        // для жирности, курсива и подчёркивания
        static bool bold = false;
        static bool italic = false;
        static bool underlined = false;

        // для пред. печати
        static int visibleLines = 0;
        static int totalPage = 0;
        static int pageNow = 0;
        static int from = 0;
        static int to = 0;

        // для отображения сведений в статус бар
        static int index, line, col;

        SearchForm sf = new SearchForm();
        ReplaceForm rf = new ReplaceForm();
        OpenFileDialog ofdParserPDF = new OpenFileDialog();


        public CustomForm()
        {
            InitializeComponent();
            Text = "NodePad";
            StartPosition = FormStartPosition.CenterScreen;

            // событие для перетаскивания
            richTextBox.AllowDrop = true;
            richTextBox.DragEnter += RichTextBox1_DragEnter;
            richTextBox.DragDrop += RichTextBox1_DragDrop;

            ComboBox1.SelectedIndex = 0;
            ComboBox2.SelectedIndex = 0;

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
        }

        private void UpdateFormText()
        {
            if (isChanged) Text = "NodePad*";
            else Text = "NodePad";
        }


        // Перетаскивание файла
        private void RichTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            try
            {
                richTextBox.LoadFile(files[0]);
                saveFileName = files[0];
            }
            catch (ArgumentException)
            {
                if (Path.GetExtension(files[0]).Equals(".txt"))  // txt
                {
                    using (var fs = new FileStream(files[0], FileMode.Open, FileAccess.Read))
                    using (var sr = new StreamReader(fs))
                    {
                        richTextBox.Text = sr.ReadToEnd();
                    }
                    saveFileName = files[0];
                }
                else  // pdf
                {
                    ofdParserPDF.FileName = files[0];
                    backgroundWorker1.RunWorkerAsync();  // запускаем другой поток
                }
            }
            sEtalon = richTextBox.Text;
            isChanged = false;
            isFirstSave = true;
            UpdateFormText();
        }

        private void RichTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    string fileExtension = Path.GetExtension(files[0]);
                    if (fileExtension.Equals(".txt") || fileExtension.Equals(".pdf"))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }


        // сохранение файла
        private void SaveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text = saveFileName;
            if (isChanged)  // если файл изменился
            {
                bool isSave = true;
                if (String.IsNullOrEmpty(saveFileName))
                {
                    if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                    saveFileName = saveFileDialog1.FileName;
                    richTextBox.SaveFile(saveFileName);
                }
                else
                {
                    if (isFirstSave)
                    {
                        if (MessageBox.Show("Данные будут изменены\nВы согласны?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            richTextBox.SaveFile(saveFileName);
                            isFirstSave = false;
                        }
                        else isSave = false;
                    }
                    else
                    {
                        richTextBox.SaveFile(saveFileName);
                    }
                }

                if (isSave)
                {
                    isChanged = false;
                    UpdateFormText();
                }
            }
        }

        // сохранение файла save as
        private void SaveAsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveFileName = saveFileDialog1.FileName;
                richTextBox.SaveFile(saveFileName);

                isChanged = false;
                isFirstSave = true;
                UpdateFormText();
            }
        }

        // открытие файла
        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveFileName = openFileDialog1.FileName;
                try
                {
                    richTextBox.LoadFile(saveFileName);
                }
                catch (ArgumentException)
                {
                    using (var fs = new FileStream(saveFileName, FileMode.Open, FileAccess.Read))
                    using (var sr = new StreamReader(fs))
                    {
                        richTextBox.Text = sr.ReadToEnd();
                    }
                }
                sEtalon = richTextBox.Text;
                isChanged = false;
                isFirstSave = true;
                UpdateFormText();
            }
        }


        // базовые операции
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.SelectAll();
        }

        private void New_Click(object sender, EventArgs e)
        {
            richTextBox.Clear();
        }

        private void Redo_Click(object sender, EventArgs e)
        {
            richTextBox.Redo();
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            richTextBox.Undo();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            richTextBox.Copy();
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            richTextBox.Paste();
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            richTextBox.Cut();
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.SelectedText = "";
        }

        private void LeftJustifyButton_Click(object sender, EventArgs e)
        {
            richTextBox.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void CenterJustifyButton_Click(object sender, EventArgs e)
        {
            richTextBox.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void RightJustifyButton_Click(object sender, EventArgs e)
        {
            richTextBox.SelectionAlignment = HorizontalAlignment.Right;
        }


        // начальные настройки страниц для печати
        private void StartSettingsForPrint()
        {
            visibleLines = 1169 / richTextBox.Font.Height - 4;  // сколько строк помещается у RichTextBox с размером листа A4
            totalPage = richTextBox.Lines.Length / visibleLines + 1;  // узнаём кол-во страниц
            from = 0;
            pageNow = 1;
        }

        // печать документа
        private void PrintFile_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            printDialog1.AllowSomePages = true;  // будет ли показана опция выбора конкретных страниц для печати
            printDialog1.ShowHelp = true;
            printDialog1.Document = printDoc;

            printDoc.PrintPage += PrintDocument_PrintPage;  // вызывается для каждой страницы, которую необходимо напечатать
            StartSettingsForPrint();  // начальные настройки для печати

            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        // предварительный просмотр страниц для печати
        private void PrintPreview_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            PrintPreviewDialog preview = new PrintPreviewDialog();

            printDoc.PrintPage += PrintDocument_PrintPage;  // вызывается для каждой страницы, которую необходимо напечатать
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);  // устанавливаем размер страницы для А4

            preview.ClientSize = new Size(richTextBox.Size.Width, richTextBox.Size.Height);  // размер окна пред. печати равен размеру окна редактора
            preview.Document = printDoc;

            StartSettingsForPrint();  // начальные настройки для печати

            if (preview.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        // отрисовка страниц
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // определение шрифта и размера текста
            Font font = new Font(new FontFamily(richTextBox.SelectionFont.Name), richTextBox.SelectionFont.Size, FontStyle.Regular);

            string[] lines = richTextBox.Lines;  // список всех строк в RichTextBox
            StringBuilder str = new StringBuilder();  // буфер

            if (pageNow == totalPage) to = lines.Length;  // если это последняя страница, то всю строку записываем
            else to = pageNow * visibleLines;  // иначе, записываем столько, сколько помещается

            for (int j = from; j < to; j++)
            {
                str.Append(lines[j] + "\n");  // добавляем каждую строку в буфер
            }
            e.Graphics.DrawString(str.ToString(), font, Brushes.Black, 0, 0);  // записываем текст для печати

            if (pageNow == totalPage) e.HasMorePages = false;  // если это последняя страница, то все страницы были напечатаны
            else
            {
                pageNow++;
                from = to;
                e.HasMorePages = true;  // иначе, переходим к новой странице
            }

            // система будет автоматически вызывать событие PrintPage до тех пор, пока e.HasMorePages
            // не будет установлено в false, указывая, что все страницы были напечатаны
        }


        // измение цвета
        private void ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = false;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = richTextBox.SelectionColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
                richTextBox.SelectionColor = colorDialog1.Color;
        }

        // настройки для параметров страницы перед печатью
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // инициализируйте свойство PrinterSettings диалогового окна для хранения настроек принтера, заданных пользователем
            pageSetupDialog1.PageSettings = new PageSettings();

            // инициализировать свойство PrinterSettings диалогового окна для хранения настроек принтера, установленных пользователем
            pageSetupDialog1.PrinterSettings = new PrinterSettings();

            // если результат в порядке, отобразите выбранные настройки в ListBox. Эти значения могут быть использованы при печати документа
            if (pageSetupDialog1.ShowDialog() == DialogResult.OK)
            {
                object[] results = new object[]
                                       {
                                           pageSetupDialog1.PageSettings.Margins,
                                           pageSetupDialog1.PageSettings.PaperSize,
                                           pageSetupDialog1.PageSettings.Landscape,
                                           pageSetupDialog1.PrinterSettings.PrinterName,
                                           pageSetupDialog1.PrinterSettings.PrintRange
                                       };
            }
        }

        // форматирование шрифтов
        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                // 1. размер текста
                string size = (Math.Round(fontDialog1.Font.Size)).ToString();
                if (!ComboBox2.Items.Contains(size))  // если такого размера в списке нет
                {
                    int i;
                    for (i = 0; i < ComboBox2.Items.Count; i++)
                    {
                        if (int.Parse(size) < int.Parse(ComboBox2.Items[i].ToString())) break;
                    }
                    ComboBox2.Items.Insert(i, size);  // добавляем в определённое место (для отсортировки)
                }
                ComboBox2.SelectedItem = size;

                // 2. стиль шрифта
                string font = fontDialog1.Font.Name;
                if (!ComboBox1.Items.Contains(font))  // если такого шрифта в списке нет
                {
                    ComboBox1.Items.Add(font);  // добавляем
                }
                ComboBox1.SelectedItem = font;
            }
        }


        // help
        private void HelpPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            helpProvider1.HelpNamespace = "test.chm";
            Help.ShowHelp(this, helpProvider1.HelpNamespace, HelpNavigator.TableOfContents);
        }

        // web-help
        private void WebHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/WordPad");
        }


        // обработка "жирный", "курсив" и "подчеркнутый"
        private void BoldStyleButton_Click(object sender, EventArgs e)
        {
            if (!bold)  // если вкл жирный шрифт
            {
                if (italic && !underlined) fontStile = FontStyle.Bold | FontStyle.Italic;
                else if (!italic && underlined) fontStile = FontStyle.Bold | FontStyle.Underline;
                else if (italic && underlined) fontStile = FontStyle.Bold | FontStyle.Italic | FontStyle.Underline;
                else fontStile = FontStyle.Bold;

                BoldStyleButton.BackColor = Color.DimGray;
            }
            else  // если выкл жирный шрифт
            {
                if (italic && !underlined) fontStile = FontStyle.Italic;
                else if (!italic && underlined) fontStile = FontStyle.Underline;
                else if (italic && underlined) fontStile = FontStyle.Italic | FontStyle.Underline;
                else fontStile = FontStyle.Regular;

                BoldStyleButton.BackColor = Color.FromArgb(240, 240, 240);
            }
            bold = !bold;
            richTextBox.SelectionFont = new Font(fontFamily, size, fontStile);
        }

        private void ItalicStyleButton_Click(object sender, EventArgs e)
        {
            if (!italic)
            {
                if (bold && !underlined) fontStile = FontStyle.Bold | FontStyle.Italic;
                else if (!bold && underlined) fontStile = FontStyle.Underline | FontStyle.Italic;
                else if (bold && underlined) fontStile = FontStyle.Bold | FontStyle.Italic | FontStyle.Underline;
                else fontStile = FontStyle.Italic;

                ItalicStyleButton.BackColor = Color.DimGray;
            }
            else
            {
                if (bold && !underlined) fontStile = FontStyle.Bold;
                else if (!bold && underlined) fontStile = FontStyle.Underline;
                else if (bold && underlined) fontStile = FontStyle.Bold | FontStyle.Underline;
                else fontStile = FontStyle.Regular;

                ItalicStyleButton.BackColor = Color.FromArgb(240, 240, 240);
            }
            italic = !italic;
            richTextBox.SelectionFont = new Font(fontFamily, size, fontStile);
        }

        private void UnderlinedStyleButton_Click(object sender, EventArgs e)
        {
            if (!underlined)
            {
                if (bold && !italic) fontStile = FontStyle.Bold | FontStyle.Underline;
                else if (!bold && italic) fontStile = FontStyle.Italic | FontStyle.Underline;
                else if (bold && italic) fontStile = FontStyle.Bold | FontStyle.Italic | FontStyle.Underline;
                else fontStile = FontStyle.Underline;

                UnderlinedStyleButton.BackColor = Color.DimGray;
            }
            else
            {
                if (bold && !italic) fontStile = FontStyle.Bold;
                else if (!bold && italic) fontStile = FontStyle.Italic;
                else if (bold && italic) fontStile = FontStyle.Bold | FontStyle.Italic;
                else fontStile = FontStyle.Regular;

                UnderlinedStyleButton.BackColor = Color.FromArgb(240, 240, 240);
            }
            underlined = !underlined;
            richTextBox.SelectionFont = new Font(fontFamily, size, fontStile);
        }


        // обработка событий в комбобоксах
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            size = int.Parse(ComboBox2.Text);
            richTextBox.SelectionFont = new Font(fontFamily, size, fontStile);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fontFamily = ComboBox1.Text;
            richTextBox.SelectionFont = new Font(fontFamily, size, fontStile);
        }


        // статистика текста в статус бар
        private static int EM_LINEINDEX = 0xbb;
        [DllImport("user32.dll")]
        extern static int SendMessage(IntPtr hwnd, int message, int wparam, int lparam);

        private void UpdateTextInfo()
        {
            index = richTextBox.SelectionStart;  // позиция начала выделенного текста
            line = richTextBox.GetLineFromCharIndex(index);  // получаем номер строки
            col = index - SendMessage(richTextBox.Handle, EM_LINEINDEX, -1, 0);  // номер столбца
            toolStripStatusLabel1.Text = "Строка - " + (line + 1) + " Столбец - " + col + " Всего символов: " + richTextBox.Text.Length;
        }


        // загрузка изображения
        private void ToolStripButtonLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
            ofd.Filter = "jpg files|*.jpg";
            ofd.FilterIndex = 1;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                sPathToPicture = ofd.FileName;
                Clipboard.SetImage(Image.FromFile(sPathToPicture));  // используется для помещения изображения в буфер обмена ос

                richTextBox.SelectionStart = 0;
                richTextBox.Paste();  // достаём из буфера данные

                Clipboard.Clear();  // очищаем буфер
            }
        }


        // поиск строки в тексте
        private void ToolStripButtonSearch_Click_1(object sender, EventArgs e)
        {
            Search(true);
        }

        private void Search(bool flag)
        {
            if (flag)  // если это первый запуск поиска
            {
                iNextSearchPoint = 0;
                if (sf.ShowDialog() != DialogResult.OK) return;
            }

            if (richTextBox.Text.IndexOf(sf.GetSearchString()) == -1)
            {
                MessageBox.Show("Такого нет!");
            }
            else  // если строка есть в тексте
            {
                try
                {
                    // наводим курсор на начало строки которую ищем
                    richTextBox.SelectionStart = richTextBox.Text.IndexOf(sf.GetSearchString(), iNextSearchPoint);
                }
                catch (SystemException ex)
                {
                    if (ex.Data.ToString() == "System.Collections.ListDictionaryInternal")
                    {
                        MessageBox.Show("Достигнут конец поиска");
                    }
                    else
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                richTextBox.SelectionLength = sf.GetSearchString().Length;  // выделяем найденную строку
                iNextSearchPoint = richTextBox.SelectionStart + sf.GetSearchString().Length + 1;  // устанавливаем 'указатель' на следующей символ после найденной строки
            }
        }


        // замена строки
        private void ToolStripButtonReplace_Click_1(object sender, EventArgs e)
        {
            Replace();
        }

        private bool Search(string str)
        {
            if (richTextBox.Text.IndexOf(str) != -1)
            {
                richTextBox.SelectionStart = richTextBox.Text.IndexOf(str);
                richTextBox.SelectionLength = str.Length;
                iNextSearchPoint = (richTextBox.SelectionStart + str.Length + 1);
                return true;
            }
            return false;
        }

        private void Replace()
        {
            if (rf.ShowDialog() == DialogResult.OK)
            {
                if (rf.GetOldString().Length <= 0)
                {
                    MessageBox.Show("Введите строку что менять!");
                }
                else if (rf.GetNewString().Length <= 0)
                {
                    MessageBox.Show("Введите строку на что менять!");
                }
                else if (!Search(rf.GetOldString()))
                {
                    MessageBox.Show("Нет тут такого!");
                }
                else
                {
                    bool flag;
                    do
                    {
                        flag = Search(rf.GetOldString());
                        if (flag)
                        {
                            Clipboard.SetText(rf.GetNewString());
                            richTextBox.Paste();
                            iReplacesCount++;
                        }
                    } while (flag && rf.GetReplaceAll());
                    MessageBox.Show($"Всего замен: {iReplacesCount}");
                }
            }
        }


        // закрытие приложения
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isChanged)  // если изменения есть
            {
                var answer = MessageBox.Show("Сохранить все изменения?", "Message", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (answer == DialogResult.Yes)
                {
                    SaveFileToolStripMenuItem_Click(sender, e);  // вызов обработчика для сохранения файла
                }
                else if (answer == DialogResult.Cancel)
                {
                    e.Cancel = true;  // отмена события
                }
            }
        }


        // вставка даты и времени
        private void ToolStripButtonDate_Click_1(object sender, EventArgs e)
        {
            string date = (DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day : DateTime.Now.Day + "";
            string month = (DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month : DateTime.Now.Month + "";
            richTextBox.Text += $"{date}/{month}/{DateTime.Now.Year} {DateTime.Now.TimeOfDay}";
        }


        // экстрактор pdf файлов
        private void ToolStripButtonPDF_Click(object sender, EventArgs e)
        {
            //ofdParserPDF = new OpenFileDialog();
            ofdParserPDF.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            ofdParserPDF.Filter = "pdf files|*.pdf";
            ofdParserPDF.FilterIndex = 1;

            if (ofdParserPDF.ShowDialog() == DialogResult.OK)
            {
                backgroundWorker1.RunWorkerAsync();  // запускаем другой поток
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ParsePdf(ofdParserPDF.FileName);
        }

        public void ParsePdf(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException(fileName);

            PdfReader reader = new PdfReader(fileName);
            StringBuilder sb = new StringBuilder();

            for (int page = 0; page < reader.NumberOfPages; page++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                sb.Append(PdfTextExtractor.GetTextFromPage(reader, page + 1, strategy) + "\n\n");
            }

            reader.Close();
            richTextBox.Text = sb.ToString();
        }


        // события для RichTextBox
        private void RichTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void RichTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F) Search(true);
            else if (e.KeyCode == Keys.F3) Search(false);
            else if (e.Control && e.KeyCode == Keys.R) Replace();

            UpdateTextInfo();
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateTextInfo();

            if (!isChanged && sEtalon != richTextBox.Text)  // если строка изменилась, то меняем заголовок у окна (добавляем *)
            {
                isChanged = true;
                UpdateFormText();
            }
        }
    }
}
