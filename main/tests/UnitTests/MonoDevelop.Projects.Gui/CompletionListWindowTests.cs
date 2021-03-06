// 
// CompletionListWindowTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using NUnit.Framework;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom.Parser;

namespace MonoDevelop.Projects.Gui
{
	[TestFixture()]
	public class CompletionListWindowTests
	{
		class TestCompletionWidget : ICompletionWidget 
		{
			public string CompletedWord {
				get;
				set;
			}
			#region ICompletionWidget implementation
			public event EventHandler CompletionContextChanged {
				add { /* TODO */ }
				remove { /* TODO */ }
			}
			
			public string GetText (int startOffset, int endOffset)
			{
				return "";
			}
			
			public char GetChar (int offset)
			{
				return '\0';
			}
			
			public CodeCompletionContext CreateCodeCompletionContext (int triggerOffset)
			{
				return null;
			}
			public CodeCompletionContext CurrentCodeCompletionContext {
				get {
					return null;
				}
			}
			public string GetCompletionText (CodeCompletionContext ctx)
			{
				return "";
			}
			
			public void SetCompletionText (CodeCompletionContext ctx, string partial_word, string complete_word)
			{
				this.CompletedWord = complete_word;
			}
			
			public void SetCompletionText (CodeCompletionContext ctx, string partial_word, string complete_word, int offset)
			{
				this.CompletedWord = complete_word;
			}
			
			public void Replace (int offset, int count, string text)
			{
			}
			public int TextLength {
				get {
					return 0;
				}
			}
			
			public int SelectedLength {
				get {
					return 0;
				}
			}
			
			public Gtk.Style GtkStyle {
				get {
					return null;
				}
			}
			#endregion
			
		}
		
		static void SimulateInput (CompletionListWindow listWindow, string input)
		{
			foreach (char ch in input) {
				KeyActions ka;
				switch (ch) {
				case '8':
					listWindow.PreProcessKeyEvent (Gdk.Key.Up, '\0', Gdk.ModifierType.None, out ka);
					break;
				case '2':
					listWindow.PreProcessKeyEvent (Gdk.Key.Down, '\0', Gdk.ModifierType.None, out ka);
					break;
				case '4':
					listWindow.PreProcessKeyEvent (Gdk.Key.Left, '\0', Gdk.ModifierType.None, out ka);
					break;
				case '6':
					listWindow.PreProcessKeyEvent (Gdk.Key.Right, '\0', Gdk.ModifierType.None, out ka);
					break;
				case '\t':
					listWindow.PreProcessKeyEvent (Gdk.Key.Tab, '\t', Gdk.ModifierType.None, out ka);
					break;
				case '\b':
					listWindow.PreProcessKeyEvent (Gdk.Key.BackSpace, '\b', Gdk.ModifierType.None, out ka);
					break;
				case '\n':
					listWindow.PreProcessKeyEvent (Gdk.Key.Return, '\n', Gdk.ModifierType.None, out ka);
					break;
				default:
					listWindow.PreProcessKeyEvent ((Gdk.Key)ch, ch, Gdk.ModifierType.None, out ka);
					break;
				}
			}
		}
		
		class SimulationSettings {
			public string PartialWord { get; set; }
			public string SimulatedInput { get; set; }
			public bool AutoSelect { get; set; }
			public bool CompleteWithSpaceOrPunctuation { get; set; }
			public bool AutoCompleteEmptyMatch { get; set; }
			public string DefaultCompletionString { get; set; }
			
			public string[] CompletionData { get; set; }
		}
		
		static string RunSimulation (string partialWord, string simulatedInput, bool autoSelect, bool completeWithSpaceOrPunctuation, params string[] completionData)
		{
			return RunSimulation (partialWord, simulatedInput, autoSelect, completeWithSpaceOrPunctuation, true, completionData);
		}
		
