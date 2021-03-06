// GutterMargin.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (c) 2007 Novell, Inc (http://www.novell.com)
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
//
//

using System;
using Gtk;
using Gdk;

namespace Mono.TextEditor
{
	public class GutterMargin : Margin
	{
		TextEditor editor;
		int width;
		int oldLineCountLog10 = -1;
		
		public GutterMargin (TextEditor editor)
		{
			this.editor = editor;
			
			base.cursor = new Gdk.Cursor (Gdk.CursorType.RightPtr);
			this.editor.Document.LineChanged += UpdateWidth;
			this.editor.Document.TextSet += HandleEditorDocumenthandleTextSet;
			this.editor.Caret.PositionChanged += EditorCarethandlePositionChanged;
		}

		void HandleEditorDocumenthandleTextSet (object sender, EventArgs e)
		{
			UpdateWidth (null, null);
		}

		void EditorCarethandlePositionChanged (object sender, DocumentLocationEventArgs e)
		{
			if (e.Location.Line == editor.Caret.Line)
				return;
			editor.RedrawMarginLine (this, e.Location.Line);
			editor.RedrawMarginLine (this, editor.Caret.Line);
		}

		void CalculateWidth ()
		{
			using (var layout = PangoUtil.CreateLayout (editor)) {
				layout.FontDescription = editor.Options.Font;
				layout.SetText (editor.Document.LineCount.ToString ());
				layout.Alignment = Pango.Alignment.Left;
				layout.Width = -1;
				int height;
				layout.GetPixelSize (out this.width, out height);
				this.width += 4;
			}
		}
		
		void UpdateWidth (object sender, LineEventArgs args)
		{
			int currentLineCountLog10 = (int)System.Math.Log10 (editor.Document.LineCount);
			if (oldLineCountLog10 != currentLineCountLog10) {
				CalculateWidth ();
				oldLineCountLog10 = currentLineCountLog10;
				editor.Document.CommitUpdateAll ();
			}
		}
		
		public override double Width {
			get {
				return width;
			}
		}
		
		DocumentLocation anchorLocation = new DocumentLocation (DocumentLocation.MinLine, DocumentLocation.MinColumn);
		internal protected override void MousePressed (MarginMouseEventArgs args)
		{
			base.MousePressed (args);
			
			if (args.Button != 1 || args.LineNumber < DocumentLocation.MinLine)
				return;
			editor.LockedMargin = this;
			int lineNumber       = args.LineNumber;
			bool extendSelection = (args.ModifierState & Gdk.ModifierType.ShiftMask) == Gdk.ModifierType.ShiftMask;
			if (lineNumber <= editor.Document.LineCount) {
				DocumentLocation loc = new DocumentLocation (lineNumber, DocumentLocation.MinColumn);
				LineSegment line = args.LineSegment;
				if (args.Type == EventType.TwoButtonPress) {
					if (line != null)
						editor.MainSelection = new Selection (loc, GetLineEndLocation (editor.GetTextEditorData (), lineNumber));
				} else if (extendSelection) {
					if (!editor.IsSomethingSelected) {
						editor.MainSelection = new Selection (loc, loc);
					} 
					editor.MainSelection.Lead   = loc;
				} else {
					anchorLocation = loc;
					editor.ClearSelection ();
				}
				editor.Caret.PreserveSelection = true;
				editor.Caret.Location = loc;
				editor.Caret.PreserveSelection = false;
			}
		}
		
		internal protected override void MouseReleased (MarginMouseEventArgs args)
		{
			editor.LockedMargin = null;
			base.MouseReleased (args);
		}
		
		public static DocumentLocation GetLineEndLocation (TextEditorData data, int lineNumber)
		{
			LineSegment line = data.Document.GetLine (lineNumber);
			
			DocumentLocation result = new DocumentLocation (lineNumber, line.EditableLength + 1);
			
			FoldSegment segment = null;
			foreach (FoldSegment folding in data.Document.GetStartFoldings (line)) {
				if (folding.IsFolded && folding.Contains (data.Document.LocationToOffset (result))) {
					segment = folding;
					break;
				}
			}
			if (segment != null) 
				result = data.Document.OffsetToLocation (segment.EndLine.Offset + segment.EndColumn - 1); 
			return result;
		}
		
		internal protected override void MouseHover (MarginMouseEventArgs args)
		{
			base.MouseHover (args);
			
			if (args.Button == 1) {
			//	DocumentLocation loc = editor.Document.LogicalToVisualLocation (editor.GetTextEditorData (), editor.Caret.Location);
				
				int lineNumber = args.LineNumber >= DocumentLocation.MinLine ? args.LineNumber : editor.Document.LineCount;
				editor.Caret.PreserveSelection = true;
				editor.Caret.Location = new DocumentLocation (lineNumber, DocumentLocation.MinColumn);
				editor.MainSelection = new Selection (anchorLocation, editor.Caret.Location);
				editor.Caret.PreserveSelection = false;
			}
		}
		
		public override void Dispose ()
		{
			if (base.cursor == null)
				return;
			
			base.cursor.Dispose ();
			base.cursor = null;
			
			this.editor.Document.TextSet -= HandleEditorDocumenthandleTextSet;
			this.editor.Document.LineChanged -= UpdateWidth;
//			layout = layout.Kill ();
			base.Dispose ();
		}
		
		Cairo.Color lineNumberBgGC, lineNumberGC, lineNumberHighlightGC;
		internal protected override void OptionsChanged ()
		{
			CalculateWidth ();
			
			lineNumberBgGC = editor.ColorStyle.LineNumber.CairoBackgroundColor;
			lineNumberGC = editor.ColorStyle.LineNumber.CairoColor;
			lineNumberHighlightGC = editor.ColorStyle.LineNumberFgHighlighted;
		}
		
		internal protected override void Draw (Cairo.Context cr, Cairo.Rectangle area, LineSegment lineSegment, int line, double x, double y, double lineHeight)
		{
			cr.Rectangle (x, y, Width, lineHeight);
			cr.Color = lineNumberBgGC;
			cr.Fill ();
			
			if (line <= editor.Document.LineCount) {
				// Due to a mac? gtk bug I need to re-create the layout here
				// otherwise I get pango exceptions.
				using (var layout = PangoUtil.CreateLayout (editor)) {
					layout.FontDescription = editor.Options.Font;
					layout.Width = (int)Width;
					layout.Alignment = Pango.Alignment.Right;
					layout.SetText (line.ToString ());
					cr.Save ();
					cr.Translate (x + (int)Width, y);
					cr.Color = editor.Caret.Line == line ? lineNumberHighlightGC : lineNumberGC;
					cr.ShowLayout (layout);
					cr.Restore ();
				}
			}
		}
	}
}
