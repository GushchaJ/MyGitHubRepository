using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Main_Form : Form
    {
        public Main_Form()
        {
            InitializeComponent();
        }


        Stack<string> undoActions = new Stack<string>();
        Stack<string> redoActions = new Stack<string>();
        string path = "";
        string filename = "";
        int change = 1;

        //TODO
        //уберите двойной показ окон диалога. например, в момент, когда в редакторе есть текст, то вызов команды Open приводит к тому, что
        // дважды появляется окно диалога с запросом на сохранение текущего документа (?..)

        private DialogResult Proverka() // Запрос на сохранение изменений в документе
        {
           
            DialogResult result = DialogResult.Ignore;

            // TODO: в данной проверке необходимо проверять не только на пустоту, 
            // но и 
            // - на то, что текст состоит лишь из одного и более пробелов (изучите метод Trim() для строковых данных)
            // - на то, редактировался текст или не редактировался (изучите соостветствующие св-ва элемента RichTextBox)

            if (richTextBox1.Text != "")
            {
                result = MessageBox.Show(
                    "Вы хотите сохранить изменения в Вашем документе?",
                    "Блокнот",
                    MessageBoxButtons.YesNoCancel);
            }
            return result;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) // Создание нового файла
        {
            DialogResult result = Proverka();
                if (result == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.No)
                {
                    this.richTextBox1.Clear(); //Удаление текста из элем. упр. текстовым полем
                }
        }


        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e) // Закрытие формы по нажатию на крестик
        {
            if (change != 0)
            {
                //TODO учтите также ответ пользователя "Нет"
                DialogResult result = Proverka();
                if (result == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        // Открытие файла
        private void openToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            path = openFileDialog1.FileName;
            filename = Path.GetFileNameWithoutExtension(path);
            // читаем файл в строку
            string fileText = File.ReadAllText(path, Encoding.Default);
            richTextBox1.Text = fileText;

            this.Text = filename;
        }


        // Сохранение с созданием нового файла или перезаписью существующего
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            change = 0;
            // получаем выбранный файл
            path = saveFileDialog1.FileName;
            filename = Path.GetFileNameWithoutExtension(path);
            // сохраняем текст в файл
            File.WriteAllText(path, richTextBox1.Text);

            this.Text = filename;
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e) // Сохранение изменений в текущий документ
        {
            if (path != "")
            {
                File.WriteAllText(path, richTextBox1.Text);
                change = 0;
            }
            else
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e) // Закрытие формы по нажатию на кнопку в меню
        {
            DialogResult result = Proverka();
            if (result == DialogResult.Yes)
            {
                saveToolStripMenuItem_Click(sender, e);

                this.Close();
            }
            else if (result == DialogResult.Cancel)
            {

            }
            else
            {
                this.Close();
            }
        }



        private void richTextBox1_KeyDown(object sender, KeyEventArgs e) 
        {
            undoActions.Push(richTextBox1.Text); // Запись в стек предыдущей версии текста

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) // Отмена последнего действия
        {
            if (undoActions.Count < 1)
            {
                return;
            }

            if (redoActions.Count == 0 || richTextBox1.Text != "")
            {
                redoActions.Push(richTextBox1.Text);
            }

            richTextBox1.Text = undoActions.Pop();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) // Повторение последнего действия
        {
            if (redoActions.Count < 1)
            {
                return;
            }

            if (undoActions.Count == 0 || richTextBox1.Text != "")
            {
                undoActions.Push(richTextBox1.Text);
            }

            richTextBox1.Text = redoActions.Pop();

        }

        //TODO доработайте - undo|redo не выполняется должным образом, если копировался и вставлялся текст. 
        // также проверьте, чтобы при форматировании тоже корректно выполнялись команды undo|redo
        private void richTextBox1_TextChanged(object sender, EventArgs e) // Проверка на возможность выполнения действия Undo/Redo
        {
            change = 1;

            if (undoActions.Count == 0)
                undoToolStripMenuItem.Enabled = false;
            else
            {
                undoToolStripMenuItem.Enabled = true;
            }

            if (redoActions.Count == 0)
                redoToolStripMenuItem.Enabled = false;
            else
            {
                redoToolStripMenuItem.Enabled = true;
            }
        }


        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut(); //Вырезка
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy(); // Копирование
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste(); //Вставка
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionFont = fontDialog1.Font;
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e) // Изменение цвета текста
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionColor = colorDialog1.Color;
            }
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e) // Изменение выравнивания текста
        {
            //TODO хорошо, что вы реализовали один обработчик события для идентичных кнопок меню,
            // НО! это дало мало эффекта. Храните нужное значение HorizontalAlignment.(...) в св-ве Tag для каждой кнопки 
            // и используйте напрямую эту информацию 
            //richTextBox1.SelectionAlignment = (HorizontalAlignment)(sender as ToolStripMenuItem).Tag

            string btn = (sender as ToolStripMenuItem).Text;

            switch (btn)
            {
                case "Left":
                    richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
                    break;
                case "Right":
                    richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
                    break;
                case "Center":
                    richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
                    break;
            }
        }

        //TODO почистите код от лишних методов
        /*
         *по-моему, следующих два метода уже не актуальны?
         * 
         * 
        private void rightToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            //leftToolStripMenuItem_Click(sender, e);
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //leftToolStripMenuItem_Click(sender, e);
        }
        */

        private void toolStripButton1_Click(object sender, EventArgs e) // Изменение стиля текста
        {
            Font currentFont = richTextBox1.SelectionFont;
            FontStyle newFontStyle = FontStyle.Regular;
            
            if (toolStripButton1.Checked == true) newFontStyle |= FontStyle.Bold;
            if (toolStripButton2.Checked == true) newFontStyle |= FontStyle.Italic; ;
            if (toolStripButton3.Checked == true) newFontStyle |= FontStyle.Underline;
            if (toolStripButton4.Checked == true) newFontStyle |= FontStyle.Strikeout;

            richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
        }

        //TODO почистите код от лишних методов
        /*
         *по-моему, следующие методы также не актуальны?
         * 
         * Внимание! Могут возникнуть ошибки компиляции, т.к. удаление этих методов сопряжено с удалением некоторых строк из файла Main_Form.Designer.cs, поэтому могут возни
         * Надеюсь, разберетесь. (Если не разберетесь - пишите/спрашивайте)
         * 
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toolStripButton1_Click(sender, e); 
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toolStripButton1_Click(sender, e);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            toolStripButton1_Click(sender, e);
        }

        private void создатьToolStripButton_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(sender,e);
        }

        private void открытьToolStripButton_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender,e);
        }

        private void сохранитьToolStripButton_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender,e);
        }

        private void вырезатьToolStripButton_Click(object sender, EventArgs e)
        {
            cutToolStripMenuItem_Click(sender,e);
        }

        private void копироватьToolStripButton_Click(object sender, EventArgs e)
        {
            copyToolStripMenuItem_Click(sender,e);
        }

        private void вставкаToolStripButton_Click(object sender, EventArgs e)
        {
            pasteToolStripMenuItem_Click(sender,e);
        }
        */

        private void richTextBox1_Click(object sender, EventArgs e) // Определение состояния кнопок изменения стиля 
        {
            Font currentFont = richTextBox1.SelectionFont;

            if (currentFont.Bold) toolStripButton1.Checked = true; else toolStripButton1.Checked = false;
            if (currentFont.Italic) toolStripButton2.Checked = true; else toolStripButton2.Checked = false;
            if (currentFont.Underline) toolStripButton3.Checked = true; else toolStripButton3.Checked = false;
            if (currentFont.Strikeout) toolStripButton4.Checked = true; else toolStripButton4.Checked = false;
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {

        }
    }
}