		static string RunSimulation (string partialWord, string simulatedInput, bool autoSelect, bool completeWithSpaceOrPunctuation, bool autoCompleteEmptyMatch, params string[] completionData)
		{
			return RunSimulation (new SimulationSettings () {
				PartialWord = partialWord,
				SimulatedInput = simulatedInput,
				AutoSelect = autoSelect,
				CompleteWithSpaceOrPunctuation = completeWithSpaceOrPunctuation,
				AutoCompleteEmptyMatch = autoCompleteEmptyMatch,
				CompletionData = completionData
			});
		}
		
		static string RunSimulation (SimulationSettings settings)
		{
			CompletionListWindow listWindow = CreateListWindow (settings);
			SimulateInput (listWindow, settings.SimulatedInput);
			return ((TestCompletionWidget)listWindow.CompletionWidget).CompletedWord;
		}

		static CompletionListWindow CreateListWindow (CompletionListWindowTests.SimulationSettings settings)
		{
			CompletionDataList dataList = new CompletionDataList ();
			dataList.AutoSelect = settings.AutoSelect;
			dataList.AddRange (settings.CompletionData);
			dataList.DefaultCompletionString = settings.DefaultCompletionString;
			ListWindow.ClearHistory ();
			CompletionListWindow listWindow = new CompletionListWindow {
				CompletionDataList = dataList,
				CompletionWidget = new TestCompletionWidget (),
				AutoSelect = settings.AutoSelect,
				CodeCompletionContext = new CodeCompletionContext (),
				CompleteWithSpaceOrPunctuation = settings.CompleteWithSpaceOrPunctuation,
				AutoCompleteEmptyMatch = settings.AutoCompleteEmptyMatch,
				PartialWord = settings.PartialWord,
				DefaultCompletionString = settings.DefaultCompletionString
			};
			listWindow.UpdateWordSelection ();
			listWindow.ResetSizes ();
			return listWindow;
		}

		
		[Test()]
		public void TestPunctuationCompletion ()
		{
			string output = RunSimulation ("", "aaa ", true, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAb", output);
			
			output = RunSimulation ("", "aa.", true, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAb", output);
			
			output = RunSimulation ("", "AbAbAb.", true, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAb", output);
		}
		
		[Test()]
		public void TestPunctuationCompletionShouldNotComplete ()
		{
			string output = RunSimulation ("", "aaa ", true, false, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual (null, output);
		}
		
		[Test()]
		public void TestTabCompletion ()
		{
			string output = RunSimulation ("", "aaa\t", true, false, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAb", output);
		}
		
		[Test()]
		public void TestTabCompletionWithAutoSelectOff ()
		{
			string output = RunSimulation ("", "aaa\t", false, false, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAb", output);
		}
		
		[Test()]
		public void TestReturnCompletion ()
		{
			string output = RunSimulation ("", "aaa\n", true, false, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAb", output);
		}
		
		[Test()]
		public void TestReturnCompletionWithAutoSelectOff ()
		{
			string output = RunSimulation ("", "aaa\n", false, false, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual (null, output);
		}
		
		[Test()]
		public void TestAutoSelectionOn ()
		{
			// shouldn't select anything since auto select is disabled.
			string output = RunSimulation ("", "aaa ", true, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAb", output);
			
			// now with cursor down
			output = RunSimulation ("", "aaa2 ", true, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAbAb", output);
		}
		
		[Test()]
		public void TestAutoSelectionOff ()
		{
			// shouldn't select anything since auto select is disabled.
			string output = RunSimulation ("", "aaa ", false, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.IsNull (output);
			
			// now with cursor down (shouldn't change selection)
			output = RunSimulation ("", "aaa2 ", false, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAb", output);
			
			// now with 2x cursor down - shold select next item.
			output = RunSimulation ("", "aaa22 ", false, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb",
				"AbAbAbAbAb");
			
			Assert.AreEqual ("AbAbAbAb", output);
		}
		
		[Test()]
		public void TestBackspace ()
		{
			string output = RunSimulation ("", "aaaa\b\t", true, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAb", output);
			
			output = RunSimulation ("", "aaaa\b\b\b\t", true, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAb", output);
			
			output = RunSimulation ("", "aaaa\b\b\baaa\t", true, true, 
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAbAbAb", output);
		}
		
		[Test()]
		public void TestBackspacePreserveAutoSelect ()
		{
			string output = RunSimulation ("", "c\bc ", false, true, 
				"a",
				"b", 
				"c");
			
			Assert.AreEqual (null, output);
		}
		
		[Test()]
		public void TestAutoCompleteEmptyMatchOn ()
		{
			string output = RunSimulation ("", " ", true, true, true,
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAb", output);
			
			output = RunSimulation ("", "\t", true, true, true,
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAb", output);
			
		}
		
		[Test()]
		public void TestAutoCompleteFileNames ()
		{
			string output = RunSimulation ("", "Doc.cs ", true, true, true, "Document.cs");

			Assert.AreEqual ("Document.cs", output);
			
			output = RunSimulation ("", "cwid.cs ", true, true, true,
				"Test.txt",
				"CompletionWidget.cs", 
				"CommandWindow.cs");

			Assert.AreEqual ("CompletionWidget.cs", output);
		}
		
		[Test()]
		public void TestAutoCompleteEmptyMatchOff ()
		{
			string output = RunSimulation ("", " ", true, true, false,
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual (null, output);
			
			output = RunSimulation ("", "\t", true, true, false,
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAb", output);
			
			output = RunSimulation ("", "a ", true, true, false,
				"AbAb",
				"AbAbAb", 
				"AbAbAbAb");
			
			Assert.AreEqual ("AbAb", output);
			
		}
		
		string[] punctuationData = {
			"AbAb",
			"/AbAb", 
			"Accc",
			",AbAb",
			",A..bAb",
			",A.bAb",
			"Addd",
		};
		
		[Test]
		public void TestMatchPunctuation ()
		{
			string output = RunSimulation ("", "/\n", true, false, false, punctuationData);
			Assert.AreEqual ("/AbAb", output);
			
			output = RunSimulation ("", "A\n", true, false, false, punctuationData);
			Assert.AreEqual ("AbAb", output);
			
			output = RunSimulation ("", ",A..\n", true, false, false, punctuationData);
			Assert.AreEqual (",A..bAb", output);
		}
		
		[Test]
		public void TestMatchPunctuationCommitOnSpaceAndPunctuation ()
		{
			string output = RunSimulation ("", "Ac ", true, true, false, punctuationData);
			Assert.AreEqual ("Accc", output);
			
			output = RunSimulation ("", "/ ", true, true, false, punctuationData);
			Assert.AreEqual ("/AbAb", output);
			
			output = RunSimulation ("", ".", true, true, false, punctuationData);
			Assert.AreEqual (null, output);
			
			output = RunSimulation ("", "A ", true, true, false, punctuationData);
			Assert.AreEqual ("AbAb", output);
			
			output = RunSimulation ("", ",A.b ", true, true, false, punctuationData);
			Assert.AreEqual (",A.bAb", output);
		}
		
		[Test]
		public void TestDefaultCompletionString ()
		{
			string output = RunSimulation (new SimulationSettings {
				PartialWord = "",
				SimulatedInput = "\t",
				AutoSelect = true,
				CompleteWithSpaceOrPunctuation = true,
				AutoCompleteEmptyMatch = false,
				CompletionData = new[] {
					"A",
					"B",
					"C"
				},
				DefaultCompletionString = "C"
			});
			
			Assert.AreEqual ("C", output);
			
			output = RunSimulation (new SimulationSettings {
				PartialWord = "",
				SimulatedInput = " ",
				AutoSelect = true,
				CompleteWithSpaceOrPunctuation = true,
				AutoCompleteEmptyMatch = false,
				CompletionData = new[] {
					"A",
					"B",
					"C"
				},
				DefaultCompletionString = "C"
			});
			
			Assert.AreEqual ("C", output);
		}
		
		[Test]
		public void TestDefaultCompletionStringList ()
		{
			CompletionListWindow listWindow = CreateListWindow (new SimulationSettings {
				PartialWord = "",
				SimulatedInput = "\t",
				AutoSelect = true,
				CompleteWithSpaceOrPunctuation = true,
				AutoCompleteEmptyMatch = false,
				CompletionData = new[] {
					"A",
					"B",
					"C"
				},
				DefaultCompletionString = "C"
			});
			Assert.AreEqual (3, listWindow.FilteredItems.Count);
		}
		
		/// <summary>
		/// Bug 543923 - Completion window should deselect when word is deleted
		/// </summary>
		[Test]
		public void TestBug543923 ()
		{
			string output = RunSimulation (new SimulationSettings {
				PartialWord = "",
				SimulatedInput = "i\b ",
				AutoSelect = true,
				CompleteWithSpaceOrPunctuation = true,
				AutoCompleteEmptyMatch = false,
				CompletionData = new[] { "#if", "if", "other" }
			});
			Assert.IsTrue (string.IsNullOrEmpty (output));
		}
		
		
		/// <summary>
		/// Bug 543938 - Completion list up/down broken with single entry
		/// </summary>
		[Test]
		public void TestBug543938 ()
		{
			string output = RunSimulation ("", "2 ", true, true, false, "singleEntry");
			
			Assert.AreEqual ("singleEntry", output);
			
			output = RunSimulation ("", " ", true, true, false, "singleEntry");
			Assert.IsTrue (string.IsNullOrEmpty (output));
		}
		
		/// <summary>
		/// Bug 543984 – Completion window should only accept punctuation when it's an exact match
		/// </summary>
		[Test]
		public void TestBug543984 ()
		{
			string output = RunSimulation ("", "foo b\n", true, true, false, "foo bar", "foo bar baz");
			Assert.AreEqual ("foo bar", output);
		}
		
		[Test]
		public void TestBug595240 ()
		{
			string output = RunSimulation ("", "A\t", true, true, false, "AbCdEf");
			Assert.AreEqual ("AbCdEf", output);
			
			output = RunSimulation ("", "Cd\t", true, true, false, "AbCdEf");
			Assert.AreEqual ("AbCdEf", output);
			
			output = RunSimulation ("", "bC\t", true, true, false, "AbCdEf");
			Assert.AreNotEqual ("AbCdEf", output);
		}
		
		/// <summary>
		/// Bug 613539 - DOBa does not complete to DynamicObjectBase
		/// </summary>
		[Test]
		public void TestBug613539 ()
		{
			string output = RunSimulation ("", "DOB ", true, true, false, "DynamicObject", "DynamicObjectBase");
			Assert.AreEqual ("DynamicObjectBase", output);
		}
		
		/// <summary>
		/// Bug 629361 - Exact completion matches should take account of case
		/// </summary>
		[Test]
		public void TestBug629361 ()
		{
			string output = RunSimulation ("", "unit\t", true, true, false, "Unit", "unit");
			Assert.AreEqual ("unit", output);
		}
		
		/// <summary>
		/// Bug 668136 - Subword matching in completion does not work well for xml
		/// </summary>
		[Test]
		public void TestBug668136 ()
		{
			string output = RunSimulation ("", "bar\t", true, true, false, "foo:test", "foo:bar", "foo:foo");
			Assert.AreEqual ("foo:bar", output);
		}
		
		[Test]
		public void TestSubstringMatch ()
		{
			string output = RunSimulation ("", "comcoll\n", true, true, false, "CustomCommandCollection");
			Assert.AreEqual ("CustomCommandCollection", output);
			
			output = RunSimulation ("", "cuscoll\n", true, true, false, "CustomCommandCollection");
			Assert.AreEqual ("CustomCommandCollection", output);
			
			output = RunSimulation ("", "commandcollection\n", true, true, false, "CustomCommandCollection");
			Assert.AreEqual ("CustomCommandCollection", output);
		}
		
		[TestFixtureSetUp] 
		public void SetUp()
		{
			Gtk.Application.Init ();
		}
	}
}
