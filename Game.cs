using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Minesweeper
{
	// When revealing a tile, the gameState keeps track of if the game is won, lost or has to be continued
	public enum gameState {Continue, Won, Lost};
	// Game class that manages the setup
	class Game
	{
		Header _header;
		GameField _gameField;
		public Game()
		{
			_header=new Header();
			_gameField=new GameField(12, 12, 30);
		}

		public void init(Grid owner)
		{
			// Gamefield
			_gameField.populate();
			owner.Children.Add(_gameField);
			// Header
			_header.populate();
			_header.setMinesCountText(Convert.ToString(_gameField.maxBombs()));
			owner.Children.Add(_header);
		}
	}


	// Represents a single tile
	public class Tile:Button
	{
		static int _idCounter=0;
		int _id;
		int _row;
		int _col;
		// Each tile holds a reference to its 8 adjacent tiles
		Tile[] _adjacents;

		//Flags
		bool _isBomb=false;
		bool _isRevealed=false;
		bool _isMarked=false;
		int _bombsAround=0;
		GameField _owner;

		// Setter/Getter
		public bool isBomb() => _isBomb;
		public void setBomb(bool val) => _isBomb=val;
		public bool isRevealed() => _isRevealed;
		public void setRevealed(bool val) => _isRevealed=val;
		public bool isMarked() => _isMarked;
		public int bombsAround() => _bombsAround;

		public Tile adjacents(int index) => _adjacents[index];
		public Tile setAdjacents(int index, Tile tile) => _adjacents[index]=tile;


		public Tile(int row, int column, int btnWidth, int btnHeight, GameField owner)
		{
			_owner=owner;
			_owner.Children.Add(this);
			_id=_idCounter;
			_row=row;
			_col=column;
			VerticalAlignment=VerticalAlignment.Top;
			HorizontalAlignment=HorizontalAlignment.Left;
			Height=btnWidth;
			Width=btnHeight;
			Click+=btn_LClick;
			MouseRightButtonDown+=btn_RClick;
			_adjacents=new Tile[8];
			_idCounter++;
		}

		// Let the user mark a tile with the given symbol
		private void markTile(string symbol)
		{
			if(_isMarked==false)
			{
				Content=symbol;
				_isMarked=true;
			}
			else if(_isMarked==true)
			{
				Content="";
				_isMarked=false;
			}
		}

		// Tries to reveals all adjacent tiles
		public void revealNeighbours()
		{
			foreach(Tile tile in _adjacents)
			{
				if(tile is null)
					continue;
				else
					tile.reveal();
			}
		}

		// Reveal the tile
		public gameState reveal()
		{
			if(_isBomb==false)
			{
				if(_isRevealed==false)
				{
					if(_bombsAround>0)
					{
						IsEnabled=false;
						Content=_bombsAround;
						_isRevealed=true;
					}
					else
					{
						IsEnabled=false;
						_isRevealed=true;
						revealNeighbours();
					}
				}
			}
			else
			{
				// if a bomb was revealed - game is lost immediately
				return gameState.Lost;
			}

			return checkGameState();
		}

		// Checks the game state
		private gameState checkGameState()
		{
			int tileCount = _owner.rows()*_owner.cols();
			int revealedTiles = 0;
			foreach(Tile tile in _owner.tiles())
			{
				if(tile.isRevealed()&&!tile.isBomb())
					revealedTiles++;
			}
			if(revealedTiles==tileCount-_owner.maxBombs())
				return gameState.Won;
			else
				return gameState.Continue;
		}

		// Checks all 8 adjacent tiles and counts the bombs
		public void countBombsAround()
		{
			foreach(Tile tile in _adjacents)
			{
				if(tile is null)
					continue;
				else
					if(tile.isBomb())
					_bombsAround++;
			}
		}

		// Reveal a tile by left-clicking it
		private void btn_LClick(object sender, EventArgs e)
		{
			gameState state=reveal();
			if(state==gameState.Won)
				_owner.showAllBombs();
			else if(state==gameState.Lost)
				_owner.reset();
		}

		// Mark a tile by right-clicking it
		private void btn_RClick(object sender, EventArgs e)
		{
			markTile("X");
		}
	}

	// Base class for game grid objects
	public abstract class GameGrid:Grid
	{
		int _rows;
		int _cols;
		int _cellWidth;
		int _cellHeight;

		// Getter/Setter
		public int rows() => _rows;
		public int setRows(int val) => _rows=val;
		public int cols() => _cols;
		public int setCols(int val) => _cols=val;
		public int cellWidth() => _cellWidth;
		public int setCellWidth(int val) => _cellWidth=val;
		public int cellHeight() => _cellHeight;
		public int setCellHeight(int val) => _cellHeight=val;

		// Creates rows and columns for easy arrangement
		public void createRowsAndColumns(int rows, int cols)
		{
			for(int row = 0; row<rows; row++)
			{
				RowDefinitions.Add(new RowDefinition());
			}
			for(int col = 0; col<cols; col++)
			{
				ColumnDefinitions.Add(new ColumnDefinition());

			}
		}
	}

	// Displays information on the top
	public class Header:GameGrid
	{
		//GameTimer _gameTimer;
		TextBlock _tb_minesCount;
		TextBlock _tb_headerMinesCount;

		// Setter/Getter
		public void setMinesCountText(string val) => _tb_minesCount.Text=val;

		public Header(int rows=2, int cols=7, int cellSize=30)
		{
			setCellWidth(cellSize+15);
			setCellHeight(cellSize);
			setRows(rows);
			setCols(cols);
			Width=cols*cellWidth();
			Height=rows*cellHeight();
			HorizontalAlignment=HorizontalAlignment.Center;
			VerticalAlignment=VerticalAlignment.Top;
			ShowGridLines=false;

			createRowsAndColumns(rows, cols);

			// Mines count textblock
			_tb_minesCount=new TextBlock();
			_tb_minesCount.Text="test";
			// Mines count header textblock
			_tb_headerMinesCount=new TextBlock();
			_tb_headerMinesCount.Text="Mines:";
		}


		// Fills the header with content
		public void populate()
		{
			// Mines count
			Children.Add(_tb_minesCount);
			Grid.SetRow(_tb_minesCount, 1);
			Grid.SetColumn(_tb_minesCount, 1);
			// header
			Children.Add(_tb_headerMinesCount);
			Grid.SetRow(_tb_headerMinesCount, 0);
			Grid.SetColumn(_tb_headerMinesCount, 1);
		}
	}

	// Represents the game field
	public class GameField:GameGrid
	{
		int _maxBombs;
		Tile[,] _tiles;
		public Tile[,] tiles() => _tiles;

		// Setter/Getter
		public int maxBombs() => _maxBombs;

		public GameField(int rows, int cols, int btnSize, int bombPercentage=10)
		{
			setCellWidth(btnSize);
			setCellHeight(btnSize);
			Width=cols*cellWidth();
			Height=rows*cellHeight();
			setRows(rows);
			setCols(cols);
			_maxBombs=(rows*cols)/bombPercentage;
			Margin=new Thickness(0, 80, 0, 0);
			HorizontalAlignment=HorizontalAlignment.Left;
			VerticalAlignment=VerticalAlignment.Top;
			ShowGridLines=true;
			createRowsAndColumns(rows, cols);
		}

		// Populates the game field with tiles
		public void populate()
		{
			_tiles=new Tile[rows(), cols()];
			// Create tiles
			for(int row = 0; row<rows(); row++)
			{
				for(int col = 0; col<cols(); col++)
				{
					_tiles[row, col]=new Tile(row, col, cellWidth(), cellHeight(), this);
					Grid.SetRow(_tiles[row, col], row);
					Grid.SetColumn(_tiles[row, col], col);
				}
			}
			// Place bombs
			placeBombs();
			// Assign the tile's adjacent tiles
			for(int row = 0; row<rows(); row++)
			{
				for(int col = 0; col<cols(); col++)
				{
					if(row-1>=0&&col-1>=0)
						_tiles[row, col].setAdjacents(0, _tiles[row-1, col-1]);
					if(row-1>0)
						_tiles[row, col].setAdjacents(1, _tiles[row-1, col]);
					if(row-1>0&&col+1<cols())
						_tiles[row, col].setAdjacents(2, _tiles[row-1, col+1]);
					if(col-1>0)
						_tiles[row, col].setAdjacents(3, _tiles[row, col-1]);
					if(col+1<cols())
						_tiles[row, col].setAdjacents(4, _tiles[row, col+1]);
					if(row+1<rows()&&col-1>0)
						_tiles[row, col].setAdjacents(5, _tiles[row+1, col-1]);
					if(row+1<rows())
						_tiles[row, col].setAdjacents(6, _tiles[row+1, col]);
					if(row+1<rows()&&col+1<cols())
						_tiles[row, col].setAdjacents(7, _tiles[row+1, col+1]);
				}
			}
			// Count the bombs a tile is surrounded by
			// The amount is saved inside each tile (int myTile._bombsAround)
			for(int row = 0; row<rows(); row++)
			{
				for(int col = 0; col<cols(); col++)
				{
					_tiles[row, col].countBombsAround();
				}
			}
		}

		public void showAllBombs()
		{
			for(int row = 0; row<rows(); row++)
			{
				for(int col = 0; col<cols(); col++)
				{
					if(_tiles[row, col].isBomb())
						_tiles[row, col].Content="*";
				}
			}
		}

		// Reset the game field
		public void reset()
		{
			populate();
		}

		// Game is lost - so show all bombs
		public void gameOver()
		{
			showAllBombs();
		}

		// Iterates through the grid and places bombs randomly (called in populate)
		private void placeBombs()
		{
			Random rnd=new Random();
			int bombsPlaced=0;
			while(bombsPlaced<_maxBombs)
			{
				for(int row=0; row<rows(); row++)
				{
					for(int col=0; col<cols(); col++)
					{
						if(rnd.Next(1, 50)==1)
						{
							if(!_tiles[row, col].isBomb())
							{
								_tiles[row, col].setBomb(true);
								bombsPlaced++;
								if(bombsPlaced>=_maxBombs)
									return;
							}
						}
					}
				}
			}
		}
	}
}
