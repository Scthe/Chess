﻿using ChessUI.model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessUI {

	public partial class MainWindow : Window {

		private static SolidColorBrush defaultBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dcc4a3"));
		private static SolidColorBrush alternateBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9d7c3f"));
		private static SolidColorBrush hoverBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#caca93"));
		private static SolidColorBrush alternateHoverBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#948c3e"));
		private static SolidColorBrush orangeBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff8c3e"));
		private static SolidColorBrush redBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff4c00"));
		private static int INVALID_CELL = -1;

		private ImageCache _imgCache = new ImageCache();
		private int HoveredCell = INVALID_CELL;
		private int clickStart = INVALID_CELL;
		private Board _board = new Board();
		private int _selectedCell = INVALID_CELL;

		public MainWindow() {
			InitializeComponent();
			_imgCache.init();

			for (int i = 0; i < 64; i++) {
				Panel cell = new WrapPanel();
				cell.Background = getColor(i, false);
				cell.Tag = i;
				cell.IsMouseDirectlyOverChanged += onCellHover;
				cell.MouseLeftButtonDown += onCellDown;
				cell.MouseLeftButtonUp += onCellUp;
				ChessBoard.Children.Add(cell);
			}

			foreach (Chess.Fs.Pawn pawn in _board.Pieces) {
				drawPawn(pawn);
			}
			/*
			var a =_board.getMoves(null);
			foreach (Chess.Fs.Position p in a) {
				markPosition(p);
			}*/
		}

		// listeners
		private void onCellHover(object sender, DependencyPropertyChangedEventArgs e) {
			redrawBoardBackground();

			Panel cell = sender as Panel;
			int i = (int)cell.Tag;
			cell.Background = getColor(i, true);
			HoveredCell = i;

			// get hovered pawn
			var position = CellToPosition(i);
			var pawn = _board.AtPosition(position);

			if (_selectedCell != INVALID_CELL) {
				var actCell = VisualTreeHelper.GetChild(ChessBoard, _selectedCell) as Panel;
				actCell.Background = redBrush;
			}

			// mark possible moves
			if (pawn != null) {
				var a = _board.GetMoves(pawn);
				foreach (Chess.Fs.Position pos in a) {
					markPosition(pos);
				}
			}
		}

		private void onCellDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Panel cell = sender as Panel;
			clickStart = (int)cell.Tag;
		}

		private void onCellUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Panel cell = sender as Panel;
			int i = (int)cell.Tag;
			var pos = CellToPosition(i);
			if (i == clickStart) {
				//Console.WriteLine("click ! " + _selectedCell);
				if (_selectedCell != INVALID_CELL) {
					Chess.Fs.Pawn pawn = _board.AtPosition(CellToPosition(_selectedCell));
					if (pawn != null) {
						_board.Move(pawn, pos);
						forceRedraw();
					}
					_selectedCell = INVALID_CELL;
				} else {
					cell.Background = redBrush;
					_selectedCell = i;
				}
			}
			clickStart = INVALID_CELL;
		}

		private void Debug_Toogle(object sender, RoutedEventArgs e) {
			Console.WriteLine("debug: ");
		}

		// drawing
		private void forceRedraw() {
			for (int ii = 0; ii < 64; ii++) {
				Panel c = VisualTreeHelper.GetChild(ChessBoard, ii) as Panel;
				c.Children.Clear();
			}
			foreach (Chess.Fs.Pawn pawn in _board.Pieces) {
				drawPawn(pawn);
			}
		}

		private void drawPawn(Chess.Fs.Pawn pawn) {
			var p = pawn.p;
			int i = positionToCell(pawn.p);
			Panel cell = VisualTreeHelper.GetChild(ChessBoard, i) as Panel;

			Image image = new Image();
			image.Source = _imgCache[pawn.data];
			image.IsEnabled = false;

			cell.Children.Add(image);
		}

		private void markPosition(Chess.Fs.Position pos) {
			int i = positionToCell(pos);
			Panel cell = VisualTreeHelper.GetChild(ChessBoard, i) as Panel;
			cell.Background = orangeBrush;
		}

		// utils
		private int positionToCell(Chess.Fs.Position p) {
			return (7 - p.row) * 8 + p.col;
		}

		private Chess.Fs.Position CellToPosition(int i) {
			int r = 7 - (i / 8), c = i % 8;
			return new Chess.Fs.Position(r, c);
		}

		private Chess.Fs.Color getColor_(int i) {
			return ((i + i / 8) & 0x1) == 0 ? Chess.Fs.Color.WHITE : Chess.Fs.Color.BLACK;
		}

		private SolidColorBrush getColor(int i, bool select) {
			return getColor_(i) == Chess.Fs.Color.WHITE ?
				(select ? hoverBrush : defaultBrush) :
				(select ? alternateHoverBrush : alternateBrush);
		}

		private void redrawBoardBackground() {
			for (int i = 0; i < 64; i++) {
				Panel cell = VisualTreeHelper.GetChild(ChessBoard, i) as Panel;
				cell.Background = getColor(i, false);
			}
		}
	}


	public sealed class ImageCache {

		private static readonly string[] path = { 
			"/Resources/pawn.png",
			"/Resources/rook.png",
			"/Resources/knight.png",
			"/Resources/bishop.png",
			"/Resources/queen.png",
			"/Resources/king.png",
			"/Resources/pawn_b.png",
			"/Resources/rook_b.png",
			"/Resources/knight_b.png",
			"/Resources/bishop_b.png",
			"/Resources/queen_b.png",
			"/Resources/king_b.png" };

		private BitmapImage[] _imageCache = new BitmapImage[12];

		public void init() {
			for (int i = 0; i < path.Length; i++) {
				_imageCache[i] = new BitmapImage(new Uri(path[i], UriKind.Relative));
			}
		}

		public BitmapImage this[Chess.Fs.Piece p] {
			get {
				int i = -1;
				if (p.type_ == Chess.Fs.PawnType.PAWN) i = 0;
				else if (p.type_ == Chess.Fs.PawnType.ROOK) i = 1;
				else if (p.type_ == Chess.Fs.PawnType.KNIGHT) i = 2;
				else if (p.type_ == Chess.Fs.PawnType.BISHOP) i = 3;
				else if (p.type_ == Chess.Fs.PawnType.QUEEN) i = 4;
				else if (p.type_ == Chess.Fs.PawnType.KING) i = 5;
				if (p.player == Chess.Fs.Color.BLACK)
					i += 6;
				return _imageCache[i];
			}
		}
	}
}
