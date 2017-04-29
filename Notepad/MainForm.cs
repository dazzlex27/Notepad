using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Notepad
{
	public partial class MainForm : Form
	{
		private const string DefaultFileName = "Untitled";

		private Stack<string> _textChangeStack;
		private FileManager _fileManager;

		private bool _changed;
		private string _currentPath;
		private string _currentText;
		private string _currentFileName;

		protected string CurrentText
		{
			get { return _currentText; }
			set
			{
				_currentText = value;
				SetTextBoxText(value);
			}
		}

		protected string CurrentPath
		{
			get { return _currentPath; }
			set
			{
				_currentPath = value;
				CurrentFileName = GetFileNameFromPath(value);
			}
		}

		protected string CurrentFileName
		{
			get { return _currentFileName; }
			set
			{
				_currentFileName = value;
				Text = value + " - Notepad";
			}
		}

		public MainForm()
		{
			InitializeComponent();
			_textChangeStack = new Stack<string>();
			ClearStack();
			_fileManager = new FileManager();
			CurrentFileName = DefaultFileName;
			textBox1.WordWrap = false;
		}

		private bool PerformAction(Action method)
		{
			if (!_changed)
			{
				method();
				return true;
			}
			else
			{
				switch (PromptToSaveChanges())
				{
					case DialogResult.Yes:
						if (SaveTextToFile(_currentPath))
							method();
						return true;
					case DialogResult.No:
						method();
						return true;
					default:
						return false;
				}
			}
		}

		private void CreateNewFile()
		{
			CurrentText = "";
			_currentPath = "";
		}

		private void OpenTextFromFile()
		{
			if (_ofd.ShowDialog() == DialogResult.OK)
			{
				CurrentPath = _ofd.FileName;
				CurrentText = _fileManager.OpenTextFromFile(CurrentPath);
				_changed = false;
			}
		}

		private bool SaveTextToFile(string fullPath)
		{
			if (fullPath == "")
			{
				if (_sfd.ShowDialog() == DialogResult.OK)
				{
					CurrentPath = _sfd.FileName;
				}
				else
					return false;
			}

			_fileManager.SaveTextToFile(CurrentPath, CurrentText);
			_changed = false;
			return true;
		}

		private void ExitApp()
		{
			Dispose();
			Environment.Exit(0);
		}

		private void ClearStack()
		{
			_textChangeStack.Clear();
			_textChangeStack.Push(_currentText);
		}

		private void SetTextBoxText(string text)
		{
			textBox1.Text = text;
		}

		private string GetFileNameFromPath(string path)
		{
			int index = path.LastIndexOf("\\") + 1;

			return path.Substring(index, path.Length - index);
		}

		private DialogResult PromptToSaveChanges()
		{
			return GetAnswerFromMessageBox($"Do you want to save changes to {_currentFileName}?", "Unsaved changes");
		}

		private DialogResult GetAnswerFromMessageBox(string text, string caption)
		{
			return MessageBox.Show(
				text,
				caption,
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question);
		}

		private void TextBox1_TextChanged(object sender, EventArgs e)
		{
			var temp = sender as TextBox;
			_currentText = temp.Text;
			_changed = true;
			_textChangeStack.Push(_currentText);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!PerformAction(ExitApp))
				e.Cancel = true;
		}

		private void NewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (PerformAction(CreateNewFile))
			{
				CurrentFileName = DefaultFileName;
				_changed = false;
				ClearStack();
			}
		}

		private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformAction(OpenTextFromFile);
			ClearStack();
		}

		private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveTextToFile(_currentPath);
		}

		private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_sfd.ShowDialog() == DialogResult.OK)
			{
				CurrentPath = _sfd.FileName;
				_changed = false;
			}
		}

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformAction(ExitApp);
		}

		private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_textChangeStack.Count > 1)
			{
				_textChangeStack.Pop();
				CurrentText = _textChangeStack.Pop();
			}
		}

		private void WordWrapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textBox1.WordWrap = !textBox1.WordWrap;
			wordWrapToolStripMenuItem.Checked = textBox1.WordWrap;
		}

		private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textBox1.SelectAll();
		}
	}
}
